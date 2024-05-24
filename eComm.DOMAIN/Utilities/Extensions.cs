using eComm.DOMAIN.DTO;
using eComm.DOMAIN.Models;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using Stripe.Checkout;
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

        public static List<ProductDTO> MapProductsToDTO(this List<Models.Product> products)
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
                    ImageUrlL = product.ImageUrlL,
                    AverageRating = product.AverageRating,
                    Price = product.Price
                };
                productsDTO.Add(productDTO);
            }

            return productsDTO;
        }

        public static ProductDTO MapProductToDTO(this Models.Product product)
        {
            return new ProductDTO()
            {
                ISBN = product.ISBN,
                Title = product.Title,
                Author = product.Author,
                Publisher = product.Publisher,
                PublicationYear = product.PublicationYear,
                ImageUrlL = product.ImageUrlL,
                AverageRating = product.AverageRating,
                Price = product.Price,
            };
        }

        public static List<SessionLineItemOptions> MapToLineItems(this List<CartProduct> products)
        {
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var product in products)
            {
                var lineItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        Currency = "usd",
                        ProductData = new()
                        {
                            Name = product.Title,
                            Description = product.ISBN,
                            Images = [product.ImageUrlS]
                        },
                        UnitAmount = (product.Price * 100) / product.Count
                    },
                    Quantity = product.Count,
                };
                lineItems.Add(lineItem);
            }

            return lineItems;
        }

        public static byte[] ConvertToPdf(this byte[] htmlByteArr)
        {
            using (var memoryStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(memoryStream);
                PdfDocument pdf = new PdfDocument(writer);
                var document = HtmlConverter.ConvertToDocument(new MemoryStream(htmlByteArr), pdf, new ConverterProperties());
                document.Close();
                return memoryStream.ToArray();
            }
        }
    }
}
