using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_V4.Data
{
    public class TakingsService
    {
        public Task<Takings[]> GetTakingsRangeAsync(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
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

            //where no end date is chosen.. ie on the first render of the dashboard.
            if (BranchID == 0)
            {
                var parameters = new { StartDate, EndDate };
                var sales = db.QueryFirst<Takings>("Select " +
                                                        "sum(vs.sell_net)SellNet, " +
                                                        "sum(vs.sell_net + vs.sell_vat)SellVat, " +
                                                        "sum(vs.cost_net)CostNet, " +
                                                        "sum(vs.cost_net + vs.cost_vat)CostVat, " +
                                                        "sum(vs.sell_net - vs.cost_net)Profit, " +
                                                        "sum((vs.sell_net + vs.sell_vat) - (vs.cost_net + vs.cost_vat))ProfitVat, " +
                                                        "sum(vs.SELL_QTY)SumSellQty, " +
                                                        "count(distinct headid)Visits, " +
                                                        "sum(vs.SELL_QTY) / count(distinct headid) as AverageBasket, " +
                                                        "sum(vs.sell_net) / count(distinct headid) as AverageSpend, " +
                                                        "sum(vs.sell_net + vs.sell_vat) / count(distinct headid) as AverageSpendVAT " +
                                                        "from vesales vs " +
                                                        "inner join vetrans vt " +
                                                        "on vt.TRANSID = vs.TRANSID " +
                                                        "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                                                        "where vt.DateTime >= @StartDate and vt.DateTime <= @EndDate and vt.TransType in ('CS', 'RT', 'CO', 'DV','SO', 'XD', 'DS')", parameters);

                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new Takings
                {
                    CostNet = Math.Round(sales.CostNet, 2, MidpointRounding.AwayFromZero),
                    CostVat = Math.Round(sales.CostVat, 2, MidpointRounding.AwayFromZero),
                    SellNet = Math.Round(sales.SellNet, 2, MidpointRounding.AwayFromZero),
                    SellVAT = Math.Round(sales.SellVAT, 2, MidpointRounding.AwayFromZero),
                    Profit = Math.Round(sales.Profit, 2, MidpointRounding.AwayFromZero),
                    ProfitVat = Math.Round(sales.ProfitVat, 2, MidpointRounding.AwayFromZero),
                    SumSellQty = Math.Round(sales.SumSellQty, 2, MidpointRounding.AwayFromZero),
                    Visits = sales.Visits,
                    AverageBasket = Math.Round(sales.AverageBasket, 2, MidpointRounding.AwayFromZero),
                    AverageSpend = Math.Round(sales.AverageSpend, 2, MidpointRounding.AwayFromZero),
                    AverageSpendVAT = Math.Round(sales.AverageSpendVAT, 2, MidpointRounding.AwayFromZero)
                }).ToArray());
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID };
                var sales = db.QueryFirst<Takings>("Select " +
                                                    "sum(vs.sell_net)SellNet, " +
                                                    "sum(vs.sell_net + vs.sell_vat)SellVat, " +
                                                    "sum(vs.cost_net)CostNet, " +
                                                    "sum(vs.cost_net + vs.cost_vat)CostVat, " +
                                                    "sum(vs.sell_net - vs.cost_net)Profit, " +
                                                    "sum((vs.sell_net + vs.sell_vat) - (vs.cost_net + vs.cost_vat))ProfitVat, " +
                                                    "sum(vs.SELL_QTY)SumSellQty, " +
                                                    "count(distinct headid)Visits, " +
                                                    "sum(vs.SELL_QTY) / count(distinct headid) as AverageBasket, " +
                                                    "sum(vs.sell_net) / count(distinct headid) as AverageSpend, " +
                                                    "sum(vs.sell_net + vs.sell_vat) / count(distinct headid) as AverageSpendVAT " +
                                                    "from vesales vs " +
                                                    "inner join vetrans vt " +
                                                    "on vt.TRANSID = vs.TRANSID " +
                                                    "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                                                    "where vt.DateTime >= @StartDate and vt.DateTime <= @EndDate AND vt.BRANCHID=@BranchID and vt.TransType in ('CS', 'RT', 'CO', 'DV','SO', 'XD', 'DS')", parameters);

                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new Takings
                {
                    CostNet = Math.Round(sales.CostNet, 2, MidpointRounding.AwayFromZero),
                    CostVat = Math.Round(sales.CostVat, 2, MidpointRounding.AwayFromZero),
                    SellNet = Math.Round(sales.SellNet, 2, MidpointRounding.AwayFromZero),
                    SellVAT = Math.Round(sales.SellVAT, 2, MidpointRounding.AwayFromZero),
                    Profit = Math.Round(sales.Profit, 2, MidpointRounding.AwayFromZero),
                    ProfitVat = Math.Round(sales.ProfitVat, 2, MidpointRounding.AwayFromZero),
                    SumSellQty = Math.Round(sales.SumSellQty, 2, MidpointRounding.AwayFromZero),
                    Visits = sales.Visits,
                    AverageBasket = Math.Round(sales.AverageBasket, 2, MidpointRounding.AwayFromZero),
                    AverageSpend = Math.Round(sales.AverageSpend, 2, MidpointRounding.AwayFromZero),
                    AverageSpendVAT = Math.Round(sales.AverageSpendVAT, 2, MidpointRounding.AwayFromZero)
                }).ToArray());
            }
        }
        public Task<Takings[]> GetDailyTakingsAsync(DateTime? StartDate, Int64 BranchID = default)
        {
            IDbConnection db = new FbConnection(Startup.VectorConnection);

            if (BranchID == 0)
            {
                var parameters = new { StartDate };
                var sales = db.QueryFirst<Takings>("Select " +
                                                        "sum(vs.sell_net)SellNet, " +
                                                        "sum(vs.sell_net + vs.sell_vat)SellVat, " +
                                                        "sum(vs.cost_net)CostNet, " +
                                                        "sum(vs.cost_net + vs.cost_vat)CostVat, " +
                                                        "sum(vs.sell_net - vs.cost_net)Profit, " +
                                                        "sum((vs.sell_net + vs.sell_vat) - (vs.cost_net + vs.cost_vat))ProfitVat, " +
                                                        "sum(vs.SELL_QTY)SumSellQty, " +
                                                        "count(distinct headid)Visits, " +
                                                        "sum(vs.SELL_QTY) / count(distinct headid) as AverageBasket, " +
                                                        "sum(vs.sell_net) / count(distinct headid) as AverageSpend, " +
                                                        "sum(vs.sell_net + vs.sell_vat) / count(distinct headid) as AverageSpendVAT " +
                                                        "from vesales vs " +
                                                        "inner join vetrans vt " +
                                                        "on vt.TRANSID = vs.TRANSID " +
                                                        "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                                                        "where vt.DateTime>=@StartDate and vt.TransType in ('CS', 'RT', 'CO', 'DV','SO', 'XD', 'DS')", parameters);
                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new Takings
                {
                    CostNet = Math.Round(sales.CostNet, 2, MidpointRounding.AwayFromZero),
                    CostVat = Math.Round(sales.CostVat, 2, MidpointRounding.AwayFromZero),
                    SellNet = Math.Round(sales.SellNet, 2, MidpointRounding.AwayFromZero),
                    SellVAT = Math.Round(sales.SellVAT, 2, MidpointRounding.AwayFromZero),
                    Profit = Math.Round(sales.Profit, 2, MidpointRounding.AwayFromZero),
                    ProfitVat = Math.Round(sales.ProfitVat, 2, MidpointRounding.AwayFromZero),
                    SumSellQty = Math.Round(sales.SumSellQty, 2, MidpointRounding.AwayFromZero),
                    Visits = sales.Visits,
                    AverageBasket = Math.Round(sales.AverageBasket, 2, MidpointRounding.AwayFromZero),
                    AverageSpend = Math.Round(sales.AverageSpend, 2, MidpointRounding.AwayFromZero),
                    AverageSpendVAT = Math.Round(sales.AverageSpendVAT, 2, MidpointRounding.AwayFromZero)
                }).ToArray());
            }
            else
            {
                var parameters = new { StartDate, BranchID };
                var sales = db.QueryFirst<Takings>("Select " +
                                        "sum(vs.sell_net)SellNet, " +
                                        "sum(vs.sell_net + vs.sell_vat)SellVat, " +
                                        "sum(vs.cost_net)CostNet, " +
                                        "sum(vs.cost_net + vs.cost_vat)CostVat, " +
                                        "sum(vs.sell_net - vs.cost_net)Profit, " +
                                        "sum((vs.sell_net + vs.sell_vat) - (vs.cost_net + vs.cost_vat))ProfitVat, " +
                                        "sum(vs.SELL_QTY)SumSellQty, " +
                                        "count(distinct headid)Visits, " +
                                        "sum(vs.SELL_QTY) / count(distinct headid) as AverageBasket, " +
                                        "sum(vs.sell_net) / count(distinct headid) as AverageSpend, " +
                                        "sum(vs.sell_net + vs.sell_vat) / count(distinct headid) as AverageSpendVAT " +
                                        "from vesales vs " +
                                        "inner join vetrans vt " +
                                        "on vt.TRANSID = vs.TRANSID " +
                                        "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                                        "where vt.DateTime>=@StartDate AND vt.BRANCHID=@BranchID and vt.TransType in ('CS', 'RT', 'CO', 'DV','SO', 'XD', 'DS')", parameters);
                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new Takings
                {
                    CostNet = Math.Round(sales.CostNet, 2, MidpointRounding.AwayFromZero),
                    CostVat = Math.Round(sales.CostVat, 2, MidpointRounding.AwayFromZero),
                    SellNet = Math.Round(sales.SellNet, 2, MidpointRounding.AwayFromZero),
                    SellVAT = Math.Round(sales.SellVAT, 2, MidpointRounding.AwayFromZero),
                    Profit = Math.Round(sales.Profit, 2, MidpointRounding.AwayFromZero),
                    ProfitVat = Math.Round(sales.ProfitVat, 2, MidpointRounding.AwayFromZero),
                    SumSellQty = Math.Round(sales.SumSellQty, 2, MidpointRounding.AwayFromZero),
                    Visits = sales.Visits,
                    AverageBasket = Math.Round(sales.AverageBasket, 2, MidpointRounding.AwayFromZero),
                    AverageSpend = Math.Round(sales.AverageSpend, 2, MidpointRounding.AwayFromZero),
                    AverageSpendVAT = Math.Round(sales.AverageSpendVAT, 2, MidpointRounding.AwayFromZero)
                }).ToArray());
            }
        }

        public Task<Takings[]> GetTakingsTrendAsync(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
        {
            IDbConnection db = new FbConnection(Startup.VectorConnection);
            if (End == DateTime.MinValue)
            {
                End = DateTime.Now;
            }
 
            DateTime? _starts = Start.HasValue ? Start.Value.DateTime : DateTime.Today.AddDays(-6 - (int)DateTime.Now.DayOfWeek);
            DateTime? _ends = End.HasValue ? End.Value.DateTime : DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);

            DateTime StartDate = (DateTime)_starts;
            DateTime EndDate = (DateTime)_ends;

            //where no end date is chosen.. ie on the first render of the dashboard.
            if (BranchID == 0)
            {
                var parameters = new { StartDate, EndDate };
                var sales = db.Query<Takings>("Select " +
                                                        "cast(vt.DATETIME as Date) TransDate, " + 
                                                        "sum(vs.sell_net)SellNet, " +
                                                        "sum(vs.sell_net + vs.sell_vat)SellVat, " +
                                                        "sum(vs.cost_net)CostNet, " +
                                                        "sum(vs.cost_net + vs.cost_vat)CostVat, " +
                                                        "sum(vs.sell_net - vs.cost_net)Profit, " +
                                                        "sum((vs.sell_net + vs.sell_vat) - (vs.cost_net + vs.cost_vat))ProfitVat, " +
                                                        "sum(vs.SELL_QTY)SumSellQty, " +
                                                        "count(distinct headid)Visits, " +
                                                        "sum(vs.SELL_QTY) / count(distinct headid) as AverageBasket, " +
                                                        "sum(vs.sell_net) / count(distinct headid) as AverageSpend, " +
                                                        "sum(vs.sell_net + vs.sell_vat) / count(distinct headid) as AverageSpendVAT " +
                                                        "from vesales vs " +
                                                        "inner join vetrans vt " +
                                                        "on vt.TRANSID = vs.TRANSID " +
                                                        "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                                                        "where vt.DateTime >= @StartDate and vt.DateTime <= @EndDate and vt.TransType in ('CS', 'RT', 'CO', 'DV','SO', 'XD', 'DS') " +
                                                        "Group by cast(vt.DATETIME as Date)", parameters).ToList();

                if (sales.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, sales.Count).Select(index => new Takings
                    {
                        TransDate = sales[index].TransDate,
                        CostNet = Math.Round(sales[index].CostNet, 2, MidpointRounding.AwayFromZero),
                        CostVat = Math.Round(sales[index].CostVat, 2, MidpointRounding.AwayFromZero),
                        SellNet = Math.Round(sales[index].SellNet, 2, MidpointRounding.AwayFromZero),
                        SellVAT = Math.Round(sales[index].SellVAT, 2, MidpointRounding.AwayFromZero),
                        Profit = Math.Round(sales[index].Profit, 2, MidpointRounding.AwayFromZero),
                        ProfitVat = Math.Round(sales[index].ProfitVat, 2, MidpointRounding.AwayFromZero),
                        SumSellQty = Math.Round(sales[index].SumSellQty, 2, MidpointRounding.AwayFromZero),
                        Visits = sales[index].Visits,
                        AverageBasket = Math.Round(sales[index].AverageBasket, 2, MidpointRounding.AwayFromZero),
                        AverageSpend = Math.Round(sales[index].AverageSpend, 2, MidpointRounding.AwayFromZero),
                        AverageSpendVAT = Math.Round(sales[index].AverageSpendVAT, 2, MidpointRounding.AwayFromZero)
                    }).ToArray());
                }
                else
                    return Task.FromResult<Takings[]>(null);
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID };
                var sales = db.Query<Takings>("Select " +
                                                    "cast(vt.DATETIME as Date) TransDate, " +
                                                    "sum(vs.sell_net)SellNet, " +
                                                    "sum(vs.sell_net + vs.sell_vat)SellVat, " +
                                                    "sum(vs.cost_net)CostNet, " +
                                                    "sum(vs.cost_net + vs.cost_vat)CostVat, " +
                                                    "sum(vs.sell_net - vs.cost_net)Profit, " +
                                                    "sum((vs.sell_net + vs.sell_vat) - (vs.cost_net + vs.cost_vat))ProfitVat, " +
                                                    "sum(vs.SELL_QTY)SumSellQty, " +
                                                    "count(distinct headid)Visits, " +
                                                    "sum(vs.SELL_QTY) / count(distinct headid) as AverageBasket, " +
                                                    "sum(vs.sell_net) / count(distinct headid) as AverageSpend, " +
                                                    "sum(vs.sell_net + vs.sell_vat) / count(distinct headid) as AverageSpendVAT " +
                                                    "from vesales vs " +
                                                    "inner join vetrans vt " +
                                                    "on vt.TRANSID = vs.TRANSID " +
                                                    "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                                                    "where vt.DateTime >= @StartDate AND vt.DateTime <= @EndDate AND vt.BRANCHID=@BranchID and vt.TransType in ('CS', 'RT', 'CO', 'DV','SO', 'XD', 'DS') " +
                                                    "Group by cast(vt.DATETIME as Date)", parameters).ToList(); 

                if (sales.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, sales.Count).Select(index => new Takings
                    {
                        TransDate = sales[index].TransDate,
                        CostNet = Math.Round(sales[index].CostNet, 2, MidpointRounding.AwayFromZero),
                        CostVat = Math.Round(sales[index].CostVat, 2, MidpointRounding.AwayFromZero),
                        SellNet = Math.Round(sales[index].SellNet, 2, MidpointRounding.AwayFromZero),
                        SellVAT = Math.Round(sales[index].SellVAT, 2, MidpointRounding.AwayFromZero),
                        Profit = Math.Round(sales[index].Profit, 2, MidpointRounding.AwayFromZero),
                        ProfitVat = Math.Round(sales[index].ProfitVat, 2, MidpointRounding.AwayFromZero),
                        SumSellQty = Math.Round(sales[index].SumSellQty, 2, MidpointRounding.AwayFromZero),
                        Visits = sales[index].Visits,
                        AverageBasket = Math.Round(sales[index].AverageBasket, 2, MidpointRounding.AwayFromZero),
                        AverageSpend = Math.Round(sales[index].AverageSpend, 2, MidpointRounding.AwayFromZero),
                        AverageSpendVAT = Math.Round(sales[index].AverageSpendVAT, 2, MidpointRounding.AwayFromZero)
                    }).ToArray());
                }
                else
                    return Task.FromResult<Takings[]>(null);
            }
        }
    }
}
