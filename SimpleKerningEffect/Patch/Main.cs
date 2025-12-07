using System.IO;
using System.Windows;
using SimpleKerningEffect.Patch.Transpiler;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin;

namespace SimpleKerningEffect.Patch
{
    public class Main : IPlugin
    {
        public string Name => "簡易カーニング（パッチ）";

        public Main()
        {
            var ymm4PluginDirectory = AppDirectories.PluginDirectory;

            var harmonyFolder = Path.Combine(ymm4PluginDirectory, "Harmony");
            var kerningFolder = Path.Combine(ymm4PluginDirectory, "SimpleKerningEffect");
            var ratioOutlineFolder = Path.Combine(ymm4PluginDirectory, "RatioOutlineEffect");
            var videoOutputMessageFolder = Path.Combine(ymm4PluginDirectory, "VideoOutputMessage");

            var harmonyYmmeDllName = "lib.har.ymmelib";

            // 0Harmony.ymmedllのパス候補
            var ratioOutlineHarmonyYmmeDll = Path.Combine(ratioOutlineFolder, harmonyYmmeDllName);
            var videoOutputMessageHarmonyYmmeDll = Path.Combine(videoOutputMessageFolder, harmonyYmmeDllName);

            var harmonyDllName = "0Harmony.dll";

            // 0Harmony.dllのパス候補
            var harmonyDll = Path.Combine(harmonyFolder, harmonyDllName);
            var kerningHarmonyDll = Path.Combine(kerningFolder, harmonyDllName);
            var ratioOutlineHarmonyDll = Path.Combine(ratioOutlineFolder, harmonyDllName);
            var videoOutputMessageHarmonyDll = Path.Combine(videoOutputMessageFolder, harmonyDllName);

            List<string> pluginToUpdate = [];

            if (File.Exists(ratioOutlineHarmonyDll) && (!File.Exists(ratioOutlineHarmonyYmmeDll)))
                pluginToUpdate.Add("比率縁取り");
            if (File.Exists(videoOutputMessageHarmonyDll) && (!File.Exists(videoOutputMessageHarmonyYmmeDll)))
                pluginToUpdate.Add("動画出力メッセージ");

            List<string> fileToDelte = [];

            if (File.Exists(harmonyDll))
                fileToDelte.Add(harmonyDll);
            if (File.Exists(kerningHarmonyDll))
                fileToDelte.Add(kerningHarmonyDll);
            if (File.Exists(ratioOutlineHarmonyDll))
                fileToDelte.Add(ratioOutlineHarmonyDll);
            if (File.Exists(videoOutputMessageHarmonyDll))
                fileToDelte.Add(videoOutputMessageHarmonyDll);

            var message = "";

            for (int i = 0; i < pluginToUpdate.Count; i++)
            {
                if (i == 0)
                    message += "以下のプラグインはアップデートが必要です。\r\n最新版をダウンロードしてインストールしてください。\r\n";

                message += "・" + pluginToUpdate[i] + "\r\n";

                if (i == pluginToUpdate.Count - 1)
                    message += "\r\n";
            }

            for (int i = 0; i < fileToDelte.Count; i++)
            {
                if (i == 0)
                    message += "YMM4を終了して以下のファイルを削除してください。\r\n削除しないとYMM4が起動しなくなることがあります。\r\n";

                message += "・" + fileToDelte[i] + "\r\n";
            }

            if (message != "")
                MessageBox.Show(message, "簡易カーニングプラグイン");


            var harmony = Activator.CreateInstance(HRef.Harmony, ["SimpleKerningEffect"]);

            var textOriginal = HRef.AccessToolsMethod.Invoke(null, ["YukkuriMovieMaker.Player.Video.Items.TextSource:UpdateResource", null, null]);
            var jimakuOriginal = HRef.AccessToolsMethod.Invoke(null, ["YukkuriMovieMaker.Player.Video.Items.JimakuSource:UpdateResource", null, null]);

            var textGetter = typeof(TextGetter).GetMethod(nameof(TextGetter.Transpiler));
            var textGetterH = Activator.CreateInstance(HRef.HarmonyMethod, [textGetter]);

            var jimakuGetter = typeof(JimakuGetter).GetMethod(nameof(JimakuGetter.Transpiler));
            var jimakuGetterH = Activator.CreateInstance(HRef.HarmonyMethod, [jimakuGetter]);

            var textRewriter = typeof(TextRewriter).GetMethod(nameof(TextRewriter.Transpiler));
            var textRewriterH = Activator.CreateInstance(HRef.HarmonyMethod, [textRewriter]);

            var jimakuRewriter = typeof(JimakuRewriter).GetMethod(nameof(JimakuRewriter.Transpiler));
            var jimakuRewriterH = Activator.CreateInstance(HRef.HarmonyMethod, [jimakuRewriter]);

            HRef.HarmonyPatch.Invoke(harmony, [textOriginal, null, null, textGetterH, null]);
            HRef.HarmonyPatch.Invoke(harmony, [jimakuOriginal, null, null, jimakuGetterH, null]);
            HRef.HarmonyPatch.Invoke(harmony, [textOriginal, null, null, textRewriterH, null]);
            HRef.HarmonyPatch.Invoke(harmony, [jimakuOriginal, null, null, jimakuRewriterH, null]);
        }
    }
}