using YukkuriMovieMaker.Player.Video;

namespace SimpleKerningEffect.ForVideoEffectChain
{
    public class FrameAndLength
    {
        public int Frame;
        public int Length;

        public FrameAndLength()
        {
            Length = 1;
        }

        public FrameAndLength(int frame, int length)
        {
            Frame = frame;
            Length = length;
        }

        public FrameAndLength(FrameAndLength origin)
        {
            Frame = origin.Frame;
            Length = origin.Length;
        }

        public void CopyFrom(FrameAndLength origin)
        {
            Frame = origin.Frame;
            Length = origin.Length;
        }

        public void UpdateFrom(TimelineItemSourceDescription description)
        {
            Frame = description.ItemPosition.Frame;
            Length = description.ItemDuration.Frame;
        }
    }
}
