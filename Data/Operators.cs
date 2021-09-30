using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IIS_V4.Data
{
    public class Operators
    {
        [Key]
        public Int64 OpID { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double SellNet { get; set; }

        public double SumSellQty { get; set; }
    }
}
