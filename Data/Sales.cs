using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IIS_V4.Data
{
    public class Sales
    {
        [Key]
        public Int64 TRANSID { get; set; }
        public DateTime TransDate { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double CASHSELLTOT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double CASHRETTOT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTSELLTOT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTRETTOT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double CASHSOTOT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTSOTOT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTDEPTSPENT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTDELTOT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double CASHCOTOT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTCOTOT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double CASHSELLNET { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double CASHRETNET { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTSELLNET { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTRETNET { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double CASHSONET { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTSONET { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTDEPTSPENTNET { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTDELNET { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double CASHCONET { get; set; }


        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTCONET { get; set; }


        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double COSTNETTOT { get; set; }


        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double CASHCHRDTOT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double ACCTCHRDTOT { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double TOTAL { get; set; }

        public double MARGIN
        {
            get
            {
                return Math.Round(((TOTAL - COSTNETTOT) / TOTAL) * 100, 2, MidpointRounding.AwayFromZero);
            }

            set
            {
                value = Math.Round(((TOTAL - COSTNETTOT) / TOTAL) * 100, 2, MidpointRounding.AwayFromZero);
                return;
            }
        }

        public double MARKUP 
        {
            get
            {
                return Math.Round(((TOTAL - COSTNETTOT) / COSTNETTOT) * 100, 2, MidpointRounding.AwayFromZero);
            }

            set
            {
                value = Math.Round(((TOTAL - COSTNETTOT) / COSTNETTOT) * 100, 2, MidpointRounding.AwayFromZero);
                return;
            }
        }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public double PROFIT { get; set; }

        public double SUMSELLQTY { get; set; }

        public Int32 TransCount { get; set; }

        public string BRANCHNAME { get; set; }

    }
}
