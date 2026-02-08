using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Xarial.XCad.Base.Attributes;
using Xarial.XCad.SolidWorks;
using Xarial.XCad.SolidWorks.UI;

namespace SolidWorksExportAddin
{
    [ComVisible(true)]
    [Title("Custom Panel")]
    [Guid("12345678-1234-1234-1234-123456789ABC")]
    public class SwAddin : SwAddInEx
    {
        private ISwTaskPane<ExportTaskPaneControl> m_TaskPane;
        private ExportTaskPaneControl m_WpfControl;

        public override void OnConnect()
        {
            try
            {
                m_TaskPane = this.CreateTaskPaneWpf<ExportTaskPaneControl>();
                m_WpfControl = m_TaskPane.Control;
                TryShowTaskPane(m_TaskPane);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Custom Panel – błąd OnConnect:\n" + ex.Message, "SolidWorks Add-in", MessageBoxButton.OK, MessageBoxImage.Warning);
                throw;
            }
        }

        public override void OnDisconnect()
        {
            if (m_TaskPane != null)
                m_TaskPane.Close();
        }

        /// <summary>Wywołuje Show() na Task Pane, jeśli metoda istnieje (np. Xarial udostępnia to w części wersji).</summary>
        private static void TryShowTaskPane(object taskPane)
        {
            if (taskPane == null) return;
            try
            {
                var method = taskPane.GetType().GetMethod("Show", Type.EmptyTypes);
                method?.Invoke(taskPane, null);
            }
            catch { /* ignoruj */ }
        }
    }
}
