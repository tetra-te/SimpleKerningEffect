using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SimpleKerningEffect.Effects
{
    [VideoEffect("テキスト行を中間点ごとに表示", ["テキスト"], ["text", "line", "keyframe"], IsEffectItemSupported = false, IsAviUtlSupported = false)]
    public class TextLineStepEffect : VideoEffectBase
    {
        public override string Label => "テキスト行を中間点ごとに表示";

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new TextLineStepEffectProcessor(this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];
    }
}