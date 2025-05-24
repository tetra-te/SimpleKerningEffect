using YukkuriMovieMaker.Plugin;
using HarmonyLib;
using SimpleKerningEffect.Patch.Transpiler;

namespace SimpleKerningEffect.Patch
{
    public class Main : IPlugin
    {
        public string Name => "簡易カーニング（パッチ）";

        public Main()
        {
            var harmony = new Harmony("SimpleKerningEffect");

            var textOriginal = AccessTools.Method("YukkuriMovieMaker.Player.Video.Items.TextSource:UpdateResource");
            var jimakuOriginal = AccessTools.Method("YukkuriMovieMaker.Player.Video.Items.JimakuSource:UpdateResource");

            var textGetter = typeof(TextGetter).GetMethod(nameof(TextGetter.Transpiler));
            var jimakuGetter = typeof(JimakuGetter).GetMethod(nameof(JimakuGetter.Transpiler));
            var textRewriter = typeof(TextRewriter).GetMethod(nameof(TextRewriter.Transpiler));
            var jimakuRewriter = typeof(JimakuRewriter).GetMethod(nameof(JimakuRewriter.Transpiler));

            harmony.Patch(textOriginal, transpiler: new HarmonyMethod(textGetter));
            harmony.Patch(jimakuOriginal, transpiler: new HarmonyMethod(jimakuGetter));
            harmony.Patch(textOriginal, transpiler: new HarmonyMethod(textRewriter));
            harmony.Patch(jimakuOriginal, transpiler: new HarmonyMethod(jimakuRewriter));
        }
    }
}