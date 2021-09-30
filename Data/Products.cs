using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IIS_V4.Data
{
    public class Products
    {

        [Key]
        public Int64 StockID { get; set; }
        public Int64 DeptID { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double SellNet { get; set; }
        public double SumSellQty { get; set; }

    }
}
