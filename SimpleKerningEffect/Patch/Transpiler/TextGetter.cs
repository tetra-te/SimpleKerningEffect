using System.Reflection.Emit;

namespace SimpleKerningEffect.Patch.Transpiler
{
    public class TextGetter
    {     
        public static IEnumerable<object> Transpiler(IEnumerable<object> instructions)
        {
            var item = HRef.AccessToolsField.Invoke(null, ["YukkuriMovieMaker.Player.Video.Items.TextSource:item"]);

            var text = HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.TextItem:Text"]);
            var basePoint = HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.TextItem:BasePoint"]);
            var x = HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.TextItem:X"]);
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
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, x])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, keyFrames])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Call, setKeyFrames])!;
                }

                yield return code;
            }
        }
    }
}