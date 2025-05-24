using System.Collections.Concurrent;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project.Items;

namespace SimpleKerningEffect.Patch
{
    public static class Storage
    {
        public static ConcurrentDictionary<(Guid, int), string> Texts = new();

        public static ConcurrentDictionary<(Guid, int), BasePoint> BasePoints = new();

        public static ConcurrentDictionary<(Guid, int), ImmutableList<int>> KeyFrames = new();

        public static void SetText(TimelineItemSourceDescription desc, string text)
        {
            Texts[(desc.SceneId, desc.Layer)] = text;
        }
        
        public static string GetText(EffectDescription desc)
        {
            return Texts.GetValueOrDefault((desc.SceneId, desc.Layer), "");
        }

        public static void SetBasePoint(TimelineItemSourceDescription desc, BasePoint basePoint)
        {
            BasePoints[(desc.SceneId, desc.Layer)] = basePoint;
        }

        public static BasePoint GetBasePoint(EffectDescription desc)
        {
            return BasePoints.GetValueOrDefault((desc.SceneId, desc.Layer), BasePoint.LeftTop);
        }

        public static void SetKeyFrames(TimelineItemSourceDescription desc, KeyFrames? keyFrames)
        {
            if (keyFrames is null) return;
            KeyFrames[(desc.SceneId, desc.Layer)] = keyFrames.Frames;
        }

        public static ImmutableList<int> GetKeyFrames(EffectDescription desc)
        {
            return KeyFrames.GetValueOrDefault((desc.SceneId, desc.Layer), ImmutableList<int>.Empty);
        }
    }
}