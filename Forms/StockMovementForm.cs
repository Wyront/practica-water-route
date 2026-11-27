namespace WaterRoute.Forms;

public partial class StockMovementForm : BaseEditForm
{
    public StockMovementForm()
        : base("Складская операция", "Остаток и запись в журнале сохраняются одной транзакцией", 680, 590)
    {
        InitializeComponent();
    }
}
