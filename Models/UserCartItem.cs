namespace NewShoppingCart.Models
{
    public class UserCartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ItemName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice => Price * Quantity;
    }
}
