using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

partial class DeliveryForm
{
    private Panel infoPanel = null!;
    private Label routeLabel = null!;
    private Label addressLabel = null!;
    private FlowLayoutPanel actionsPanel = null!;
    private Button startButton = null!;
    private Button arrivedButton = null!;
    private Button chatButton = null!;
    private DataGridView itemsGrid = null!;
    private Panel resultPanel = null!;
    private Label returnTitleLabel = null!;
    private Label returnLabel = null!;
    private NumericUpDown returnedNumeric = null!;
    private Label commentLabel = null!;
    private TextBox commentTextBox = null!;
    private Button failedButton = null!;
    private Button deliveredButton = null!;

    private void InitializeComponent()
    {
        infoPanel = new Panel(); routeLabel = new Label(); addressLabel = new Label(); actionsPanel = new FlowLayoutPanel();
        startButton = new Button(); arrivedButton = new Button(); chatButton = new Button(); itemsGrid = new DataGridView();
        resultPanel = new Panel(); returnTitleLabel = new Label(); returnLabel = new Label(); returnedNumeric = new NumericUpDown();
        commentLabel = new Label(); commentTextBox = new TextBox(); failedButton = new Button(); deliveredButton = new Button();
        infoPanel.SuspendLayout(); actionsPanel.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)itemsGrid).BeginInit(); resultPanel.SuspendLayout(); SuspendLayout();
        SetFormHeader("Обработка доставки · WR-10241", "Полная передача всех позиций либо недоставка без переданных товаров");
        ClientSize = new Size(1050, 820);
        infoPanel.Dock = DockStyle.Top; infoPanel.Height = 86; infoPanel.BackColor = UITheme.SurfaceMuted; infoPanel.Padding = new Padding(14);
        routeLabel.Dock = DockStyle.Top; routeLabel.Text = "R-021  ·  Клиент №1  ·  +7 900 000-00-11"; routeLabel.Font = UITheme.BodyFont(11F, FontStyle.Bold);
        addressLabel.Dock = DockStyle.Top; addressLabel.Text = "Невский проспект, 10, кв. 5  ·  Окно 12:00–15:00"; addressLabel.Font = UITheme.BodyFont(9.5F); addressLabel.ForeColor = UITheme.TextMuted;
        infoPanel.Controls.Add(addressLabel); infoPanel.Controls.Add(routeLabel);
        actionsPanel.Dock = DockStyle.Top; actionsPanel.Height = 50; actionsPanel.WrapContents = false;
        ConfigureButton(startButton, "Начать маршрут", 145, false); ConfigureButton(arrivedButton, "Курьер прибыл", 145, false);
        ConfigureButton(chatButton, "Чат с диспетчером", 175, false);
        actionsPanel.Controls.Add(startButton); actionsPanel.Controls.Add(arrivedButton); actionsPanel.Controls.Add(chatButton);
        UITheme.StyleGrid(itemsGrid);
        itemsGrid.Columns.Add("Product", "Товар"); itemsGrid.Columns.Add("Ordered", "Заказано"); itemsGrid.Columns.Add("Delivered", "Передано"); itemsGrid.Columns.Add("Returnable", "Возвратная тара");
        itemsGrid.Rows.Add("Вода питьевая 19 л", "4", "4", "Да"); itemsGrid.Rows.Add("Вода питьевая 5 л", "6", "6", "Нет"); itemsGrid.Rows.Add("Помпа механическая", "1", "1", "Нет");
        resultPanel.Dock = DockStyle.Bottom; resultPanel.Height = 210; resultPanel.BackColor = UITheme.Surface; resultPanel.Padding = new Padding(14);
        returnTitleLabel.AutoSize = true; returnTitleLabel.Location = new Point(14, 12); returnTitleLabel.Text = "Возврат тары и результат"; returnTitleLabel.Font = UITheme.BodyFont(11F, FontStyle.Bold);
        resultPanel.Controls.Add(returnTitleLabel);
        returnLabel.AutoSize = true; returnLabel.Location = new Point(14, 54); returnLabel.Text = "Принято бутылей"; resultPanel.Controls.Add(returnLabel);
        returnedNumeric.Location = new Point(180, 48); returnedNumeric.Width = 140; returnedNumeric.Maximum = 10000; resultPanel.Controls.Add(returnedNumeric);
        commentLabel.AutoSize = true; commentLabel.Location = new Point(14, 94); commentLabel.Text = "Комментарий"; resultPanel.Controls.Add(commentLabel);
        commentTextBox.SetBounds(180, 88, 680, 58); commentTextBox.Multiline = true; commentTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; resultPanel.Controls.Add(commentTextBox);
        failedButton.SetBounds(180, 158, 230, 38); failedButton.Text = "Не доставлено + проблема"; failedButton.ForeColor = UITheme.Danger; failedButton.FlatStyle = FlatStyle.Flat;
        deliveredButton.SetBounds(430, 158, 220, 38); deliveredButton.Text = "Доставлено полностью"; deliveredButton.BackColor = UITheme.Primary; deliveredButton.ForeColor = Color.White; deliveredButton.FlatStyle = FlatStyle.Flat;
        resultPanel.Controls.Add(failedButton); resultPanel.Controls.Add(deliveredButton);
        contentPanel.Controls.Add(itemsGrid); contentPanel.Controls.Add(resultPanel); contentPanel.Controls.Add(actionsPanel); contentPanel.Controls.Add(infoPanel);
        infoPanel.ResumeLayout(false); actionsPanel.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)itemsGrid).EndInit(); resultPanel.ResumeLayout(false); resultPanel.PerformLayout(); ResumeLayout(false);
    }

    private static void ConfigureButton(Button button, string text, int width, bool primary)
    {
        button.Text = text; button.Width = width; button.Height = 38; button.BackColor = primary ? UITheme.Primary : UITheme.Surface;
        button.ForeColor = primary ? Color.White : UITheme.Text; button.FlatStyle = FlatStyle.Flat;
    }
}
