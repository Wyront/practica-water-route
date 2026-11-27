using WaterRoute.Controls;

namespace WaterRoute.Forms;

partial class StorekeeperImportForm
{
    private void InitializeComponent()
    {
        var importPage = new ImportPage
        {
            Dock = DockStyle.Fill
        };
        Content.Controls.Add(importPage);
        SaveButton.Visible = false;
    }
}
