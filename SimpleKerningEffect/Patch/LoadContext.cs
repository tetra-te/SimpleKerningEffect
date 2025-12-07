using System.Reflection;
using System.Runtime.Loader;

namespace SimpleKerningEffect.Patch
{
    internal class LoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver resolver;

        public LoadContext(string path) : base(isCollectible: false)
        {
            resolver = new AssemblyDependencyResolver(path);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var path = resolver.ResolveAssemblyToPath(assemblyName);
            if (path is null)
            {
                return null;
            }
            else
            {
                return LoadFromAssemblyPath(path);
            }
        }
    }
}
