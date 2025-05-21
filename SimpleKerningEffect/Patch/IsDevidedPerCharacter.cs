using System.Collections.Immutable;
using YukkuriMovieMaker.Plugin.Effects;
using SimpleKerningEffect.Effects;

namespace SimpleKerningEffect.Patch
{
    public static class IsDevidedPerCharacter
    {
        public static bool GetDevided(bool original, ImmutableList<IVideoEffect> effects)
        {
            foreach (var effect in effects)
            {
                if ((effect is Effects.SimpleKerningEffect ||
                     effect is TextSpaceEffect ||
                     effect is TextSpaceBesideEffect ||
                     effect is TextSpaceVerticalEffect) &&
                     effect.IsEnabled)
                {
                    return true;
                }
            }

            return original;
        }
    }
}