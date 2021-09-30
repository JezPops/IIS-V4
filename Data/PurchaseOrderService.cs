using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_V4.Data
{
    public class PurchaseOrderService
    {
        public Task<PurchaseOrders[]> GetPurchaseOrders(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
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
                var orders = db.QueryFirst<PurchaseOrders>(
                                "select Count(*), " +
                                "sum(vesupporderitems.cost_net) costnet, " +
                                "sum(vesupporderitems.cost_net + vesupporderitems.cost_vat) costvat " +
                                "from VESUPPORDERS " +
                                "inner join VESUPPS on VESUPPORDERS.SUPPID = VESUPPS.SUPPID " +
                                "join vesupporderitems on vesupporders.supporderid = vesupporderitems.supporderid " +
                                "where VESUPPORDERS.ORDERDATE >= @StartDate and  VESUPPORDERS.ORDERDATE <= @EndDate", parameters);
                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new PurchaseOrders
                {
                    CostNet = Math.Round(orders.CostNet, 2, MidpointRounding.AwayFromZero),
                    Count = orders.Count,
                }).ToArray());
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID };
                var orders = db.QueryFirst<PurchaseOrders>(
                                "select Count(*), " +
                                "sum(vesupporderitems.cost_net) costnet, " +
                                "sum(vesupporderitems.cost_net + vesupporderitems.cost_vat) costvat " +
                                "from VESUPPORDERS " +
                                "inner join VESUPPS on VESUPPORDERS.SUPPID = VESUPPS.SUPPID " +
                                "join vesupporderitems on vesupporders.supporderid = vesupporderitems.supporderid " +
                                "where VESUPPORDERS.ORDERDATE >= @StartDate and VESUPPORDERS.ORDERDATE <= @EndDate AND VESUPPORDERS.BRANCHID=@BranchID", parameters);
                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new PurchaseOrders
                {
                    CostNet = Math.Round(orders.CostNet, 2, MidpointRounding.AwayFromZero),
                    Count = orders.Count,
                }).ToArray());
            }
        }
    }
}
