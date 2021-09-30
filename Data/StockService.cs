using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

//This takes too long for the default daily dashboard.
namespace IIS_V4.Data
{
    public class StockService
    {
        public Task<Stock[]> GetStockValue()
        {
            IDbConnection db = new FbConnection(Startup.VectorConnection);
            var orders = db.QueryFirst<Stock>("SELECT " +
                                              "sum(vesuppstock.cost_net * vebranchstock.quantity) CostNet, " +
                                              "sum(vestock.QUANTITY) " +
                                              "from vestock " +
                                              "join vebranchstock on vestock.stockid = vebranchstock.stockid " +
                                              "join vesuppstock on (vesuppstock.stockid = vestock.stockid and vesuppstock.preferred = 1)");
            return Task.FromResult(Enumerable.Range(0, 1).Select(index => new Stock
            {
                CostNet = Math.Round(orders.CostNet, 2, MidpointRounding.AwayFromZero),
            }).ToArray());
        }
    }
}
