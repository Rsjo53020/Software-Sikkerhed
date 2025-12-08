using WebGoatCore.Models;

public class CartItemDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public double UnitPrice { get; set; }
    public short Quantity { get; set; }

    public double LineTotal => UnitPrice * Quantity;
}