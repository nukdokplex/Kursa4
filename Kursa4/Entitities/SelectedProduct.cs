namespace Kursa4.Entitities
{
    public class SelectedProduct
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }

        public SelectedProduct(long id, string name, decimal price, int count)
        {
            this.ID = id;
            this.Name = name;
            this.Price = price;
            this.Count = count;
        }
    }
}