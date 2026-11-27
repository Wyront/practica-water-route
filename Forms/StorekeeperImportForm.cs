namespace WaterRoute.Forms;

public sealed partial class StorekeeperImportForm : BaseEditForm
{
    public StorekeeperImportForm() : base("Импорт CSV", "Товары и складские поступления. При любой ошибке весь файл отклоняется", 900, 650)
    {
        InitializeComponent();
    }
}
