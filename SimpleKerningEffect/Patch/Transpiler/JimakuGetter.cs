using System.Reflection.Emit;

namespace SimpleKerningEffect.Patch.Transpiler
{
    public class JimakuGetter
    {     
        public static IEnumerable<object> Transpiler(IEnumerable<object> instructions)
        {
            var item = HRef.AccessToolsField.Invoke(null, ["YukkuriMovieMaker.Player.Video.Items.JimakuSource:item"]);

            var text = HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.VoiceItem:Serif"]);
            var basePoint = HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.VoiceItem:BasePoint"]);
            var volume = HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.VoiceItem:Volume"]);
            var keyFrames = HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Commons.Animation:KeyFrames"]);
            
            var setText = typeof(Storage).GetMethod(nameof(Storage.SetText));
            var setBasePoint = typeof(Storage).GetMethod(nameof(Storage.SetBasePoint));
            var setKeyFrames = typeof(Storage).GetMethod(nameof(Storage.SetKeyFrames));
            
            foreach (var code in instructions)
            {
                if ((OpCode)HRef.opcode.GetValue(code)! == OpCodes.Ret)
                {
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_1, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_0, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldfld, item])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, text])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Call, setText])!;

                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_1, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_0, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldfld, item])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, basePoint])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Call, setBasePoint])!;

                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_1, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_0, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldfld, item])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, volume])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, keyFrames])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Call, setKeyFrames])!;
                }

                yield return code;
            }
        }
    }
}