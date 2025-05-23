using System.Collections.Immutable;
using YukkuriMovieMaker.Plugin.Effects;
using SimpleKerningEffect.Effects;
using YukkuriMovieMaker.Commons;
using HarmonyLib;

namespace SimpleKerningEffect.Patch
{
    public static class SetValue
    {
        public static bool SetDevided(bool original, ImmutableList<IVideoEffect> effects)
        {
            foreach (var effect in effects)
            {
                if ((effect is Effects.SimpleKerningEffect ||
                     effect is TextLineStepEffect ||
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

        public static double SetLineHeight(double original, ImmutableList<IVideoEffect> effects)
        {
            foreach (var effect in effects)
            {
                if (effect is TextLineStepEffect && effect.IsEnabled)
                {
                    return 0d;
                }
            }

            return original;
        }
    }
}