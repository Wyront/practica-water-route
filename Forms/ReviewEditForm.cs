namespace WaterRoute.Forms;

public partial class ReviewEditForm : BaseEditForm
{
    public ReviewEditForm()
        : base("Отзыв о доставке WR-10241", "Независимые оценки курьера и заказа после полной доставки", 760, 690)
    {
        InitializeComponent();
    }

    public ReviewEditForm(bool readOnly)
        : base("Отзыв о доставке WR-10241", "Независимые оценки курьера и заказа после полной доставки", 760, 690)
    {
        InitializeComponent(readOnly);
    }
}
