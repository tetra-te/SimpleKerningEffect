using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using YukkuriMovieMaker.Commons;

namespace SimpleKerningEffect.Patch
{
    internal static class HRef
    {
        static string harmonyPath = Path.Combine(AppDirectories.PluginDirectory, "SimpleKerningEffect", "lib.har.ymmelib");

        static AssemblyLoadContext context = new LoadContext(harmonyPath);

        static Assembly assembly = context.LoadFromAssemblyPath(harmonyPath);

        public static Type Harmony = assembly.GetType("HarmonyLib.Harmony")!;
        public static MethodInfo HarmonyPatch = Harmony.GetMethod("Patch", BindingFlags.Public | BindingFlags.Instance)!;

        public static Type HarmonyMethod = assembly.GetType("HarmonyLib.HarmonyMethod")!;

        public static Type AccessTools = assembly.GetType("HarmonyLib.AccessTools")!;
        public static MethodInfo AccessToolsMethod = AccessTools.GetMethod("Method", BindingFlags.Public | BindingFlags.Static, [typeof(string), typeof(Type[]), typeof(Type[])])!;
        public static MethodInfo AccessToolsField = AccessTools.GetMethod("Field", BindingFlags.Public | BindingFlags.Static, [typeof(string)])!;
        public static MethodInfo AccessToolsPropertyGetter = AccessTools.GetMethod("PropertyGetter", BindingFlags.Public | BindingFlags.Static, [typeof(string)])!;

        public static Type CodeInstruction = assembly.GetType("HarmonyLib.CodeInstruction")!;
        public static FieldInfo opcode = CodeInstruction.GetField("opcode")!;
        public static FieldInfo operand = CodeInstruction.GetField("operand")!;
    }
}
