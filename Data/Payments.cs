using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IIS_V4.Data
{
    public class Payments
    {
        [Key]
        public Int64 TransID { get; set; }

        public int Count { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double PayAmount { get; set; }
    }
}
