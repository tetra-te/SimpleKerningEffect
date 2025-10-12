using System.ComponentModel.DataAnnotations;

namespace SimpleKerningEffect.Effects
{
    public enum WritingDirection
    {
        [Display(Name = "自動", Description = "横書き・縦書きを自動判定します")]
        Auto,
        [Display(Name = "横書き", Description = "文字を横書きとして扱います")]
        Beside,
        [Display(Name = "縦書き", Description = "文字を縦書きとして扱います")]
        Vertical
    }
}
