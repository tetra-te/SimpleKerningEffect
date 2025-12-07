using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SimpleKerningEffect.Patch.Transpiler
{
    public static class JimakuRewriter
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // 共通
            var fieldItem = AccessTools.Field("YukkuriMovieMaker.Player.Video.Items.JimakuSource:item");
            var characterGetterCharacter = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.VoiceItem:Character");
            var characterGetterVideoEffects = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Character:JimakuVideoEffects");
            var itemGetterVideoEffects = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.VoiceItem:JimakuVideoEffects");
            
            // 文字ごとに分割用
            var characterGetterIsDevided = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Character:IsDevidedPerCharacter") ??
                                           AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Character:IsDividedPerCharacter");

            var itemGetterIsDevided = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.VoiceItem:IsDevidedPerCharacter") ?? 
                                      AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.VoiceItem:IsDividedPerCharacter");
            
            var setDevided = typeof(SetValue).GetMethod(nameof(SetValue.SetDevided));

            // 行の高さ用
            var setLineHeight = typeof(SetValue).GetMethod(nameof(SetValue.SetLineHeight));
            var methodGetValue = AccessTools.Method("YukkuriMovieMaker.Commons.Animation:GetValue");
            var characterGetterLineHeight = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Character:LineHeight2");
            var itemGetterLineHeight = AccessTools.PropertyGetter("YukkuriMovieMaker.Project.Items.VoiceItem:LineHeight2");
            var characterGetterLineHeightFound = false;
            var itemGetterLineHeightFound = false;
            

            foreach (var code in instructions)
            {
                yield return code;

                if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo method1 && method1 == characterGetterIsDevided)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, fieldItem);
                    yield return new CodeInstruction(OpCodes.Callvirt, characterGetterCharacter);
                    yield return new CodeInstruction(OpCodes.Callvirt, characterGetterVideoEffects);
                    yield return new CodeInstruction(OpCodes.Call, setDevided);
                }

                if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo method2 && method2 == itemGetterIsDevided)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, fieldItem);
                    yield return new CodeInstruction(OpCodes.Callvirt, itemGetterVideoEffects);
                    yield return new CodeInstruction(OpCodes.Call, setDevided);
                }          

                if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo method3 && method3 == characterGetterLineHeight)
                {
                    characterGetterLineHeightFound = true;
                }

                if (characterGetterLineHeightFound && code.opcode == OpCodes.Callvirt && code.operand is MethodInfo method4 && method4 == methodGetValue)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, fieldItem);
                    yield return new CodeInstruction(OpCodes.Callvirt, characterGetterCharacter);
                    yield return new CodeInstruction(OpCodes.Callvirt, characterGetterVideoEffects);
                    yield return new CodeInstruction(OpCodes.Call, setLineHeight);

                    characterGetterLineHeightFound = false;
                }

                if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo method5 && method5 == itemGetterLineHeight)
                {
                    itemGetterLineHeightFound = true;
                }

                if (itemGetterLineHeightFound && code.opcode == OpCodes.Callvirt && code.operand is MethodInfo method6 && method6 == methodGetValue)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, fieldItem);
                    yield return new CodeInstruction(OpCodes.Callvirt, itemGetterVideoEffects);
                    yield return new CodeInstruction(OpCodes.Call, setLineHeight);

                    itemGetterLineHeightFound = false;
                }
            }
        }
    }
}