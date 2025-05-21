using HarmonyLib;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project.Items;

namespace SimpleKerningEffect.Patch
{
    public class PatchJimaku
    {     
        public static void Postfix(TimelineItemSourceDescription desc, object __instance)
        {
            var jimakuSourceType = __instance.GetType();
            var itemField = AccessTools.Field(jimakuSourceType, "item");
            var item = itemField.GetValue(__instance);

            var voiceItemType = item.GetType();
            var serifProp = AccessTools.Property(voiceItemType, "Serif");
            var serif = serifProp.GetValue(item);
            var basePointProp = AccessTools.Property(voiceItemType, "BasePoint");
            var basePoint = basePointProp.GetValue(item);

            Storage.Texts[(desc.SceneId, desc.Layer)] = (string)serif;
            Storage.BasePoints[(desc.SceneId, desc.Layer)] = (BasePoint)basePoint;
        }
    }
}
