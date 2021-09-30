using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_V4.Data
{
    public class ProductsService
    {
        public Task<Products[]> GetBestSellingProductsbyDeptAsync(DateTimeOffset? Start, DateTimeOffset? End, Int64 DeptID, Int64 BranchID = default)
        {
            IDbConnection db = new FbConnection(Startup.VectorConnection);
            if (End == DateTime.MinValue)
            {
                End = DateTime.Now;
            }

            DateTime? _starts = Start.HasValue ? Start.Value.DateTime : DateTime.Today;
            DateTime? _ends = End.HasValue ? End.Value.DateTime : DateTime.Now;

            DateTime StartDate = (DateTime)_starts;
            DateTime EndDate = (DateTime)_ends;

            if (BranchID == 0)
            {
                var parameters = new { StartDate, EndDate, DeptID };
                var products = db.Query<Products>("Select FIRST 10 " +
                                                  "vs.STOCKID," +
                                                  "vs.DESCRIPTION," +
                                                  "sum(vs.sell_net)SellNet," +
                                                  "sum(vs.SELL_QTY)SumSellQty " +
                                                  "from vesales vs " +
                                                  "inner join vetrans vt " +
                                                  "on vt.TRANSID = vs.TRANSID " +
                                                  "join vedepts d on d.deptid = s.deptId " +
                                                  "where vt.DateTime>=@StartDate and vt.DateTime <= @EndDate " +
                                                  "and d.deptId=@DeptID " +
                                                  "Group by vs.STOCKID, vs.DESCRIPTION " +
                                                  "Order by SellNet desc", parameters).ToList();
                if (products.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, products.Count).Select(index => new Products
                    {
                        Description = products[index].Description,
                        SellNet = products[index].SellNet,
                        SumSellQty = products[index].SumSellQty
                    }).ToArray());
                }
                else
                    return Task.FromResult<Products[]>(null);
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID, DeptID };
                var products = db.Query<Products>("Select FIRST 10 " +
                                                  "vs.STOCKID," +
                                                  "vs.DESCRIPTION," +
                                                  "sum(vs.sell_net)SellNet," +
                                                  "sum(vs.SELL_QTY)SumSellQty " +
                                                  "from vesales vs " +
                                                  "inner join vetrans vt " +
                                                  "on vt.TRANSID = vs.TRANSID " +
                                                  "join vedepts d on d.deptid = vs.deptID " +
                                                  "where vt.DateTime>=@StartDate and vt.DateTime <= @EndDate AND vt.BRANCHID=@BranchID " +
                                                  "and vs.DeptID = d.DeptID " +
                                                  "Group by vs.STOCKID, vs.DESCRIPTION " +
                                                  "Order by SellNet desc", parameters).ToList();
                if (products.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, products.Count).Select(index => new Products
                    {
                        Description = products[index].Description,
                        SellNet = products[index].SellNet,
                        SumSellQty = products[index].SumSellQty
                    }).ToArray());
                }
                else
                    return Task.FromResult<Products[]>(null);
            }
        }

        public Task<Products[]> GetBestSellingProductsAsync(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
        {
            IDbConnection db = new FbConnection(Startup.VectorConnection);
            if (End == DateTime.MinValue)
            {
                End = DateTime.Now;
            }

            DateTime? _starts = Start.HasValue ? Start.Value.DateTime : DateTime.Today;
            DateTime? _ends = End.HasValue ? End.Value.DateTime : DateTime.Now;

            DateTime StartDate = (DateTime)_starts;
            DateTime EndDate = (DateTime)_ends;

            if (BranchID == 0)
            {
                var parameters = new { StartDate, EndDate };
                var products = db.Query<Products>("Select FIRST 10 " +
                                                  "vs.STOCKID," +
                                                  "vs.DESCRIPTION," +
                                                  "sum(vs.sell_net)SellNet," +
                                                  "sum(vs.SELL_QTY)SumSellQty " +
                                                  "from vesales vs " +
                                                  "inner join vetrans vt " +
                                                  "on vt.TRANSID = vs.TRANSID " +
                                                  "where vt.DateTime>=@StartDate and vt.DateTime <= @EndDate " +
                                                  "Group by vs.STOCKID, vs.DESCRIPTION " +
                                                  "Order by SellNet desc", parameters).ToList();
                if (products.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, products.Count).Select(index => new Products
                    {
                        Description = products[index].Description,
                        SellNet = products[index].SellNet,
                        SumSellQty = products[index].SumSellQty
                    }).ToArray());
                }
                else
                    return Task.FromResult<Products[]>(null);
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID };
                var products = db.Query<Products>("Select FIRST 10 " +
                                                  "vs.STOCKID," +
                                                  "vs.DESCRIPTION," +
                                                  "sum(vs.sell_net)SellNet," +
                                                  "sum(vs.SELL_QTY)SumSellQty " +
                                                  "from vesales vs " +
                                                  "inner join vetrans vt " +
                                                  "on vt.TRANSID = vs.TRANSID " +
                                                  "where vt.DateTime>=@StartDate and vt.DateTime <= @EndDate AND vt.BRANCHID=@BranchID " +
                                                  "Group by vs.STOCKID, vs.DESCRIPTION " +
                                                  "Order by SellNet desc", parameters).ToList();
                if (products.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, products.Count).Select(index => new Products
                    {
                        Description = products[index].Description,
                        SellNet = products[index].SellNet,
                        SumSellQty = products[index].SumSellQty
                    }).ToArray());
                }
                else
                    return Task.FromResult<Products[]>(null);
            }
        }
    }
}
