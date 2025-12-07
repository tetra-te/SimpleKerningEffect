using System.Reflection;
using System.Reflection.Emit;

namespace SimpleKerningEffect.Patch.Transpiler
{
    public static class TextRewriter
    {        
        public static IEnumerable<object> Transpiler(IEnumerable<object> instructions)
        {
            // 共通
            var fieldItem = HRef.AccessToolsField.Invoke(null, ["YukkuriMovieMaker.Player.Video.Items.TextSource:item"]);
            var getterVideoEffects = HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.TextItem:VideoEffects"]);

            // 文字ごとに分割用
            var setDevided = typeof(SetValue).GetMethod(nameof(SetValue.SetDevided));
            var getterIsDevidedPerCharacter = (MethodInfo?)HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.TextItem:IsDevidedPerCharacter"]);

            // 行の高さ用
            var setLineHeight = typeof(SetValue).GetMethod(nameof(SetValue.SetLineHeight));
            var getterLineHeight = (MethodInfo?)HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.TextItem:LineHeight2"]);
            var methodGetValue = (MethodInfo?)HRef.AccessToolsMethod.Invoke(null, ["YukkuriMovieMaker.Commons.Animation:GetValue", null, null]);
            var lineHeightGetterFound = false;


            foreach (var code in instructions)
            {
                yield return code;

                var opcode = (OpCode)HRef.opcode.GetValue(code)!;
                var operand = HRef.operand.GetValue(code);

                if (opcode == OpCodes.Callvirt && operand is MethodInfo method1 && method1 == getterIsDevidedPerCharacter)
                {
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_0, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldfld, fieldItem])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, getterVideoEffects])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Call, setDevided])!;
                }

                if (opcode == OpCodes.Callvirt && operand is MethodInfo method2 && method2 == getterLineHeight)
                {
                    lineHeightGetterFound = true;
                }

                if (lineHeightGetterFound && opcode == OpCodes.Callvirt && operand is MethodInfo method3 && method3 == methodGetValue)
                {
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_0, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldfld, fieldItem])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, getterVideoEffects])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Call, setLineHeight])!;

                    lineHeightGetterFound = false;
                }
            }
        }
    }
}