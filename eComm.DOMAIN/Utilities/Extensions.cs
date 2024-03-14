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
    }
}
