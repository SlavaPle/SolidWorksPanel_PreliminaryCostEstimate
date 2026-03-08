using System.Windows.Controls;
using Xarial.XCad.Base.Attributes;
using SolidWorksExportAddin.Resources;

namespace SolidWorksExportAddin
{
    [Title("Preliminary Cost Estimate")]
    [Icon(typeof(Res), nameof(Res.TaskPaneIcon))]
    public partial class ExportTaskPaneControl : UserControl
    {
        public ExportTaskPaneControl()
        {
            InitializeComponent();
        }
    }
}
