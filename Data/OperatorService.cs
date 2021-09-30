using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_V4.Data
{
    public class OperatorService
    {
        public Task<Operators[]> GetOperatorRankingAsync(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
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
                var operators = db.Query<Operators>("Select First 10 " +
                                                "o.OpID, " +
                                                "o.Name, " +
                                                "sum(vs.sell_net)SellNet,  " +
                                                "sum(vs.SELL_QTY)SumSellQty  " +
                                                "FROM VEOPS o  " +
                                                "inner Join vetrans vt  " +
                                                "on o.OPID = vt.OPID  " +
                                                "inner join vesales vs  " +
                                                "on vs.TRANSID = vt.TRANSID  " +
                                                "where vt.DateTime>=@StartDate and vt.DateTime <= @EndDate " +
                                                "Group by o.OPID, o.NAME  " +
                                                "Order by SellNet desc", parameters).ToList();
                if (operators.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, operators.Count).Select(index => new Operators
                    {
                        Name = operators[index].Name,
                        SellNet = operators[index].SellNet,
                        SumSellQty = operators[index].SumSellQty
                    }).ToArray());
                }
                else
                    return Task.FromResult<Operators[]>(null);
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID };
                var operators = db.Query<Operators>("Select FIRST 10 " +
                                                "o.OpID, " +
                                                "o.Name, " +
                                                "sum(vs.sell_net)SellNet,  " +
                                                "sum(vs.SELL_QTY)SumSellQty  " +
                                                "FROM VEOPS o  " +
                                                "inner Join vetrans vt  " +
                                                "on o.OPID = vt.OPID  " +
                                                "inner join vesales vs  " +
                                                "on vs.TRANSID = vt.TRANSID  " +
                                                "where vt.DateTime>=@StartDate and vt.DateTime <= @EndDate AND vt.BRANCHID=@BranchID " +
                                                "Group by o.OPID, o.NAME  " +
                                                "Order by SellNet desc", parameters).ToList();
                if (operators.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, operators.Count).Select(index => new Operators
                    {
                        Name = operators[index].Name,
                        SellNet = operators[index].SellNet,
                        SumSellQty = operators[index].SumSellQty
                    }).ToArray());
                }
                else
                    return Task.FromResult<Operators[]>(null);
            }
        }
    }
}
