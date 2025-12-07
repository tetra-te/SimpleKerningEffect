using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SimpleKerningEffect.Patch.Transpiler
{
    public static class TextRewriter
    {        
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // 共通
            var fieldItem = AccessTools.Field("YukkuriMovieMaker.Player.Video.Items.TextSource:item");
            var getterVideoEffects = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.TextItem:VideoEffects");

            // 文字ごとに分割用
            var setDevided = typeof(SetValue).GetMethod(nameof(SetValue.SetDevided));
            var getterIsDevidedPerCharacter = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.TextItem:IsDevidedPerCharacter") ??
                                              AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.TextItem:IsDividedPerCharacter");

            // 行の高さ用
            var setLineHeight = typeof(SetValue).GetMethod(nameof(SetValue.SetLineHeight));
            var getterLineHeight = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.TextItem:LineHeight2");
            var methodGetValue = AccessTools.Method("YukkuriMovieMaker.Commons.Animation:GetValue");
            var lineHeightGetterFound = false;


            foreach (var code in instructions)
            {
                yield return code;

                if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo method1 && method1 == getterIsDevidedPerCharacter)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, fieldItem);
                    yield return new CodeInstruction(OpCodes.Callvirt, getterVideoEffects);
                    yield return new CodeInstruction(OpCodes.Call, setDevided);
                }

                if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo method2 && method2 == getterLineHeight)
                {
                    lineHeightGetterFound = true;
                }

                if (lineHeightGetterFound && code.opcode == OpCodes.Callvirt && code.operand is MethodInfo method3 && method3 == methodGetValue)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, fieldItem);
                    yield return new CodeInstruction(OpCodes.Callvirt, getterVideoEffects);
                    yield return new CodeInstruction(OpCodes.Call, setLineHeight);

                    lineHeightGetterFound = false;
                }
            }
        }
    }
}