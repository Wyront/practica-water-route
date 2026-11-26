namespace WaterRoute.Controls;

partial class DirectoriesPage
{
    private TabControl directoriesTabControl = null!;
    private TabPage productsTabPage = null!;
    private RegistryPage productsPage = null!;
    private TabPage warehousesTabPage = null!;
    private RegistryPage warehousesPage = null!;
    private TabPage vehiclesTabPage = null!;
    private RegistryPage vehiclesPage = null!;
    private TabPage paymentsTabPage = null!;
    private RegistryPage paymentsPage = null!;
    private TabPage waterTypesTabPage = null!;
    private RegistryPage waterTypesPage = null!;

    private void InitializeComponent()
    {
        directoriesTabControl = new TabControl();
        productsTabPage = new TabPage();
        productsPage = new RegistryPage();
        warehousesTabPage = new TabPage();
        warehousesPage = new RegistryPage();
        vehiclesTabPage = new TabPage();
        vehiclesPage = new RegistryPage();
        paymentsTabPage = new TabPage();
        paymentsPage = new RegistryPage();
        waterTypesTabPage = new TabPage();
        waterTypesPage = new RegistryPage();
        directoriesTabControl.SuspendLayout();
        productsTabPage.SuspendLayout();
        warehousesTabPage.SuspendLayout();
        vehiclesTabPage.SuspendLayout();
        paymentsTabPage.SuspendLayout();
        waterTypesTabPage.SuspendLayout();
        SuspendLayout();
        // 
        // directoriesTabControl
        // 
        directoriesTabControl.Controls.Add(productsTabPage);
        directoriesTabControl.Controls.Add(warehousesTabPage);
        directoriesTabControl.Controls.Add(vehiclesTabPage);
        directoriesTabControl.Controls.Add(paymentsTabPage);
        directoriesTabControl.Controls.Add(waterTypesTabPage);
        directoriesTabControl.Dock = DockStyle.Fill;
        directoriesTabControl.Name = "directoriesTabControl";
        directoriesTabControl.Padding = new Point(14, 8);
        directoriesTabControl.SelectedIndex = 0;
        // 
        // productsTabPage
        // 
        productsTabPage.Controls.Add(productsPage);
        productsTabPage.Name = "productsTabPage";
        productsTabPage.Padding = new Padding(3);
        productsTabPage.Text = "Товары";
        productsTabPage.UseVisualStyleBackColor = true;
        // 
        // productsPage
        // 
        productsPage.Dock = DockStyle.Fill;
        productsPage.Name = "productsPage";
        // 
        // warehousesTabPage
        // 
        warehousesTabPage.Controls.Add(warehousesPage);
        warehousesTabPage.Name = "warehousesTabPage";
        warehousesTabPage.Padding = new Padding(3);
        warehousesTabPage.Text = "Склады";
        warehousesTabPage.UseVisualStyleBackColor = true;
        // 
        // warehousesPage
        // 
        warehousesPage.Dock = DockStyle.Fill;
        warehousesPage.Name = "warehousesPage";
        // 
        // vehiclesTabPage
        // 
        vehiclesTabPage.Controls.Add(vehiclesPage);
        vehiclesTabPage.Name = "vehiclesTabPage";
        vehiclesTabPage.Padding = new Padding(3);
        vehiclesTabPage.Text = "Автомобили";
        vehiclesTabPage.UseVisualStyleBackColor = true;
        // 
        // vehiclesPage
        // 
        vehiclesPage.Dock = DockStyle.Fill;
        vehiclesPage.Name = "vehiclesPage";
        // 
        // paymentsTabPage
        // 
        paymentsTabPage.Controls.Add(paymentsPage);
        paymentsTabPage.Name = "paymentsTabPage";
        paymentsTabPage.Padding = new Padding(3);
        paymentsTabPage.Text = "Способы оплаты";
        paymentsTabPage.UseVisualStyleBackColor = true;
        // 
        // paymentsPage
        // 
        paymentsPage.Dock = DockStyle.Fill;
        paymentsPage.Name = "paymentsPage";
        // 
        // waterTypesTabPage
        // 
        waterTypesTabPage.Controls.Add(waterTypesPage);
        waterTypesTabPage.Name = "waterTypesTabPage";
        waterTypesTabPage.Padding = new Padding(3);
        waterTypesTabPage.Text = "Типы воды и тары";
        waterTypesTabPage.UseVisualStyleBackColor = true;
        // 
        // waterTypesPage
        // 
        waterTypesPage.Dock = DockStyle.Fill;
        waterTypesPage.Name = "waterTypesPage";
        // 
        // DirectoriesPage
        // 
        Content.Controls.Add(directoriesTabControl);
        Name = "DirectoriesPage";
        Size = new Size(1000, 700);
        directoriesTabControl.ResumeLayout(false);
        productsTabPage.ResumeLayout(false);
        warehousesTabPage.ResumeLayout(false);
        vehiclesTabPage.ResumeLayout(false);
        paymentsTabPage.ResumeLayout(false);
        waterTypesTabPage.ResumeLayout(false);
        ResumeLayout(false);
    }
}
