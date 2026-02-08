using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace SolidWorksExportAddin.Resources
{
    /// <summary>Zasoby (ikona) dla Custom Panel w Task Pane. Plik: Resources\CustomPanel.ico (16x16 lub 24x24).</summary>
    public static class Res
    {
        private const string EmbeddedIconName = "SolidWorksExportAddin.Resources.CustomPanel.ico";
        private const string IconFileName = "CustomPanel.ico";

        /// <summary>Ikona zakładki Custom Panel. Najpierw z wbudowanego zasobu, potem z pliku przy DLL.</summary>
        public static Icon TaskPaneIcon
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
                        try
                        {
                            return new Icon(stream);
                        }
                        finally
                        {
                            stream.Dispose();
                        }
                    }
                    // 2) Fallback: plik obok DLL (Resources\CustomPanel.ico)
                    var baseDir = Path.GetDirectoryName(asm.Location);
                    if (!string.IsNullOrEmpty(baseDir))
                    {
                        var iconPath = Path.Combine(baseDir, "Resources", IconFileName);
                        if (File.Exists(iconPath))
                            return new Icon(iconPath);
                    }
                }
                catch
                {
                    // ignoruj – panel załaduje się bez ikony
                }
                return null;
            }
        }
    }
}
