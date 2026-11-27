using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class ReviewEditForm
{
    private GroupBox courierGroupBox = null!;
    private GroupBox orderGroupBox = null!;
    private TableLayoutPanel courierFields = null!;
    private TableLayoutPanel orderFields = null!;
    private NumericUpDown courierRatingNumeric = null!;
    private NumericUpDown orderRatingNumeric = null!;
    private TextBox courierTextBox = null!;
    private TextBox orderTextBox = null!;
    private Label noteLabel = null!;

    private void InitializeComponent()
    {
        courierGroupBox = new GroupBox();
        orderGroupBox = new GroupBox();
        courierFields = FormLayout.CreateTwoColumnLayout();
        orderFields = FormLayout.CreateTwoColumnLayout();
        courierRatingNumeric = FormLayout.CreateNumeric(5, 1, 5);
        orderRatingNumeric = FormLayout.CreateNumeric(5, 1, 5);
        courierTextBox = UITheme.CreateTextBox("Вежливость, пунктуальность, аккуратность", true);
        orderTextBox = UITheme.CreateTextBox("Качество воды, состав и оформление заказа", true);
        noteLabel = UITheme.CreateLabel(string.Empty, 9F, FontStyle.Regular, UITheme.TextMuted, false);
        ((System.ComponentModel.ISupportInitialize)courierRatingNumeric).BeginInit();
        ((System.ComponentModel.ISupportInitialize)orderRatingNumeric).BeginInit();
        courierGroupBox.SuspendLayout();
        orderGroupBox.SuspendLayout();
        courierFields.SuspendLayout();
        orderFields.SuspendLayout();
        SuspendLayout();
        //
        // courierGroupBox
        //
        courierGroupBox.Controls.Add(courierFields);
        courierGroupBox.Dock = DockStyle.Top;
        courierGroupBox.Font = UITheme.BodyFont(10F, FontStyle.Bold);
        courierGroupBox.Height = 210;
        courierGroupBox.Margin = new Padding(0, 0, 0, 12);
        courierGroupBox.Name = "courierGroupBox";
        courierGroupBox.Padding = new Padding(14);
        courierGroupBox.TabIndex = 0;
        courierGroupBox.TabStop = false;
        courierGroupBox.Text = "Оценка курьера";
        FormLayout.AddRow(courierFields, "Оценка 1–5", courierRatingNumeric, 42);
        FormLayout.AddRow(courierFields, "Текст отзыва", courierTextBox, 110);
        //
        // orderGroupBox
        //
        orderGroupBox.Controls.Add(orderFields);
        orderGroupBox.Dock = DockStyle.Top;
        orderGroupBox.Font = UITheme.BodyFont(10F, FontStyle.Bold);
        orderGroupBox.Height = 210;
        orderGroupBox.Margin = new Padding(0, 0, 0, 12);
        orderGroupBox.Name = "orderGroupBox";
        orderGroupBox.Padding = new Padding(14);
        orderGroupBox.TabIndex = 1;
        orderGroupBox.TabStop = false;
        orderGroupBox.Text = "Оценка заказа";
        FormLayout.AddRow(orderFields, "Оценка 1–5", orderRatingNumeric, 42);
        FormLayout.AddRow(orderFields, "Текст отзыва", orderTextBox, 110);
        //
        // noteLabel
        //
        noteLabel.Dock = DockStyle.Bottom;
        noteLabel.Height = 58;
        noteLabel.Name = "noteLabel";
        noteLabel.Text = "Один комплект отзывов на заказ. Ответы сотрудников не предусмотрены.";
        //
        // ReviewEditForm
        //
        Content.Controls.Add(noteLabel);
        Content.Controls.Add(orderGroupBox);
        Content.Controls.Add(courierGroupBox);
        Name = "ReviewEditForm";
        ((System.ComponentModel.ISupportInitialize)courierRatingNumeric).EndInit();
        ((System.ComponentModel.ISupportInitialize)orderRatingNumeric).EndInit();
        courierFields.ResumeLayout(false);
        courierFields.PerformLayout();
        orderFields.ResumeLayout(false);
        orderFields.PerformLayout();
        courierGroupBox.ResumeLayout(false);
        courierGroupBox.PerformLayout();
        orderGroupBox.ResumeLayout(false);
        orderGroupBox.PerformLayout();
        ResumeLayout(false);
    }

    private void InitializeComponent(bool readOnly)
    {
        InitializeComponent();
        SaveButton.Visible = !readOnly;
        courierRatingNumeric.Enabled = !readOnly;
        orderRatingNumeric.Enabled = !readOnly;
        courierTextBox.ReadOnly = readOnly;
        orderTextBox.ReadOnly = readOnly;
    }
}
