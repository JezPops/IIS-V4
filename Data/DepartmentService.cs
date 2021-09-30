using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_V4.Data
{
    public class DepartmentService
    {
        public Task<Departments[]> GetBestSellingDepartmentsAsync(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
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
                var departments = db.Query<Departments>("Select FIRST 5 " +
                                                  "vs.DeptID," +
                                                  "d.Name," +
                                                  "sum(vs.sell_net)SellNet," +
                                                  "sum(vs.sell_net + vs.Sell_vat)SellVAT, " +
                                                  "sum(vs.COST_NET)COSTNET, " +
                                                  "sum(vs.COST_NET + vs.COST_VAT)COSTVAT, " +
                                                  "sum(vs.SELL_QTY)SumSellQty " +
                                                  "from vesales vs " +
                                                  "inner join vetrans vt " +
                                                  "on vt.TRANSID = vs.TRANSID " +
                                                  "inner join vedepts d " +
                                                  "on d.DeptID = vs.DeptID " +
                                                  "where vt.DateTime>=@StartDate and vt.DateTime <= @EndDate " +
                                                  "Group by vs.DeptID, d.Name " +
                                                  "Order by SellNet desc", parameters).ToList();
                if (departments.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, departments.Count).Select(index => new Departments
                    {
                        Name = departments[index].Name,
                        SELLNET = departments[index].SELLNET,
                        COSTNET = departments[index].COSTNET,
                        SELLVAT = departments[index].SELLVAT,
                        COSTVAT = departments[index].COSTVAT,
                        PROFIT = (departments[index].SELLNET - departments[index].COSTNET),
                        SumSellQty = departments[index].SumSellQty
                    }).ToArray());
                }
                else
                    return Task.FromResult<Departments[]>(null);
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID };
                var departments = db.Query<Departments>("Select FIRST 5 " +
                                                  "vs.DeptID, " +
                                                  "d.Name, " +
                                                  "sum(vs.sell_net)SellNet," +
                                                  "sum(vs.sell_net + vs.Sell_vat)SellVAT, "+
                                                  "sum(vs.COST_NET)COSTNET, " +
                                                  "sum(vs.COST_NET + vs.COST_VAT)COSTVAT, "+
                                                  "sum(vs.SELL_QTY)SumSellQty " +
                                                  "from vesales vs " +
                                                  "inner join vetrans vt " +
                                                  "on vt.TRANSID = vs.TRANSID " +
                                                  "inner join vedepts d " +
                                                  "on d.DeptID = vs.DeptID " +
                                                  "where vt.DateTime>=@StartDate and vt.DateTime <= @EndDate AND vt.BRANCHID=@BranchID " +
                                                  "Group by vs.DeptID, d.Name " +
                                                  "Order by SellNet desc", parameters).ToList();
                if (departments.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, departments.Count).Select(index => new Departments
                    {
                        Name = departments[index].Name,
                        SELLNET = departments[index].SELLNET,
                        COSTNET = departments[index].COSTNET,
                        SELLVAT = departments[index].SELLVAT,
                        COSTVAT = departments[index].COSTVAT,
                        PROFIT = (departments[index].SELLNET - departments[index].COSTNET),
                        SumSellQty = departments[index].SumSellQty
                    }).ToArray());
                }
                else
                    return Task.FromResult<Departments[]>(null);

            }
        }
    }
}
