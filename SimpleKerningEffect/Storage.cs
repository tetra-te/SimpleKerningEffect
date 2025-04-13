using System.Collections.Concurrent;
using YukkuriMovieMaker.Player.Video;

namespace SimpleKerningEffect
{
    public static class Storage
    {
        public static ConcurrentDictionary<(Guid, int), string> Texts = new();

        public static string GetText(EffectDescription effectDescription)
        {
            return Texts.GetValueOrDefault((effectDescription.SceneId, effectDescription.Layer), "");
        }
    }
}
