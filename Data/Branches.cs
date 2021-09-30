using System;
using System.ComponentModel.DataAnnotations;

namespace IIS_V4.Data
{
    public class Branches
    {
        [Key]
        public Int64 BRANCHID { get; set; }
        public Int64 PARENTID { get; set; }
        public string Name { get; set; }
        public string BranchStringID { get; set; }
    }
}
