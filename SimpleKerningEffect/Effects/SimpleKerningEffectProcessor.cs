using System.Text.RegularExpressions;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using SimpleKerningEffect.ForVideoEffectChain;

namespace SimpleKerningEffect.Effects
{
    internal class SimpleKerningEffectProcessor : IVideoEffectProcessor
    {
        readonly SimpleKerningEffect item;
        readonly VideoEffectChainNode chain;
        int oldLenOfEffects;
        //ID2D1Image? input;

        public ID2D1Image Output => chain.Output ?? throw new NullReferenceException("output of " + nameof(chain) + "is null");

        public SimpleKerningEffectProcessor(SimpleKerningEffect item, IGraphicsDevicesAndContext devices)
        {
            this.item = item;
            chain = new VideoEffectChainNode(devices);
            oldLenOfEffects = item.Effects.Count;
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            var drawDesc = effectDescription.DrawDescription;
            var inputIndex = effectDescription.InputIndex + 1;
            var inputCount = effectDescription.InputCount;

            var index = item.Index.Replace(" ", "");
            var part = index.Split(",");
            var match = false;

            int InvertCaret(string input)
            {
                if (Regex.IsMatch(input, @"^\^[0-9]{1,9}"))
                {
                    var trimmed = input[1..];
                    return inputCount - int.Parse(trimmed) + 1;
                }
                else
                {
                    return int.Parse(input);
                }
            }

            for (int i = 0; i < part.Length; i++)
            {
                var numerics = Regex.IsMatch(part[i], @"^(\^|)[0-9]{1,9}$");
                var hyphen = Regex.IsMatch(part[i], @"^(\^|)[0-9]{1,9}-(\^|)[0-9]{1,9}$");
                var odd = Regex.IsMatch(part[i], @"^(o|O)$");
                var even = Regex.IsMatch(part[i], @"^(e|E)$");

                if (!numerics && !hyphen && !odd && !even)
                {
                    chain.ClearChain();
                    return drawDesc;
                }
                if (numerics)
                {
                    match = (InvertCaret(part[i]) == inputIndex) ? true : match;
                }
                if (hyphen)
                {
                    var piece = part[i].Split("-");
                    var first = InvertCaret(piece[0]);
                    var second = InvertCaret(piece[1]);
                    if (first > second)
                    {
                        (first, second) = (second, first);
                    }
                    match = (first <= inputIndex) && (inputIndex <= second) ? true : match;
                }
                if (odd)
                {
                    match = (inputIndex % 2 == 1) ? true : match;
                }
                if (even)
                {
                    match = (inputIndex % 2 == 0) ? true : match;
                }
            }
            if (!match)
            {
                chain.ClearChain();
                return drawDesc;
            }

            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var x = item.X.GetValue(frame, length, fps);
            var y = item.Y.GetValue(frame, length, fps);
            var z = item.Z.GetValue(frame, length, fps);
            var opacity = item.Opacity.GetValue(frame, length, fps) / 100;
            var zoom = item.Zoom.GetValue(frame, length, fps) / 100;
            var rotation = item.Rotation.GetValue(frame, length, fps);
            var invert = item.Invert;

            DrawDescription newDescription = drawDesc with
            {
                Draw = new(
                    drawDesc.Draw.X + (float)x,
                    drawDesc.Draw.Y + (float)y,
                    drawDesc.Draw.Z + (float)z),
                Opacity = drawDesc.Opacity * opacity,
                Zoom = drawDesc.Zoom * (float)zoom,
                Rotation = new(
                    drawDesc.Rotation.X,
                    drawDesc.Rotation.Y,
                    drawDesc.Rotation.Z + (float)rotation),
                Invert = invert ? !drawDesc.Invert : drawDesc.Invert
            };

            if (oldLenOfEffects + item.Effects.Count > 0)
            {
                oldLenOfEffects = item.Effects.Count;
                chain.UpdateChain(item.Effects);
                newDescription = chain.UpdateOutputAndDescription(effectDescription, newDescription);
            }

            return newDescription;
        }
        public void ClearInput()
        {
            //input = null;
            chain.ClearInput();
        }

        public void SetInput(ID2D1Image? input)
        {
            //this.input = input;
            chain.SetInput(input);
            chain.UpdateChain(item.Effects);
        }

        public void Dispose()
        {
            chain.Dispose();
        }

    }
}