using Vortice.Direct2D1;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project.Items;
using SimpleKerningEffect.Patch;

namespace SimpleKerningEffect.Effects
{
    internal class TextSpaceEffectProcessor : IVideoEffectProcessor
    {
        readonly TextSpaceEffect item;
        ID2D1Image? input;

        public ID2D1Image Output => input ?? throw new NullReferenceException(nameof(input) + "is null");

        public TextSpaceEffectProcessor(TextSpaceEffect item)
        {
            this.item = item;
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            var drawDesc = effectDescription.DrawDescription;
            var start = (int)item.Start;
            var end = (int)item.End;
            if (start >= end) return drawDesc;

            float median = (start + end) / 2f;
            var inputIndex = effectDescription.InputIndex + 1;
            var difference = inputIndex - median;

            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var space = (float)item.Space.GetValue(frame, length, fps);

            float draw = 0;
            if (start <= inputIndex && inputIndex <= end)
                draw = space * difference;
            if (item.Adjust && inputIndex < start)
                draw = space * (start - median);
            if (item.Adjust && end < inputIndex)
                draw = space * (end - median);

            float x = 0;
            float y = 0;

            var writingDirection = item.WritingDirection;
            var basePoint = Storage.GetBasePoint(effectDescription);

            switch (writingDirection)
            {
                case WritingDirection.Auto:
                    if (isBeside(basePoint))
                        x = draw;
                    else
                        y = draw;
                    break;
                case WritingDirection.Beside:
                    x = draw;
                    break;
                case WritingDirection.Vertical:
                    y = draw;
                    break;
            }

            return
                drawDesc with
                {
                    Draw = new(
                    drawDesc.Draw.X + x,
                    drawDesc.Draw.Y + y,
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

        static bool isBeside(BasePoint basePoint)
        {
            switch (basePoint)
            {
                case BasePoint.LeftTop:
                    return true;
                case BasePoint.CenterTop:
                    return true;
                case BasePoint.RightTop:
                    return true;
                case BasePoint.LeftCenter:
                    return true;
                case BasePoint.CenterCenter:
                    return true;
                case BasePoint.RightCenter:
                    return true;
                case BasePoint.LeftBottom:
                    return true;
                case BasePoint.CenterBottom:
                    return true;
                case BasePoint.RightBottom:
                    return true;
                case BasePoint.VTopRight:
                    return false;
                case BasePoint.VCenterRight:
                    return false;
                case BasePoint.VBottomRight:
                    return false;
                case BasePoint.VTopCenter:
                    return false;
                case BasePoint.VCenterCenter:
                    return false;
                case BasePoint.VBottomCenter:
                    return false;
                case BasePoint.VTopLeft:
                    return false;
                case BasePoint.VCenterLeft:
                    return false;
                case BasePoint.VBottomLeft:
                    return false;
                default:
                    return true;
            }
        }
    }
}
