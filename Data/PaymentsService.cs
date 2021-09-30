using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_V4.Data
{
    public class PaymentsService
    {
        public Task<Payments[]> GetPaymentsSummary(DateTime StartDate, Int64 BranchID = default)
        {
            IDbConnection db = new FbConnection(Startup.VectorConnection);
            var parameters = new { StartDate };
            var payments = db.QueryFirst<Payments>(
                                    "select " +
                                    "Count(*), " +
                                    "sum(vepaytrans.amount)PayAmount " +
                                    "FROM VETRANS " +
                                    "JOIN VEPAYTRANS ON VETRANS.TRANSID = VEPAYTRANS.TRANSID " +
                                    "WHERE VETRANS.DATETIME >= @StartDate", parameters);
            return Task.FromResult(Enumerable.Range(0, 1).Select(index => new Payments
            {
                PayAmount = Math.Round(payments.PayAmount, 2, MidpointRounding.AwayFromZero),
                Count = payments.Count,
            }).ToArray());
        }
    }
}
