using System.IO;
using System.Reflection;
using Xarial.XCad.UI;

namespace SolidWorksExportAddin.Resources
{
    /// <summary>Zasoby (ikona) dla Custom Panel w Task Pane. Plik: Resources\CustomPanel.ico (16x16 lub 24x24).</summary>
    public static class Res
    {
        private const string EmbeddedIconName = "SolidWorksExportAddin.Resources.CustomPanel.ico";
        private const string IconFileName = "CustomPanel.ico";

        /// <summary>Ikona zakładki Custom Panel (IXImage – bufor bajtów .ico).</summary>
        public static IXImage TaskPaneIcon
        {
            get
            {
                try
                {
                    var asm = Assembly.GetExecutingAssembly();

                    // 1) Wbudowany zasób
                    var stream = asm.GetManifestResourceStream(EmbeddedIconName);
                    if (stream != null && stream.Length > 0)
                    {
                        using (stream)
                        using (var ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            return new BaseImage(ms.ToArray());
                        }
                    }

                    // 2) Fallback: plik obok DLL (Resources\CustomPanel.ico)
                    var baseDir = Path.GetDirectoryName(asm.Location);
                    if (!string.IsNullOrEmpty(baseDir))
                    {
                        var iconPath = Path.Combine(baseDir, "Resources", IconFileName);
                        if (File.Exists(iconPath))
                            return new BaseImage(File.ReadAllBytes(iconPath));
                    }
                }
                catch
                {
                    // panel załaduje się bez ikony
                }
                return null;
            }
        }
    }
}
