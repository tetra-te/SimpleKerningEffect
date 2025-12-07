using System.Reflection;
using System.Reflection.Emit;

namespace SimpleKerningEffect.Patch.Transpiler
{
    public static class JimakuRewriter
    {
        public static IEnumerable<object> Transpiler(IEnumerable<object> instructions)
        {
            // 共通
            var fieldItem = HRef.AccessToolsField.Invoke(null, ["YukkuriMovieMaker.Player.Video.Items.JimakuSource:item"]);
            var characterGetterCharacter = HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.VoiceItem:Character"]);
            var characterGetterVideoEffects = HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Character:JimakuVideoEffects"]);
            var itemGetterVideoEffects = HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.VoiceItem:JimakuVideoEffects"]);
            
            // 文字ごとに分割用
            var characterGetterIsDevided = (MethodInfo?)HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Character:IsDevidedPerCharacter"]) ??
                                           (MethodInfo?)HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Character:IsDividedPerCharacter"]);

            var itemGetterIsDevided = (MethodInfo?)HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.VoiceItem:IsDevidedPerCharacter"]) ??
                                      (MethodInfo?)HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.VoiceItem:IsDividedPerCharacter"]);
            
            var setDevided = typeof(SetValue).GetMethod(nameof(SetValue.SetDevided));

            // 行の高さ用
            var setLineHeight = typeof(SetValue).GetMethod(nameof(SetValue.SetLineHeight));
            var methodGetValue = (MethodInfo?)HRef.AccessToolsMethod.Invoke(null, ["YukkuriMovieMaker.Commons.Animation:GetValue", null, null]);
            var characterGetterLineHeight = (MethodInfo?)HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Character:LineHeight2"]);
            var itemGetterLineHeight = (MethodInfo?)HRef.AccessToolsPropertyGetter.Invoke(null, ["YukkuriMovieMaker.Project.Items.VoiceItem:LineHeight2"]);
            var characterGetterLineHeightFound = false;
            var itemGetterLineHeightFound = false;
            

            foreach (var code in instructions)
            {
                yield return code;

                var opcode = (OpCode)HRef.opcode.GetValue(code)!;
                var operand = HRef.operand.GetValue(code);

                if (opcode == OpCodes.Callvirt && operand is MethodInfo method1 && method1 == characterGetterIsDevided)
                {
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_0, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldfld, fieldItem])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, characterGetterCharacter])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, characterGetterVideoEffects])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Call, setDevided])!;
                }

                if (opcode == OpCodes.Callvirt && operand is MethodInfo method2 && method2 == itemGetterIsDevided)
                {
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_0, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldfld, fieldItem])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, itemGetterVideoEffects])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Call, setDevided])!;
                }          

                if (opcode == OpCodes.Callvirt && operand is MethodInfo method3 && method3 == characterGetterLineHeight)
                {
                    characterGetterLineHeightFound = true;
                }

                if (characterGetterLineHeightFound && opcode == OpCodes.Callvirt && operand is MethodInfo method4 && method4 == methodGetValue)
                {
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_0, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldfld, fieldItem])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, characterGetterCharacter])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, characterGetterVideoEffects])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Call, setLineHeight])!;

                    characterGetterLineHeightFound = false;
                }

                if (opcode == OpCodes.Callvirt && operand is MethodInfo method5 && method5 == itemGetterLineHeight)
                {
                    itemGetterLineHeightFound = true;
                }

                if (itemGetterLineHeightFound && opcode == OpCodes.Callvirt && operand is MethodInfo method6 && method6 == methodGetValue)
                {
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldarg_0, null])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Ldfld, fieldItem])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Callvirt, itemGetterVideoEffects])!;
                    yield return Activator.CreateInstance(HRef.CodeInstruction, [OpCodes.Call, setLineHeight])!;

                    itemGetterLineHeightFound = false;
                }
            }
        }
    }
}