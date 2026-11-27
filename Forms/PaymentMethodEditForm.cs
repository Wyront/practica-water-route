using WaterRoute.Infrastructure;

namespace WaterRoute.Forms;

public sealed class PaymentMethodEditForm : BaseEditForm
{
    private readonly IRepository? _repo;
    private readonly IDictionary<string, object?>? _data;
    private readonly TextBox _codeBox;
    private readonly TextBox _nameBox;
    private readonly CheckBox _activeBox;

    public PaymentMethodEditForm() : this(null, null) { }

    public PaymentMethodEditForm(IRepository? repo, IDictionary<string, object?>? data) : base(
        data == null ? "Новый способ оплаты" : "Редактирование способа оплаты",
        "Код используется в заказах, название — для отображения клиентам", 550, 420)
    {
        _repo = repo;
        _data = data;

        var table = FormLayout.CreateTwoColumnLayout();
        _codeBox = UITheme.CreateTextBox("cash");
        _nameBox = UITheme.CreateTextBox("Наличные");
        _activeBox = new CheckBox { Text = "Доступен для выбора", Checked = true, AutoSize = true };
        FormLayout.AddRow(table, "Код *", _codeBox);
        FormLayout.AddRow(table, "Название *", _nameBox);
        FormLayout.AddRow(table, "Активен", _activeBox);
        Content.Controls.Add(table);

        if (data != null)
        {
            _codeBox.Text = Convert.ToString(data["code"]) ?? "";
            _nameBox.Text = Convert.ToString(data["name"]) ?? "";
            _activeBox.Checked = Convert.ToBoolean(data["active"]);
        }
    }

    protected override async Task<bool> ValidateAndSaveAsync()
    {
        if (_repo == null) { UiErrors.Warn("Демо-режим: нет подключения к базе данных.", this); return false; }

        var code = _codeBox.Text.Trim().ToLowerInvariant();
        var name = _nameBox.Text.Trim();
        if (code.Length == 0 || name.Length == 0) { UiErrors.Warn("Заполните код и название.", this); return false; }
        if (!System.Text.RegularExpressions.Regex.IsMatch(code, "^[a-z0-9_]{2,50}$"))
        {
            UiErrors.Warn("Код: только латинские буквы, цифры и «_» (от 2 до 50 символов).", this);
            return false;
        }

        await _repo.ExecuteAsync("sp_payment_method_crud", new
        {
            action = _data == null ? "CREATE" : "UPDATE",
            id = _data == null ? 0 : Convert.ToInt32(_data["id"]),
            code,
            name,
            active = (sbyte)(_activeBox.Checked ? 1 : 0)
        });
        return true;
    }
}
