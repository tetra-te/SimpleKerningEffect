using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SimpleKerningEffect.Patch
{
    public static class JimakuTranspiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var fieldItem = AccessTools.Field("YukkuriMovieMaker.Player.Video.Items.JimakuSource:item");
            var getDevided = typeof(IsDevidedPerCharacter).GetMethod(nameof(IsDevidedPerCharacter.GetDevided));

            var itemGetterIsDevided = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.VoiceItem:IsDevidedPerCharacter");
            var itemGetterVideoEffects = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.VoiceItem:JimakuVideoEffects");

            var characterGetterIsDevided = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Character:IsDevidedPerCharacter");
            var characterGetterCharacter = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.VoiceItem:Character");
            var characterGetterVideoEffects = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Character:JimakuVideoEffects");

            foreach (var code in instructions)
            {
                yield return code;

                if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo methodItem && methodItem == itemGetterIsDevided)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, fieldItem);
                    yield return new CodeInstruction(OpCodes.Callvirt, itemGetterVideoEffects);
                    yield return new CodeInstruction(OpCodes.Call, getDevided);
                }
                if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo methodCharacter && methodCharacter == characterGetterIsDevided)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, fieldItem);
                    yield return new CodeInstruction(OpCodes.Callvirt, characterGetterCharacter);
                    yield return new CodeInstruction(OpCodes.Callvirt, characterGetterVideoEffects);
                    yield return new CodeInstruction(OpCodes.Call, getDevided);
                }
            }
        }
    }
}