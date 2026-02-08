using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace RegisterAddin
{
    /// <summary>
    /// Rejestruje/wyrejestrowuje add-in SolidWorks. Najpierw rejestruje AssemblyResolve,
    /// żeby ładować SolidWorks.Interop.* z instalacji SW, potem wywołuje RegistrationServices.
    /// Uruchamiać z folderu zawierającego SolidWorksExportAddin.dll (jako administrator).
    /// </summary>
    internal static class Program
    {
        private const string AddinAssemblyName = "SolidWorksExportAddin.dll";
        private const string RedistSubPath = @"api\redist";
        private static readonly string DefaultInstallPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "SOLIDWORKS Corp", "SOLIDWORKS");

        static int Main(string[] args)
        {
            bool unregister = args != null && args.Length > 0 &&
                (string.Equals(args[0], "/u", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(args[0], "-u", StringComparison.OrdinalIgnoreCase));

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dllPath = Path.Combine(baseDir, AddinAssemblyName);
            if (!File.Exists(dllPath))
            {
                Console.Error.WriteLine("Nie znaleziono: " + dllPath);
                return 1;
            }

            // Najpierw rejestracja ładowania interopów z instalacji SolidWorks
            AppDomain.CurrentDomain.AssemblyResolve += ResolveSolidWorksInterop;

            try
            {
                Assembly asm = Assembly.LoadFrom(dllPath);
                var reg = new RegistrationServices();
                if (unregister)
                {
                    reg.UnregisterAssembly(asm);
                    Console.WriteLine("Wyrejestrowano add-in.");
                }
                else
                {
                    reg.RegisterAssembly(asm, AssemblyRegistrationFlags.SetCodeBase);
                    Console.WriteLine("Zarejestrowano add-in.");
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }

        private static Assembly ResolveSolidWorksInterop(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            if (!name.Name.StartsWith("SolidWorks.Interop.", StringComparison.OrdinalIgnoreCase))
                return null;

            string redistPath = GetSolidWorksRedistPath();
            if (string.IsNullOrEmpty(redistPath))
                return null;

            string dllPath = Path.Combine(redistPath, name.Name + ".dll");
            if (!File.Exists(dllPath))
                return null;

            return Assembly.LoadFrom(dllPath);
        }

        private static string GetSolidWorksRedistPath()
        {
            string defaultRedist = Path.Combine(DefaultInstallPath, RedistSubPath);
            if (Directory.Exists(defaultRedist))
                return defaultRedist;

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\SolidWorks\Applications\SolidWorks"))
                {
                    if (key != null)
                    {
                        foreach (string subKeyName in key.GetSubKeyNames())
                        {
                            using (var versionKey = key.OpenSubKey(subKeyName))
                            {
                                string installDir = versionKey?.GetValue("InstallDir") as string;
                                if (!string.IsNullOrEmpty(installDir))
                                {
                                    string redist = Path.Combine(installDir, RedistSubPath);
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
}
