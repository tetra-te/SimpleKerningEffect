using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SimpleKerningEffect.Effects
{
    [VideoEffect("簡易カーニング", ["テキスト"], ["kerning", "文字", "テキスト", "text", "プラグイン", "plugin"], isAviUtlSupported:false, isEffectItemSupported:false)]
    public class SimpleKerningEffect : VideoEffectBase
    {
        public override string Label => "簡易カーニング";

        [Display(GroupName = "カーニング対象\r\n,で複数指定　-で範囲指定　^で末尾から指定", Name = "文字位置", Description = "何文字目を対象にするか設定します\r\n例：\r\n1,3,5-10\r\n10-^3")]
        [TextEditor(AcceptsReturn = true)]
        public string Index { get => index; set => Set(ref index, value); }
        string index = string.Empty;

        [Display(GroupName = "カーニング対象\r\n,で複数指定　-で範囲指定　^で末尾から指定", Name = "行位置", Description = "何行目を対象にするか設定します\r\n例：\r\n1,3,5-10\r\n10-^3")]
        [TextEditor(AcceptsReturn = true)]
        public string Line { get => line; set => Set(ref line, value); }
        string line = string.Empty;

        [Display(GroupName = "カーニング対象", Name = "奇数文字目", Description = "奇数文字目の文字を対象に含める")]
        [ToggleSlider]
        public bool Odd { get => odd; set => Set(ref odd, value); }
        bool odd = false;

        [Display(GroupName = "カーニング対象", Name = "偶数文字目", Description = "偶数文字目の文字を対象に含める")]
        [ToggleSlider]
        public bool Even { get => even; set => Set(ref even, value); }
        bool even = false;

        [Display(GroupName = "カーニング対象", Name = "ひらがな", Description = "ひらがなを対象に含める")]
        [ToggleSlider]
        public bool Hiragana { get => hiragana; set => Set(ref hiragana, value); }
        bool hiragana = false;

        [Display(GroupName = "カーニング対象", Name = "カタカナ", Description = "カタカナを対象に含める")]
        [ToggleSlider]
        public bool Katakana { get => katakana; set => Set(ref katakana, value); }
        bool katakana = false;

        [Display(GroupName = "カーニング対象", Name = "漢字", Description = "漢字を対象に含める")]
        [ToggleSlider]
        public bool Kanji { get => kanji; set => Set(ref kanji, value); }
        bool kanji = false;

        [Display(GroupName = "カーニング対象", Name = "数字", Description = "数字を対象に含める")]
        [ToggleSlider]
        public bool Number { get => number; set => Set(ref number, value); }
        bool number = false;

        [Display(GroupName = "カーニング対象", Name = "アルファベット", Description = "アルファベットを対象に含める")]
        [ToggleSlider]
        public bool Alphabet { get => alphabet; set => Set(ref alphabet, value); }
        bool alphabet = false;

        [Display(GroupName = "カーニング対象", Name = "対象文字", Description = "対象に含めるテキストを指定します\r\nカンマ区切り可能")]
        [TextEditor(AcceptsReturn = true)]
        public string Texts { get => texts; set => Set(ref texts, value); }
        string texts = string.Empty;

        [Display(GroupName = "カーニング対象", Name = "正規表現", Description = "正規表現にマッチする部分を対象に含めます")]
        [TextEditor(AcceptsReturn = true)]
        public string Regex { get => regex; set => Set(ref regex, value); }
        string regex = string.Empty;

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

        [Display(GroupName = "簡易カーニング内のエフェクト", Name = "", Description = "対象文字にかける映像エフェクト")]
        [VideoEffectSelector(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<IVideoEffect> Effects { get => effects; set => Set(ref effects, value); }
        ImmutableList<IVideoEffect> effects = [];

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new SimpleKerningEffectProcessor(this, devices);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [X, Y, Z, Opacity, Zoom, Rotation, .. Effects];
    }
}
