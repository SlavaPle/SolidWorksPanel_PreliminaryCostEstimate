using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace SolidWorksExportAddin
{
    /// <summary>
    /// Ładuje zestawy SolidWorks.Interop.* z katalogu instalacji SolidWorks,
    /// żeby nie kopiować ich do outputu i nie zależeć od wersji.
    /// </summary>
    internal static class SolidWorksInteropLoader
    {
        private const string RedistSubPath = @"api\redist";
        private static readonly string DefaultInstallPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "SOLIDWORKS Corp", "SOLIDWORKS");

        private static bool s_registered;

        /// <summary>Wywołaj przed pierwszym odwołaniem do typów z SolidWorks.Interop (np. z atrybutu).</summary>
        internal static void EnsureRegistered()
        {
            if (s_registered) return;
            s_registered = true;
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveSolidWorksInterop;
        }

        private static Assembly OnResolveSolidWorksInterop(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            if (!name.Name.StartsWith("SolidWorks.Interop.", StringComparison.OrdinalIgnoreCase))
                return null;

            var redistPath = GetSolidWorksRedistPath();
            if (string.IsNullOrEmpty(redistPath))
                return null;

            var dllPath = Path.Combine(redistPath, name.Name + ".dll");
            if (!File.Exists(dllPath))
                return null;

            return Assembly.LoadFrom(dllPath);
        }

        private static string GetSolidWorksRedistPath()
        {
            var defaultRedist = Path.Combine(DefaultInstallPath, RedistSubPath);
            if (Directory.Exists(defaultRedist))
                return defaultRedist;

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\SolidWorks\Applications\SolidWorks"))
                {
                    if (key != null)
                    {
                        foreach (var subKeyName in key.GetSubKeyNames())
                        {
                            using (var versionKey = key.OpenSubKey(subKeyName))
                            {
                                var installDir = versionKey?.GetValue("InstallDir") as string;
                                if (!string.IsNullOrEmpty(installDir))
                                {
                                    var redist = Path.Combine(installDir, RedistSubPath);
                                    if (Directory.Exists(redist))
                                        return redist;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return null;
        }
    }

    /// <summary>
    /// Atrybut na poziomie zestawu — przy odczycie atrybutów (np. przez RegAsm) rejestruje ładowanie interopów z instalacji SW.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    internal sealed class SolidWorksInteropResolverAttribute : Attribute
    {
        public SolidWorksInteropResolverAttribute()
        {
            SolidWorksInteropLoader.EnsureRegistered();
        }
    }
}
