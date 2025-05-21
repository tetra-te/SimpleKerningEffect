using HarmonyLib;
using YukkuriMovieMaker.Plugin;

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
            var textPostfix = typeof(TextPostfix).GetMethod(nameof(TextPostfix.Postfix));
            var jimakuPostfix = typeof(JimakuPostfix).GetMethod(nameof(JimakuPostfix.Postfix));
            var textTranspiler = typeof(TextTranspiler).GetMethod(nameof(TextTranspiler.Transpiler));
            var jimakuTranspiler = typeof(JimakuTranspiler).GetMethod(nameof(JimakuTranspiler.Transpiler));

            harmony.Patch(textOriginal, new HarmonyMethod(textPostfix));
            harmony.Patch(jimakuOriginal, new HarmonyMethod(jimakuPostfix));
            harmony.Patch(textOriginal, transpiler: new HarmonyMethod(textTranspiler));
            harmony.Patch(jimakuOriginal, transpiler: new HarmonyMethod(jimakuTranspiler));
        }
    }
}