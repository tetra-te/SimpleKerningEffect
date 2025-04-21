using HarmonyLib;
using YukkuriMovieMaker.Plugin;

namespace SimpleKerningEffect.Patch
{
    public class Main : IPlugin
    {
        public string Name => "簡易カーニング（テキスト情報取得パッチ）";

        public Main()
        {
            var harmony = new Harmony("SimpleKerningEffect.TextAcquisitionPatch");

            var textOriginal = AccessTools.Method("YukkuriMovieMaker.Player.Video.Items.TextSource:UpdateResource");
            var textPostfix = typeof(PatchText).GetMethod(nameof(PatchText.Postfix));

            harmony.Patch(textOriginal, new HarmonyMethod(textPostfix));
        }
    }
}
