using System.Reflection.Emit;
using HarmonyLib;

namespace SimpleKerningEffect.Patch.Transpiler
{
    public class JimakuGetter
    {     
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var item = AccessTools.Field("YukkuriMovieMaker.Player.Video.Items.JimakuSource:item");
            
            var text = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.VoiceItem:Serif");
            var basePoint = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.VoiceItem:BasePoint");
            var volume = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.VoiceItem:Volume");
            var keyFrames = AccessTools.PropertyGetter("YukkuriMovieMaker.Commons.Animation:KeyFrames");
            
            var setText = typeof(Storage).GetMethod(nameof(Storage.SetText));
            var setBasePoint = typeof(Storage).GetMethod(nameof(Storage.SetBasePoint));
            var setKeyFrames = typeof(Storage).GetMethod(nameof(Storage.SetKeyFrames));
            
            foreach (var code in instructions)
            {
                if (code.opcode == OpCodes.Ret)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, item);
                    yield return new CodeInstruction(OpCodes.Callvirt, text);
                    yield return new CodeInstruction(OpCodes.Call, setText);

                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, item);
                    yield return new CodeInstruction(OpCodes.Callvirt, basePoint);
                    yield return new CodeInstruction(OpCodes.Call, setBasePoint);

                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, item);
                    yield return new CodeInstruction(OpCodes.Callvirt, volume);
                    yield return new CodeInstruction(OpCodes.Callvirt, keyFrames);
                    yield return new CodeInstruction(OpCodes.Call, setKeyFrames);
                }

                yield return code;
            }
        }
    }
}