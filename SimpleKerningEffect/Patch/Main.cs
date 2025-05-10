using HarmonyLib;
using YukkuriMovieMaker.Plugin;

namespace SimpleKerningEffect.Patch
{
    public class Main : IPlugin
    {
        public string Name => "簡易カーニング（テキスト情報取得パッチ）";

        public Main()
        {
            var harmony = new Harmony("SimpleKerningEffect.TextInfoAcquisitionPatch");

            var textOriginal = AccessTools.Method("YukkuriMovieMaker.Player.Video.Items.TextSource:UpdateResource");
            var jimakuOriginal = AccessTools.Method("YukkuriMovieMaker.Player.Video.Items.JimakuSource:UpdateResource");
            var textPostfix = typeof(PatchText).GetMethod(nameof(PatchText.Postfix));
            var jimakuPostfix = typeof(PatchJimaku).GetMethod(nameof(PatchJimaku.Postfix));

            harmony.Patch(textOriginal, new HarmonyMethod(textPostfix));
            harmony.Patch(jimakuOriginal, new HarmonyMethod(jimakuPostfix));
        }
    }
}
