using Vortice.Direct2D1;
using YukkuriMovieMaker.Player.Video;

namespace SimpleKerningEffect.Effects
{
    public class TextSpaceBesideEffectProcessor : IVideoEffectProcessor
    {
        readonly TextSpaceBesideEffect item;
        ID2D1Image? input;

        public ID2D1Image Output => input ?? throw new NullReferenceException(nameof(input) + "is null");

        public TextSpaceBesideEffectProcessor(TextSpaceBesideEffect item)
        {
            this.item = item;
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            var drawDesc = effectDescription.DrawDescription;
            var start = (int)item.Start;
            var end = (int)item.End;
            if (start >= end) return drawDesc;

            double median = (start + end) / 2f;
            var inputIndex = effectDescription.InputIndex + 1;
            var difference = inputIndex - median;

            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var space = item.Space.GetValue(frame, length, fps);

            double x = 0;
            if (start <= inputIndex && inputIndex <= end)
                x = space * difference;
            if (item.Adjust && inputIndex < start)
                x = space * (start - median);
            if (item.Adjust && end < inputIndex)
                x = space * (end - median);
            return
                drawDesc with
                {
                    Draw = new(
                    drawDesc.Draw.X + (float)x,
                    drawDesc.Draw.Y,
                    drawDesc.Draw.Z)
                };
        }
        public void ClearInput()
        {
            input = null;
        }
        public void SetInput(ID2D1Image? input)
        {
            this.input = input;
        }

        public void Dispose()
        {

        }

    }
}
