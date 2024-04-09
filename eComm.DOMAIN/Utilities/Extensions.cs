using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using System.Data;

namespace eComm.DOMAIN.Utilities
{
    public static class Extensions
    {
        public static DataTable ToDataTable(this List<string> isbnList)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("isbn");

            foreach (string isbn in isbnList)
            {
                DataRow row = dt.NewRow();
                row["isbn"] = isbn;
                dt.Rows.Add(row);
            }

            return dt;
        }

        public static List<ProductDTO> MapProductsToDTO(this List<Product> products)
        {
            var productsDTO = new List<ProductDTO>();

            foreach (var product in products)
            {
                var productDTO = new ProductDTO()
                {
                    ISBN = product.ISBN,
                    Author = product.Author,
                    Title = product.Title,
                    Publisher = product.Publisher,
                    PublicationYear = product.PublicationYear,
                    ImageUrlM = product.ImageUrlM
                };
                productsDTO.Add(productDTO);
            }

            return productsDTO;
        }

        public static ProductDTO MapProductToDTO(this Product product)
        {
            return new ProductDTO()
            {
                ISBN = product.ISBN,
                Title = product.Title,
                Author = product.Author,
                Publisher = product.Publisher,
                PublicationYear = product.PublicationYear,
                ImageUrlM = product.ImageUrlM
            };
        }
    }
}
