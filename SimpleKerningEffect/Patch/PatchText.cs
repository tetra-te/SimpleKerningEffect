using HarmonyLib;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project.Items;

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
            var basePointProp = AccessTools.Property(textItemType, "BasePoint");
            var basePoint = basePointProp.GetValue(item);

            Storage.Texts[(desc.SceneId, desc.Layer)] = (string)text;
            Storage.BasePoints[(desc.SceneId, desc.Layer)] = (BasePoint)basePoint;
        }
    }
}
