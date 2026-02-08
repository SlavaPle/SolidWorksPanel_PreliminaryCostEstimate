using System.Runtime.InteropServices;
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

        public override void OnConnect()
        {
            m_TaskPane = this.CreateTaskPaneWpf<ExportTaskPaneControl>();
        }

        public override void OnDisconnect()
        {
            m_TaskPane?.Close();
        }
    }
}
