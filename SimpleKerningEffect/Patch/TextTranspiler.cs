using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace SimpleKerningEffect.Patch
{
    public static class TextTranspiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var getterIsDevidedPerCharacter = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.TextItem:IsDevidedPerCharacter");
            var fieldItem = AccessTools.Field("YukkuriMovieMaker.Player.Video.Items.TextSource:item");
            var getterVideoEffects = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.TextItem:VideoEffects");
            var isDevided = typeof(IsDevidedPerCharacter).GetMethod(nameof(IsDevidedPerCharacter.GetDevided));

            foreach (var code in instructions)
            {
                yield return code;

                if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo method && method == getterIsDevidedPerCharacter)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, fieldItem);
                    yield return new CodeInstruction(OpCodes.Callvirt, getterVideoEffects);
                    yield return new CodeInstruction(OpCodes.Call, isDevided);
                }
            }
        }
    }
}