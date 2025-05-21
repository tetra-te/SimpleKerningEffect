using System.Collections.Concurrent;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project.Items;

namespace SimpleKerningEffect.Patch
{
    public static class Storage
    {
        public static ConcurrentDictionary<(Guid, int), string> Texts = new();
        public static ConcurrentDictionary<(Guid, int), BasePoint> BasePoints = new();

        public static string GetText(EffectDescription effectDescription)
        {
            return Texts.GetValueOrDefault((effectDescription.SceneId, effectDescription.Layer), "");
        }

        public static BasePoint GetBasePoint(EffectDescription effectDescription)
        {
            return BasePoints.GetValueOrDefault((effectDescription.SceneId, effectDescription.Layer), BasePoint.LeftTop);
        }
    }
}
