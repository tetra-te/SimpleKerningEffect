using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project.Items;

namespace SimpleKerningEffect.Patch
{
    public static class Storage
    {
        readonly record struct ItemKey(Guid SceneId, TimelineSourceUsage Usage, int Layer, int StartFrame, int Length)
        {
            public static ItemKey From(TimelineItemSourceDescription d)
                => new(d.SceneId, d.Usage, d.Layer,
                       d.TimelinePosition.Frame - d.ItemPosition.Frame,  // アイテム開始フレーム
                       d.ItemDuration.Frame);
        }

        sealed class Slot
        {
            public ItemKey Key;
            public string Text = "";
            public BasePoint BasePoint = BasePoint.LeftTop;
            public ImmutableList<int> KeyFrames = ImmutableList<int>.Empty;
        }

        [ThreadStatic] static Slot? slot;

        static Slot Write(TimelineItemSourceDescription desc)
        {
            var s = slot ??= new Slot();
            s.Key = ItemKey.From(desc);
            return s;
        }

        static Slot? Read(EffectDescription desc)
            => slot is { } s && s.Key == ItemKey.From(desc) ? s : null;

        public static void SetText(TimelineItemSourceDescription desc, string text) => Write(desc).Text = text ?? "";
        public static void SetBasePoint(TimelineItemSourceDescription desc, BasePoint b) => Write(desc).BasePoint = b;
        public static void SetKeyFrames(TimelineItemSourceDescription desc, KeyFrames? k) { if (k is not null) Write(desc).KeyFrames = k.Frames; }

        public static string GetText(EffectDescription desc) => Read(desc)?.Text ?? "";
        public static BasePoint GetBasePoint(EffectDescription desc) => Read(desc)?.BasePoint ?? BasePoint.LeftTop;
        public static ImmutableList<int> GetKeyFrames(EffectDescription desc) => Read(desc)?.KeyFrames ?? ImmutableList<int>.Empty;
    }
}