using System.Collections.Concurrent;

namespace SimpleKerningEffect
{
    public static class Storage
    {
        public static ConcurrentDictionary<(Guid, int), string> Texts = new();
    }
}
