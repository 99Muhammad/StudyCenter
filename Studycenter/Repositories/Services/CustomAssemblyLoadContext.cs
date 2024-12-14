using System;
using System.Reflection;
using System.Runtime.Loader;

namespace SCMS_back_end.Repositories.Services
{

    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return base.LoadUnmanagedDllFromPath(unmanagedDllName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // We are not loading any managed assemblies here.
            return null;
        }
    }

}
