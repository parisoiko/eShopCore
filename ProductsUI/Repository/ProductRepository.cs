namespace ProductsUI.Repository
{
    public class ProductRepository
    {
        string connectionString;

        public ProductRepository(string connectionString) {
            this.connectionString = connectionString;
        }

        public List<Products> GetProducts() {
            List<Products> products = new List<Products>();
            Products product;

            var data = GetProductDetailsFromDb();

            foreach (DataRow row in data.Rows) {
                product = new Products {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Category = row["Category"].ToString(),
                    Price = Convert.ToDecimal(row["Price"])
                };
                products.Add(product);
            }

            return products;
        }

        private DataTable GetProductDetailsFromDb() {
            var query = "SELECT Id, Name, Category, Price FROM Product";
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                try {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection)) {
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            dataTable.Load(reader);
                        }
                    }

                    return dataTable;
                }
                catch (Exception ex) {
                    throw;
                }
                finally {
                    connection.Close();
                }
            }
        }



    }
}
