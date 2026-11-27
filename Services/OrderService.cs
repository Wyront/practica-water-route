using System.Text.Json;
using WaterRoute.Infrastructure;

namespace WaterRoute.Services;

public sealed record CartItem(int ProductId, int Qty);
public sealed record CreateOrderResult(int OrderId, string Status);

public sealed class OrderService
{
    private readonly IRepository _repo;
    public OrderService(IRepository repo) => _repo = repo;

    public async Task<CreateOrderResult> CreateAsync(int clientId, int addressId, int paymentMethodId,
        string? comment, IReadOnlyList<CartItem> items, CancellationToken ct = default)
    {
        var itemsJson = JsonSerializer.Serialize(items.Select(i => new { product_id = i.ProductId, qty = i.Qty }));

        var output = await _repo.ExecuteWithOutputsAsync("sp_order_create_client",
            new
            {
                client_id = clientId,
                address_id = addressId,
                payment_method_id = paymentMethodId,
                comment = comment ?? "",
                items_json = itemsJson
            },
            new[] { "p_order_id", "p_status" }, ct);

        return new CreateOrderResult(Convert.ToInt32(output["p_order_id"]), (string)output["p_status"]!);
    }

    // Параметры процедуры: p_order_id, p_reason, p_user_id (кто отменяет — попадает в историю статусов).
    public Task CancelAsync(int orderId, string reason, int userId, CancellationToken ct = default)
        => _repo.ExecuteAsync("sp_order_cancel", new { order_id = orderId, reason, user_id = userId }, ct);
}
