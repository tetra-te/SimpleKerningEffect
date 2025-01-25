using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SimpleKerningEffect.Effects
{
    [VideoEffect("文字間隔調整（横書）", ["テキスト"], ["kerning", "文字", "テキスト", "text", "プラグイン", "plugin"], isAviUtlSupported:false, isEffectItemSupported:false)]
    internal class TextSpaceBesideEffect : VideoEffectBase
    {
        public override string Label => $"文字間隔調整（横書） {Space.GetValue(0, 1, 30):F1}";

        [Display(GroupName = "文字間隔調整（横書）", Name = "開始", Description = "調整する文字の開始位置")]
        [TextBoxSlider("F0", "", 1, 5)]
        [DefaultValue(1d)]
        [Range(1, 99999)]
        public double Start { get => start; set => Set(ref start, value); }
        double start = 1;
        
        [Display(GroupName = "文字間隔調整（横書）", Name = "終了", Description = "調整する文字の終了位置")]
        [TextBoxSlider("F0", "", 1, 5)]
        [DefaultValue(1d)]
        [Range(1, 99999)]
        public double End { get => end; set => Set(ref end, value); }
        double end = 1;

        [Display(GroupName = "文字間隔調整（横書）", Name = "文字間隔", Description = "文字間隔")]
        [AnimationSlider("F1", "", -50, 50)]
        public Animation Space { get; } = new Animation(0, -99999, 99999);
        
        [Display(GroupName = "文字間隔調整（横書）", Name = "全体を調整", Description = "範囲外の文字の位置を調整する")]
        [ToggleSlider]
        public bool Adjust { get => adjust; set => Set(ref adjust, value); }
        bool adjust = false;

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new TextSpaceBesideEffectProcessor(this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Space];
    }
}
