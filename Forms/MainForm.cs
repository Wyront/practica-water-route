using Microsoft.Extensions.DependencyInjection;
using WaterRoute.Controls;
using WaterRoute.Infrastructure;
using WaterRoute.Models;
using WaterRoute.Services;

namespace WaterRoute.Forms;

public sealed partial class MainForm : Form
{
    private readonly UserSession _session;
    private readonly ICurrentUser _currentUser;
    private readonly IServiceProvider _services;
    private readonly INotificationService _notifications;
    private readonly List<Button> _navigationButtons;
    private Control? _currentPage;

    // Только для конструктора визуального дизайнера; во время работы приложения не используется.
    public MainForm() : this(RoleInfo.CreateDemoSession(UserRole.Admin), new CurrentUser(), null!, new ToastNotificationService())
    {
    }

    public MainForm(UserSession session, ICurrentUser currentUser, IServiceProvider services, INotificationService notifications)
    {
        _session = session;
        _currentUser = currentUser;
        _services = services;
        _notifications = notifications;
        if (notifications is ToastNotificationService toast)
        {
            toast.SetOwner(this);
            FormClosed += (_, _) => toast.ReleaseOwner(this);
        }
        InitializeComponent();
        _navigationButtons = navigationPanel.Controls.OfType<Button>()
            .Where(b => b.Tag is PageId)
            .ToList();
        Text = $"WaterRoute — {RoleInfo.GetName(session.Role)}";
        roleLabel.Text = RoleInfo.GetName(session.Role);
        nameLabel.Text = $"{session.FullName}  ·  {session.Email}";
        _currentUser.SetSession(session);
        ConfigureNavigation();
        KeyPreview = true;
    }

    private void ConfigureNavigation()
    {
        var allowedPages = PageCatalog.GetNavigation(_session.Role).Select(item => item.Id).ToHashSet();
        foreach (var button in _navigationButtons)
            button.Visible = allowedPages.Contains((PageId)button.Tag!);
    }

    public void ShowPage(PageId id)
    {
        foreach (var button in _navigationButtons)
            button.BackColor = (PageId)button.Tag! == id ? Infrastructure.UITheme.NavigationActive : Infrastructure.UITheme.Navigation;

        Control page;
        try
        {
            page = PageFactory.Create(id, _session, _currentUser, _services.GetRequiredService<AuthService>(), _services);
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Не удалось открыть раздел", ex.Message);
            return;
        }

        if (page is DashboardPage dashboard)
            dashboard.NavigationRequested += (_, pageId) => ShowPage(pageId);

        // Некоторые разделы — это формы (BaseEditForm); чтобы встроить их в панель, они не должны быть окнами верхнего уровня.
        if (page is Form pageForm)
        {
            pageForm.TopLevel = false;
            pageForm.FormBorderStyle = FormBorderStyle.None;
        }

        contentPanel.SuspendLayout();
        _currentPage?.Dispose();
        _currentPage = page;
        _currentPage.Dock = DockStyle.Fill;
        contentPanel.Controls.Add(_currentPage);
        if (_currentPage is Form embedded)
            embedded.Show();
        contentPanel.ResumeLayout();
    }

    private void navigationButton_Click(object? sender, EventArgs e)
    {
        if (sender is Button { Tag: PageId pageId })
        {
            // Admin-specific forms
            if (pageId == PageId.Users)
            {
                OpenAdminForm<UserManagementForm>();
                return;
            }
            if (pageId == PageId.Directories)
            {
                OpenAdminForm<DirectoryManagementForm>();
                return;
            }
            if (pageId == PageId.Reports && _session.Role == UserRole.Admin)
            {
                OpenAdminForm<AdminReportsForm>();
                return;
            }
            ShowPage(pageId);
        }
    }

    private void OpenAdminForm<TForm>() where TForm : Form
    {
        using var form = _services.GetRequiredService<TForm>();
        form.ShowDialog(this);
    }

    private void notificationsButton_Click(object? sender, EventArgs e) => ShowPage(PageId.Notifications);

    private void offlineButton_Click(object? sender, EventArgs e)
    {
        using var form = new OfflineCallForm();
        form.ShowDialog(this);
    }

    private async void logoutButton_Click(object? sender, EventArgs e)
    {
        var answer = MessageBox.Show(this, "Выйти из учётной записи?", "WaterRoute",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (answer != DialogResult.Yes)
            return;

        logoutButton.Enabled = false;
        try
        {
            // Удаляем сохранённый вход («Запомнить меня»), иначе при следующем запуске пользователь войдёт сам.
            if (_services?.GetService<AuthService>() is { } auth)
                await auth.LogoutAsync();
        }
        catch
        {
            // Не удалось стереть файл сессии — всё равно выходим.
        }

        _currentUser.Clear();
        // DialogResult.OK возвращает управление окну входа; любое другое закрытие завершает приложение.
        DialogResult = DialogResult.OK;
        Close();
    }

    private void MainForm_Shown(object? sender, EventArgs e)
    {
        ShowPage(PageId.Dashboard);
        _notifications.ShowSuccess("WaterRoute", "Добро пожаловать! Система готова к работе.");
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        return keyData switch
        {
            Keys.F5 => HandleF5(),
            Keys.Control | Keys.F => HandleCtrlF(),
            Keys.Control | Keys.N => HandleCtrlN(),
            _ => base.ProcessCmdKey(ref msg, keyData)
        };
    }

    private bool HandleF5()
    {
        if (contentPanel.Controls.OfType<DashboardPage>().FirstOrDefault() is { } dashboard)
        {
            _ = dashboard.LoadDashboardDataAsync();
            return true;
        }
        return false;
    }

    private bool HandleCtrlF()
    {
        if (contentPanel.Controls.OfType<RegistryPage>().FirstOrDefault() is { } registry)
        {
            registry.FocusSearch();
            return true;
        }
        return false;
    }

    private bool HandleCtrlN()
    {
        if (contentPanel.Controls.OfType<RegistryPage>().FirstOrDefault() is { } registry)
        {
            registry.CreateNew();
            return true;
        }
        return false;
    }
}
