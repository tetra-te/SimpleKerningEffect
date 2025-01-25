using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SimpleKerningEffect.Effects
{
    [VideoEffect("簡易カーニング", ["テキスト"], ["kerning", "文字", "テキスト", "text", "プラグイン", "plugin"], isAviUtlSupported:false, isEffectItemSupported:false)]
    internal class SimpleKerningEffect : VideoEffectBase
    {
        public override string Label => "簡易カーニング";

        [Display(GroupName = "簡易カーニング\r\n対象の文字位置を数字で指定します\r\nカンマで複数指定、ハイフンで範囲指定", Name = "文字位置", Description = "何文字目を対象にするか設定します\r\n例：1,3,5-10")]
        [TextEditor(AcceptsReturn = true)]
        public string Index { get => index; set => Set(ref index, value); }
        string index = string.Empty;

        [Display(GroupName = "描画", Name = "X", Description = "描画位置（横方向）")]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation X { get; } = new Animation(0, -99999, 99999);

        [Display(GroupName = "描画", Name = "Y", Description = "描画位置（縦方向）")]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Y { get; } = new Animation(0, -99999, 99999);

        [Display(GroupName = "描画", Name = "Z", Description = "描画位置（奥行き）")]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Z { get; } = new Animation(0, -99999, 99999);

        [Display(GroupName = "描画", Name = "不透明度", Description = "不透明度")]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation Opacity { get; } = new Animation(100, 0, 100);

        [Display(GroupName = "描画", Name = "拡大率", Description = "拡大率")]
        [AnimationSlider("F1", "%", 0, 400)]
        public Animation Zoom { get; } = new Animation(100, 0, 5000);

        [Display(GroupName = "描画", Name = "回転角", Description = "回転させる角度（右回り）")]
        [AnimationSlider("F1", "°", -360, 360)]
        public Animation Rotation { get; } = new Animation(0, -36000, 36000);

        [Display(GroupName = "描画", Name = "左右反転", Description = "左右反転")]
        [ToggleSlider]
        public bool Invert { get => invert; set => Set(ref invert, value); }
        bool invert = false;

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new SimpleKerningEffectProcessor(this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [X, Y, Z, Opacity, Zoom, Rotation];
    }
}
