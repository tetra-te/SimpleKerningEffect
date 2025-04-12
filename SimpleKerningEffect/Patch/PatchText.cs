using HarmonyLib;
using YukkuriMovieMaker.Player.Video;

namespace SimpleKerningEffect.Patch
{
    public class PatchText
    {     
        public static void Postfix(TimelineItemSourceDescription desc, object __instance)
        {
            var textSourceType = __instance.GetType();
            var itemField = AccessTools.Field(textSourceType, "item");
            var item = itemField.GetValue(__instance);

            var textItemType = item.GetType();
            var textProp = AccessTools.Property(textItemType, "Text");
            var text = textProp.GetValue(item);

            Storage.Texts[(desc.SceneId, desc.Layer)] = (string)text;
        }
    }
}
