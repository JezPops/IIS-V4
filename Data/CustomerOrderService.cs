using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_V4.Data
{
    public class CustomerOrderService
    {
        public Task<CustomerOrders[]> GetCustomerOrders(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
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
                var orders = db.QueryFirst<CustomerOrders>("Select Count(*), " +
                                             "sum(co.selling_net) as SellNet " +
                                             "from vesales vs " +
                                             "inner join vetrans vt " +
                                             "on vt.TRANSID = vs.TRANSID " +
                                             "inner join vecusttrans vc " +
                                             "on vc.TRANSID = vt.TRANSID " +
                                             "inner join VECUSTORD co " +
                                             "on co.TRANSID = vc.TRANSID " +
                                             "where vt.TRANSTYPE in ('SO', 'CO', 'DS', 'XD') " +
                                             "and vt.DateTime>=@StartDate and vt.DateTime <= @EndDate", parameters);
                //(vecustord.qty - vecustord.qty_collected - vecustord.QTY_CANCELLED) = 0'??
                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new CustomerOrders
                {
                    SellNet = Math.Round(orders.SellNet, 2, MidpointRounding.AwayFromZero),
                    Count = orders.Count,
                }).ToArray());
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID };
                var orders = db.QueryFirst<CustomerOrders>("Select Count(*), " +
                                             "sum(co.selling_net) as SellNet " +
                                             "from vesales vs " +
                                             "inner join vetrans vt " +
                                             "on vt.TRANSID = vs.TRANSID " +
                                             "inner join vecusttrans vc " +
                                             "on vc.TRANSID = vt.TRANSID " +
                                             "inner join VECUSTORD co " +
                                             "on co.TRANSID = vc.TRANSID " +
                                             "where vt.TRANSTYPE in ('SO', 'CO', 'DS', 'XD') " +
                                             "and vt.DateTime>=@StartDate and vt.DateTime <= @EndDate AND vt.BRANCHID=@BranchID", parameters);
                //(vecustord.qty - vecustord.qty_collected - vecustord.QTY_CANCELLED) = 0'??
                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new CustomerOrders
                {
                    SellNet = Math.Round(orders.SellNet, 2, MidpointRounding.AwayFromZero),
                    Count = orders.Count,
                }).ToArray());
            }
        }
    }
}
