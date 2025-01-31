using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace SimpleKerningEffect.ForVideoEffectChain
{
    internal class FrameAndLength
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
    }
}
