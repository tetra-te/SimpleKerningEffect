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
  
            var inputText = Storage.GetText(effectDescription);

            /*var indexMatch = ContainNum(item.Index, inputIndex, inputCount);
            var lineMatch = false;
            var toggleMatch = false;
            var textMatch = false;
            var regexMatch = false;*/

            var match = ContainNum(item.Index, inputIndex, inputCount);

            if (inputText != "" && !match)
            {
                var inputLine = inputText.Split("\r\n");
                var lineCount = inputLine.Length;

                int currentLine = 0, textCount = 0;

                for (int i = 0; i < lineCount; i++)
                {
                    textCount += inputLine[i].Length;
                    if (textCount >= inputIndex)
                    {
                        currentLine = i + 1;
                        break;
                    }  
                }

                match = ContainNum(item.Line, currentLine, lineCount);

                var inputTextOneLine = inputText.Replace("\r\n", "");
                var inputChar = inputTextOneLine[inputIndex - 1];

                if (item.Odd && !match)
                {
                    match = (inputIndex % 2 == 1);
                }
                if (item.Even && !match)
                {
                    match = (inputIndex % 2 == 0);
                }
                if (item.Hiragana && !match)
                {
                    match = ('\u3041' <= inputChar && inputChar <= '\u3096') ||
                                  ('\u309D' <= inputChar && inputChar <= '\u309F') ||
                                  (inputChar == 'ー');
                }
                if (item.Katakana && !match)
                {
                    match = ('\u30A1' <= inputChar && inputChar <= '\u30FA') ||
                                  ('\u30FD' <= inputChar && inputChar <= '\u30FF') ||
                                  (inputChar == 'ー');
                }
                if (item.Kanji && !match)
                {
                    match = ('\u4E00' <= inputChar && inputChar <= '\u9FFF') ||
                                  ('\u3400' <= inputChar && inputChar <= '\u4DBF') ||
                                  ('\uF900' <= inputChar && inputChar <= '\uFAFF');
                }
                if (item.Number && !match)
                {
                    match = char.IsDigit(inputChar);
                }
                if (item.Alphabet && !match)
                {
                    match = ('a' <= inputChar && inputChar <= 'z') ||
                            ('A' <= inputChar && inputChar <= 'Z');
                }

                var texts = item.Texts.Split(",");

                if (!texts.Contains("") && !match) //無限ループ防止
                {
                    var shouldBreak = false;

                    for (int i = 0; i < texts.Length; i++)
                    {
                        int index = 0;

                        while ((index = inputTextOneLine.IndexOf(texts[i], index)) != -1)
                        {
                            if ((index <= inputIndex -1) && (inputIndex <= index + texts[i].Length))
                            {
                                match = true;
                                shouldBreak = true;
                                break;
                            }
                            index += texts[i].Length;
                        }

                        if (shouldBreak) break;
                    }
                }

                var pattern = item.Regex;

                if (pattern != "" && !match)
                {
                    try
                    {
                        var matches = Regex.Matches(inputTextOneLine, pattern);

                        foreach (Match matchRegex in matches)
                        {
                            if ((matchRegex.Index <= inputIndex - 1) && (inputIndex <= matchRegex.Index + matchRegex.Length))
                            {
                                match = true;
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }
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

        static bool ContainNum(string setting, int num, int length)
        {
            bool match = false;

            int InvertCaret(string input)
            {
                if (Regex.IsMatch(input, @"^\^[0-9]{1,9}"))
                {
                    var trimmed = input[1..];
                    return length - int.Parse(trimmed) + 1;
                }
                else
                {
                    return int.Parse(input);
                }
            }

            setting = setting.Replace(" ", "");
            var part = setting.Split(",");

            for (int i = 0; i < part.Length; i++)
            {
                var numerics = Regex.IsMatch(part[i], @"^(\^|)[0-9]{1,9}$");
                var hyphen = Regex.IsMatch(part[i], @"^(\^|)[0-9]{1,9}-(\^|)[0-9]{1,9}$");

                if (!numerics && !hyphen)
                {
                    match = false;
                    break;
                }
                if (numerics)
                {
                    match = InvertCaret(part[i]) == num ? true : match;
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
                    match = (first <= num && num <= second) ? true : match;
                }
            }

            return match;
        }

        public void ClearInput()
        {
            chain.ClearInput();
        }

        public void SetInput(ID2D1Image? input)
        {
            chain.SetInput(input);
            chain.UpdateChain(item.Effects);
        }

        public void Dispose()
        {
            chain.Dispose();
        }

    }
}
