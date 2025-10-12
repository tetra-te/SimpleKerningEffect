using Vortice.Direct2D1;
using YukkuriMovieMaker.Player.Video;

namespace SimpleKerningEffect.Effects
{
    public class TextSpaceVerticalEffectProcessor : IVideoEffectProcessor
    {
        readonly TextSpaceVerticalEffect item;
        ID2D1Image? input;

        public ID2D1Image Output => input ?? throw new NullReferenceException(nameof(input) + "is null");

        public TextSpaceVerticalEffectProcessor(TextSpaceVerticalEffect item)
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

            double y = 0;
            if (start <= inputIndex && inputIndex <= end)
                y = space * difference;
            if (item.Adjust && inputIndex < start)
                y = space * (start - median);
            if (item.Adjust && end < inputIndex)
                y = space * (end - median);
            return
                drawDesc with
                {
                    Draw = new(
                    drawDesc.Draw.X,
                    drawDesc.Draw.Y + (float)y,
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
