namespace ProductsUI.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string DescriptionShort { get; set; }
        public string DescriptionLong { get; set; }
        public string Category { get; set; }
        public float Price { get; set; }
        public string ImageSource { get; set; }
    }
}
