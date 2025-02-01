using SimpleKerningEffect.ForVideoEffectChain;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

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
            chain = new VideoEffectChainNode(devices, item.Effects, new());
            oldLenOfEffects = item.Effects.Count;
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            var drawDesc = effectDescription.DrawDescription;
            var inputIndex = effectDescription.InputIndex + 1;
            
            var index = item.Index.Replace(" ", "");
            var part = index.Split(",");
            var match = false;
            for (int i = 0; i < part.Length; i++)
            {
                var numerics = Regex.IsMatch(part[i], @"^[0-9]{1,9}$");
                var hyphen = Regex.IsMatch(part[i], @"^[0-9]{1,9}-[0-9]{1,9}$");
                if (!numerics && !hyphen) return drawDesc;
                if (numerics)
                    match = (int.Parse(part[i]) == inputIndex) ? true : match;
                if (hyphen)
                {
                    var piece = part[i].Split("-");
                    var first = int.Parse(piece[0]);
                    var second = int.Parse(piece[1]);
                    if (first > second) return drawDesc;
                    match = (first <= inputIndex) && (inputIndex <= second) ? true : match;                  
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

            FrameAndLength fl = new(frame: frame, length: length);

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
                chain.UpdateChain(item.Effects, fl);
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
        }

        public void Dispose()
        {
            chain.Dispose();
        }

    }
}