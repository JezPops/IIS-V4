using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IIS_V4.Data
{
    public class Departments
    {
        [Key]
        public Int64 DeptID { get; set; }
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double SELLNET { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double SELLVAT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double COSTNET { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double COSTVAT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double PROFIT { get; set; }

        public double MARGIN
        {
            get
            {
                return Math.Round(((SELLNET - COSTNET) / SELLNET) * 100, 2, MidpointRounding.AwayFromZero);
            }

            set
            {
                value = Math.Round(((SELLNET - COSTNET) / SELLNET) * 100, 2, MidpointRounding.AwayFromZero);
                return;
            }
        }

        public double SumSellQty { get; set; }
    }
}
