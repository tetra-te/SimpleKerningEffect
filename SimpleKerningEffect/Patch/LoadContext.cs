using System.Reflection;
using System.Runtime.Loader;

namespace SimpleKerningEffect.Patch
{
    internal class LoadContext : AssemblyLoadContext
    {
        public LoadContext() : base(isCollectible: false)
        {
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
