using Vortice.Direct2D1;
using YukkuriMovieMaker.Player.Video;
using SimpleKerningEffect.Patch;

namespace SimpleKerningEffect.Effects
{
    public class TextLineStepEffectProcessor : IVideoEffectProcessor
    {
        ID2D1Image? input;

        TextLineStepEffect item;

        public ID2D1Image Output => input ?? throw new NullReferenceException(nameof(input) + " is null");

        public TextLineStepEffectProcessor(TextLineStepEffect item)
        {
            this.item = item;
        }

        public void ClearInput()
        {
            input = null;
        }

        public void Dispose()
        {
        }

        public void SetInput(ID2D1Image? input)
        {
            this.input = input;
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            var text = Storage.GetText(effectDescription);
            
            if (string.IsNullOrEmpty(text))
                return effectDescription.DrawDescription;

            var inputIndex = effectDescription.InputIndex;
            var lines = text.Split("\r\n");

            int textCount = 0;
            int currentLine = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                textCount += lines[i].Length;
                if (textCount > inputIndex)
                {
                    currentLine = i;
                    break;
                } 
            }

            var keyFrames = Storage.GetKeyFrames(effectDescription);

            var frame = effectDescription.ItemPosition.Frame;

            int currentIndex = 0;

            for (int i = 0; i < keyFrames.Count; i++)
            {
                if (keyFrames[^1] <= frame)
                {
                    currentIndex = keyFrames.Count;
                    break;
                }

                if (frame < keyFrames[i])
                {
                    currentIndex = i;
                    break;
                }
            }

            var zoom = currentLine == currentIndex ? 1 : 0;

            return effectDescription.DrawDescription with
            {
                Zoom = effectDescription.DrawDescription.Zoom * zoom
            };
        }
    }
}