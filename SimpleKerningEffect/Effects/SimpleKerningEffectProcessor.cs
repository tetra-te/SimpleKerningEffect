using System.Text.RegularExpressions;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using SimpleKerningEffect.ForVideoEffectChain;
using SimpleKerningEffect.Patch;
using System.Globalization;

namespace SimpleKerningEffect.Effects
{
    public class SimpleKerningEffectProcessor : IVideoEffectProcessor
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
            var inputText = Storage.GetText(effectDescription);

            if (string.IsNullOrEmpty(inputText))
            {
                chain.ClearChain();
                return drawDesc;
            }
            
            var inputIndex = effectDescription.InputIndex + 1;
            var inputCount = effectDescription.InputCount;
            var inputTextOneLine = inputText.Replace("\r\n", "")
                                            .Replace("\n", "")
                                            .Replace("\r", "");

            if (string.IsNullOrEmpty(inputTextOneLine))
            {
                chain.ClearChain();
                return drawDesc;
            }

            if (effectDescription.InputIndex < 0 || effectDescription.InputIndex >= Length(inputTextOneLine))
            {
                chain.ClearChain();
                return drawDesc;
            }

            var inputElem= ElementAt(inputTextOneLine, effectDescription.InputIndex);

            var match = false;

            if (item.Odd && !match)
            {
                match = (inputIndex % 2 == 1);
            }
            if (item.Even && !match)
            {
                match = (inputIndex % 2 == 0);
            }
            if (item.Hiragana && !match && inputElem is [var c])
            {
                match = ('\u3041' <= c && c <= '\u3096') ||
                        ('\u309D' <= c && c <= '\u309F') ||
                        (c == 'ー');
            }
            if (item.Katakana && !match && inputElem is [var d])
            {
                match = ('\u30A1' <= d && d <= '\u30FA') ||
                        ('\u30FD' <= d && d <= '\u30FF') ||
                        (d == 'ー');
            }
            if (item.Kanji && !match && inputElem is [var e])
            {
                match = ('\u4E00' <= e && e <= '\u9FFF') ||
                        ('\u3400' <= e && e <= '\u4DBF') ||
                        ('\uF900' <= e && e <= '\uFAFF');
            }
            if (item.Number && !match && inputElem is [var f])
            {
                match = char.IsDigit(f);
            }
            if (item.Alphabet && !match && inputElem is [var g])
            {
                match = ('a' <= g && g <= 'z') ||
                        ('A' <= g && g <= 'Z');
            }
            if (!match)
            {
                match = ContainsNum(item.Index, inputIndex, inputCount);
            }
            if (!match)
            {
                var inputLine = inputText.Split("\r\n");
                var lineCount = inputLine.Length;

                int currentLine = 0, textCount = 0;

                for (int i = 0; i < lineCount; i++)
                {
                    textCount += Length(inputLine[i]);
                    if (textCount >= inputIndex)
                    {
                        currentLine = i + 1;
                        break;
                    }
                }

                match = ContainsNum(item.Line, currentLine, lineCount);
            }
            if (!match)
            {
                var texts = item.Texts.Split([",", "\r\n"], StringSplitOptions.None);

                var shouldBreak = false;

                for (int i = 0; i < texts.Length; i++)
                {
                    int index = 0;

                    while ((index = inputTextOneLine.IndexOf(texts[i], index, StringComparison.Ordinal)) != -1 && texts[i] != "")
                    {
                        var (startElem, endElem) = CodeUnitRangeToTextElementRange(inputTextOneLine, index, texts[i].Length);
                        int curElem = effectDescription.InputIndex;

                        if (startElem <= curElem && curElem < endElem)
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
            if (!match && pattern != "")
            {
                try
                {
                    var matches = Regex.Matches(inputTextOneLine, pattern);

                    foreach (Match matchRegex in matches)
                    {
                        var (startElem, endElem) = CodeUnitRangeToTextElementRange(inputTextOneLine, matchRegex.Index, matchRegex.Length);
                        var curElem = effectDescription.InputIndex;

                        if (startElem <= curElem && curElem < endElem)
                        {
                            match = true;
                            break;
                        }
                    }
                }
                catch { }
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

        static int InvertWithCaret(string input, int length)
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

        static bool ContainsNum(string numbersExpression, int searchTarget, int length)
        {                              
            numbersExpression = numbersExpression.Replace(" ", "");

            if (string.IsNullOrEmpty(numbersExpression) || searchTarget > length)
                return false;

            var match = false;

            var part = numbersExpression.Split([",", "\r\n"], StringSplitOptions.None);

            for (int i = 0; i < part.Length; i++)
            {
                var numerics = Regex.IsMatch(part[i], @"^(\^|)[0-9]{1,9}$");
                var hyphen = Regex.IsMatch(part[i], @"^(\^|)[0-9]{1,9}-(\^|)[0-9]{1,9}$");

                if (!numerics && !hyphen && part[i] != "")
                {
                    match = false;
                    break;
                }
                if (numerics)
                {
                    match = InvertWithCaret(part[i], length) == searchTarget ? true : match;
                }
                if (hyphen)
                {
                    var piece = part[i].Split("-");
                    var first = InvertWithCaret(piece[0], length);
                    var second = InvertWithCaret(piece[1], length);
                    if (first > second)
                    {
                        (first, second) = (second, first);
                    }
                    match = (first <= searchTarget && searchTarget <= second) ? true : match;
                }
            }

            return match;
        }

        private static int Length(string str)
        {
            return new StringInfo(str).LengthInTextElements;
        }

        private static string ElementAt(string str, int index)
        {
            var si = new StringInfo(str);

            if ((uint)index >= (uint)si.LengthInTextElements)
                throw new ArgumentOutOfRangeException(nameof(index));

            return si.SubstringByTextElements(index, 1);
        }

        private static (int StartElem, int EndElem) CodeUnitRangeToTextElementRange(string s, int codeStart, int codeLength)
        {
            var starts = StringInfo.ParseCombiningCharacters(s);
            var startPos = Array.BinarySearch(starts, codeStart);
            var startElem = startPos >= 0 ? startPos : ~startPos - 1;

            int codeEnd = codeStart + codeLength;
            int endPos = Array.BinarySearch(starts, codeEnd);
            int endElem = endPos >= 0 ? endPos : ~endPos;

            return (startElem, endElem);
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
