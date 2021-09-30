using Dapper;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_V4.Data
{
    public class SalesService
    {
        public Task<Sales[]> GetSalesTrendAsync(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
        {
            IDbConnection db = new FbConnection(Startup.VectorConnection);
            if (End == DateTime.MinValue)
            {
                End = DateTime.Now;
            }

            DateTime? _starts = Start.HasValue ? Start.Value.DateTime : DateTime.Today.AddDays(-6 - (int)DateTime.Now.DayOfWeek);
            DateTime? _ends = End.HasValue ? End.Value.DateTime : DateTime.Now;

            DateTime StartDate = (DateTime)_starts;
            DateTime EndDate = (DateTime)_ends;

            if (BranchID == 0)
            {
                var parameters = new { StartDate, EndDate };
                var sales = db.Query<Sales>("Select cast(vt.DATETIME as Date) TransDate, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHRETTOT, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end)  ACCTSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTRETTOT,  " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSOTOT, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTSOTOT,  " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDEPTSPENT, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDELTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCOTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCOTOT, " +
                "count(distinct case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vt.headid else null end) trancnt, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHRETNET, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET else 0 end)  ACCTSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTRETNET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSONET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTSONET, " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET else 0 end) ACCTDEPTSPENTNET, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTDELNET, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHCONET,  " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTCONET,  " +
                "sum(vs.COST_NET) COSTNETTOT,  " +
                "SUM(case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vs.SELL_QTY else 0 end) SUMSELLQTY,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCHRDTOT,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCHRDTOT " +
                "from VETRANS vt " +
                "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                "join VESALES vs on vt.TRANSID = vs.TRANSID " +
                "where vt.DateTime >= @StartDate and vt.DateTime <= @EndDate " +
                "group by cast(vt.DateTime as Date)", parameters).ToList();
                if (sales.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, sales.Count).Select(index => new Sales
                    {
                        TransDate = sales[index].TransDate,
                        CASHSELLTOT = Math.Round(sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                        CASHRETTOT = Math.Round(sales[index].CASHRETTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTSELLTOT = Math.Round(sales[index].ACCTSELLTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTRETTOT = Math.Round(sales[index].ACCTRETTOT, 2, MidpointRounding.AwayFromZero),
                        CASHSOTOT = Math.Round(sales[index].CASHSOTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTSOTOT = Math.Round(sales[index].ACCTSOTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTDEPTSPENT = Math.Round(sales[index].ACCTDEPTSPENT, 2, MidpointRounding.AwayFromZero),
                        ACCTDELTOT = Math.Round(sales[index].ACCTDELTOT, 2, MidpointRounding.AwayFromZero),
                        CASHCOTOT = Math.Round(sales[index].CASHCOTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTCOTOT = Math.Round(sales[index].ACCTCOTOT, 2, MidpointRounding.AwayFromZero),
                        CASHSELLNET = Math.Round(sales[index].CASHSELLNET, 2, MidpointRounding.AwayFromZero),
                        CASHRETNET = Math.Round(sales[index].CASHRETNET, 2, MidpointRounding.AwayFromZero),
                        ACCTSELLNET = Math.Round(sales[index].ACCTSELLNET, 2, MidpointRounding.AwayFromZero),
                        ACCTRETNET = Math.Round(sales[index].ACCTRETNET, 2, MidpointRounding.AwayFromZero),
                        CASHSONET = Math.Round(sales[index].CASHSONET, 2, MidpointRounding.AwayFromZero),
                        ACCTSONET = Math.Round(sales[index].ACCTSONET, 2, MidpointRounding.AwayFromZero),
                        ACCTDEPTSPENTNET = Math.Round(sales[index].ACCTDEPTSPENTNET, 2, MidpointRounding.AwayFromZero),
                        ACCTDELNET = Math.Round(sales[index].ACCTDELNET, 2, MidpointRounding.AwayFromZero),
                        CASHCONET = Math.Round(sales[index].CASHCONET, 2, MidpointRounding.AwayFromZero),
                        ACCTCONET = Math.Round(sales[index].ACCTCONET, 2, MidpointRounding.AwayFromZero),
                        COSTNETTOT = Math.Round(sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                        SUMSELLQTY = Math.Round(sales[index].SUMSELLQTY, 2, MidpointRounding.AwayFromZero),
                        CASHCHRDTOT = Math.Round(sales[index].CASHCHRDTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTCHRDTOT = Math.Round(sales[index].ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),
                        TOTAL = Math.Round(sales[index].ACCTSELLNET + sales[index].CASHSELLNET + sales[index].CASHRETNET + sales[index].ACCTRETNET + sales[index].ACCTDELNET + sales[index].CASHCHRDTOT + sales[index].ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),
                        PROFIT = Math.Round((sales[index].ACCTSELLNET + sales[index].CASHSELLNET + sales[index].CASHRETNET + sales[index].ACCTRETNET + sales[index].ACCTDELNET + sales[index].CASHCHRDTOT + sales[index].ACCTCHRDTOT) - sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero)
                    }).ToArray());
                }
                else
                    return Task.FromResult<Sales[]>(null);
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID };
                var sales = db.Query<Sales>("Select cast(vt.DATETIME as Date) TransDate, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHRETTOT, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end)  ACCTSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTRETTOT,  " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSOTOT, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTSOTOT,  " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDEPTSPENT, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDELTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCOTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCOTOT, " +
                "count(distinct case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vt.headid else null end) trancnt, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHRETNET, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET else 0 end)  ACCTSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTRETNET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSONET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTSONET, " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET else 0 end) ACCTDEPTSPENTNET, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTDELNET, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHCONET,  " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTCONET,  " +
                "sum(vs.COST_NET) COSTNETTOT,  " +
                "SUM(case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vs.SELL_QTY else 0 end) SUMSELLQTY,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCHRDTOT,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCHRDTOT " + 
                "from VETRANS vt " +
                "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                "join VESALES vs on vt.TRANSID = vs.TRANSID " +
                "where vt.DateTime >= @StartDate and vt.DateTime <= @EndDate and vt.BRANCHID=@BranchID " +
                "group by cast(vt.DateTime as Date)", parameters).ToList();
                if (sales.Count > 0)
                {
                    return Task.FromResult(Enumerable.Range(0, sales.Count).Select(index => new Sales
                    {
                        TransDate = sales[index].TransDate,
                        CASHSELLTOT = Math.Round(sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                        CASHRETTOT = Math.Round(sales[index].CASHRETTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTSELLTOT = Math.Round(sales[index].ACCTSELLTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTRETTOT = Math.Round(sales[index].ACCTRETTOT, 2, MidpointRounding.AwayFromZero),
                        CASHSOTOT = Math.Round(sales[index].CASHSOTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTSOTOT = Math.Round(sales[index].ACCTSOTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTDEPTSPENT = Math.Round(sales[index].ACCTDEPTSPENT, 2, MidpointRounding.AwayFromZero),
                        ACCTDELTOT = Math.Round(sales[index].ACCTDELTOT, 2, MidpointRounding.AwayFromZero),
                        CASHCOTOT = Math.Round(sales[index].CASHCOTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTCOTOT = Math.Round(sales[index].ACCTCOTOT, 2, MidpointRounding.AwayFromZero),
                        CASHSELLNET = Math.Round(sales[index].CASHSELLNET, 2, MidpointRounding.AwayFromZero),
                        CASHRETNET = Math.Round(sales[index].CASHRETNET, 2, MidpointRounding.AwayFromZero),
                        ACCTSELLNET = Math.Round(sales[index].ACCTSELLNET, 2, MidpointRounding.AwayFromZero),
                        ACCTRETNET = Math.Round(sales[index].ACCTRETNET, 2, MidpointRounding.AwayFromZero),
                        CASHSONET = Math.Round(sales[index].CASHSONET, 2, MidpointRounding.AwayFromZero),
                        ACCTSONET = Math.Round(sales[index].ACCTSONET, 2, MidpointRounding.AwayFromZero),
                        ACCTDEPTSPENTNET = Math.Round(sales[index].ACCTDEPTSPENTNET, 2, MidpointRounding.AwayFromZero),
                        ACCTDELNET = Math.Round(sales[index].ACCTDELNET, 2, MidpointRounding.AwayFromZero),
                        CASHCONET = Math.Round(sales[index].CASHCONET, 2, MidpointRounding.AwayFromZero),
                        ACCTCONET = Math.Round(sales[index].ACCTCONET, 2, MidpointRounding.AwayFromZero),
                        COSTNETTOT = Math.Round(sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                        SUMSELLQTY = Math.Round(sales[index].SUMSELLQTY, 2, MidpointRounding.AwayFromZero),
                        CASHCHRDTOT = Math.Round(sales[index].CASHCHRDTOT, 2, MidpointRounding.AwayFromZero),
                        ACCTCHRDTOT = Math.Round(sales[index].ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),
                        TOTAL = Math.Round(sales[index].ACCTSELLNET + sales[index].CASHSELLNET + sales[index].CASHRETNET + sales[index].ACCTRETNET + sales[index].ACCTDELNET + sales[index].CASHCHRDTOT + sales[index].ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),
                        PROFIT = Math.Round((sales[index].ACCTSELLNET + sales[index].CASHSELLNET + sales[index].CASHRETNET + sales[index].ACCTRETNET + sales[index].ACCTDELNET + sales[index].CASHCHRDTOT + sales[index].ACCTCHRDTOT) - sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero)
                    }).ToArray());
                }
                else
                    return Task.FromResult<Sales[]>(null);
            }
        }
        public Task<Sales[]> GetSalesAsync(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
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
                var sales = db.QueryFirst<Sales>("Select " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHRETTOT, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end)  ACCTSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTRETTOT,  " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSOTOT, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTSOTOT,  " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDEPTSPENT, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDELTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCOTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCOTOT, " +
                "count(distinct case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vt.headid else null end) trancnt, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHRETNET, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET else 0 end)  ACCTSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTRETNET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSONET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTSONET, " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET else 0 end) ACCTDEPTSPENTNET, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTDELNET, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHCONET,  " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTCONET,  " +
                "sum(vs.COST_NET) COSTNETTOT,  " +
                "SUM(case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vs.SELL_QTY else 0 end) SUMSELLQTY,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCHRDTOT,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCHRDTOT " + 
                "from VETRANS vt " +
                "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                "join VESALES vs on vt.TRANSID = vs.TRANSID " +
                "where vt.DateTime >= @StartDate and vt.DateTime <= @EndDate", parameters);

                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new Sales
                {
                    CASHSELLTOT = Math.Round(sales.COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                    CASHRETTOT = Math.Round(sales.CASHRETTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTSELLTOT = Math.Round(sales.ACCTSELLTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTRETTOT = Math.Round(sales.ACCTRETTOT, 2, MidpointRounding.AwayFromZero),
                    CASHSOTOT = Math.Round(sales.CASHSOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTSOTOT = Math.Round(sales.ACCTSOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTDEPTSPENT = Math.Round(sales.ACCTDEPTSPENT, 2, MidpointRounding.AwayFromZero),
                    ACCTDELTOT = Math.Round(sales.ACCTDELTOT, 2, MidpointRounding.AwayFromZero),
                    CASHCOTOT = Math.Round(sales.CASHCOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTCOTOT = Math.Round(sales.ACCTCOTOT, 2, MidpointRounding.AwayFromZero),
                    CASHSELLNET = Math.Round(sales.CASHSELLNET, 2, MidpointRounding.AwayFromZero),
                    CASHRETNET = Math.Round(sales.CASHRETNET, 2, MidpointRounding.AwayFromZero),
                    ACCTSELLNET = Math.Round(sales.ACCTSELLNET, 2, MidpointRounding.AwayFromZero),
                    ACCTRETNET = Math.Round(sales.ACCTRETNET, 2, MidpointRounding.AwayFromZero),
                    CASHSONET = Math.Round(sales.CASHSONET, 2, MidpointRounding.AwayFromZero),
                    ACCTSONET = Math.Round(sales.ACCTSONET, 2, MidpointRounding.AwayFromZero),
                    ACCTDEPTSPENTNET = Math.Round(sales.ACCTDEPTSPENTNET, 2, MidpointRounding.AwayFromZero),
                    ACCTDELNET = Math.Round(sales.ACCTDELNET, 2, MidpointRounding.AwayFromZero),
                    CASHCONET = Math.Round(sales.CASHCONET, 2, MidpointRounding.AwayFromZero),
                    ACCTCONET = Math.Round(sales.ACCTCONET, 2, MidpointRounding.AwayFromZero),
                    COSTNETTOT = Math.Round(sales.COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                    SUMSELLQTY = Math.Round(sales.SUMSELLQTY, 2, MidpointRounding.AwayFromZero),
                    CASHCHRDTOT = Math.Round(sales.CASHCHRDTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTCHRDTOT = Math.Round(sales.ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),
                    TOTAL = Math.Round(sales.ACCTSELLNET + sales.CASHSELLNET + sales.CASHRETNET + sales.ACCTRETNET + sales.ACCTDELNET + sales.CASHCHRDTOT + sales.ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),
                    PROFIT = Math.Round((sales.ACCTSELLNET + sales.CASHSELLNET + sales.CASHRETNET + sales.ACCTRETNET + sales.ACCTDELNET + sales.CASHCHRDTOT + sales.ACCTCHRDTOT) - sales.COSTNETTOT, 2, MidpointRounding.AwayFromZero)
                }).ToArray());
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID };
                var sales = db.QueryFirst<Sales>("Select " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHRETTOT, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end)  ACCTSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTRETTOT,  " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSOTOT, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTSOTOT,  " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDEPTSPENT, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDELTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCOTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCOTOT, " +
                "count(distinct case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vt.headid else null end) trancnt, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHRETNET, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET else 0 end)  ACCTSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTRETNET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSONET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTSONET, " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET else 0 end) ACCTDEPTSPENTNET, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTDELNET, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHCONET,  " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTCONET,  " +
                "sum(vs.COST_NET) COSTNETTOT,  " +
                "SUM(case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vs.SELL_QTY else 0 end) SUMSELLQTY,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCHRDTOT,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCHRDTOT " + 
                "from VETRANS vt " +
                "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                "join VESALES vs on vt.TRANSID = vs.TRANSID " +
                "where vt.DateTime >= @StartDate and vt.DateTime <= @EndDate and vt.BRANCHID=@BranchID", parameters);

                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new Sales
                {
                    CASHSELLTOT = Math.Round(sales.COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                    CASHRETTOT = Math.Round(sales.CASHRETTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTSELLTOT = Math.Round(sales.ACCTSELLTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTRETTOT = Math.Round(sales.ACCTRETTOT, 2, MidpointRounding.AwayFromZero),
                    CASHSOTOT = Math.Round(sales.CASHSOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTSOTOT = Math.Round(sales.ACCTSOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTDEPTSPENT = Math.Round(sales.ACCTDEPTSPENT, 2, MidpointRounding.AwayFromZero),
                    ACCTDELTOT = Math.Round(sales.ACCTDELTOT, 2, MidpointRounding.AwayFromZero),
                    CASHCOTOT = Math.Round(sales.CASHCOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTCOTOT = Math.Round(sales.ACCTCOTOT, 2, MidpointRounding.AwayFromZero),
                    CASHSELLNET = Math.Round(sales.CASHSELLNET, 2, MidpointRounding.AwayFromZero),
                    CASHRETNET = Math.Round(sales.CASHRETNET, 2, MidpointRounding.AwayFromZero),
                    ACCTSELLNET = Math.Round(sales.ACCTSELLNET, 2, MidpointRounding.AwayFromZero),
                    ACCTRETNET = Math.Round(sales.ACCTRETNET, 2, MidpointRounding.AwayFromZero),
                    CASHSONET = Math.Round(sales.CASHSONET, 2, MidpointRounding.AwayFromZero),
                    ACCTSONET = Math.Round(sales.ACCTSONET, 2, MidpointRounding.AwayFromZero),
                    ACCTDEPTSPENTNET = Math.Round(sales.ACCTDEPTSPENTNET, 2, MidpointRounding.AwayFromZero),
                    ACCTDELNET = Math.Round(sales.ACCTDELNET, 2, MidpointRounding.AwayFromZero),
                    CASHCONET = Math.Round(sales.CASHCONET, 2, MidpointRounding.AwayFromZero),
                    ACCTCONET = Math.Round(sales.ACCTCONET, 2, MidpointRounding.AwayFromZero),
                    COSTNETTOT = Math.Round(sales.COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                    SUMSELLQTY = Math.Round(sales.SUMSELLQTY, 2, MidpointRounding.AwayFromZero),
                    CASHCHRDTOT = Math.Round(sales.CASHCHRDTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTCHRDTOT = Math.Round(sales.ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),                    
                    TOTAL = Math.Round(sales.ACCTSELLNET + sales.CASHSELLNET + sales.CASHRETNET + sales.ACCTRETNET + sales.ACCTDELNET + sales.CASHCHRDTOT + sales.ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),
                    PROFIT = Math.Round((sales.ACCTSELLNET + sales.CASHSELLNET + sales.CASHRETNET + sales.ACCTRETNET + sales.ACCTDELNET + sales.CASHCHRDTOT + sales.ACCTCHRDTOT) - sales.COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                 }).ToArray());
            }
        }

        public Task<Sales[]> GetDeptSalesAsync(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
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
                var sales = db.Query<Sales>("Select FIRST 5 vs.DeptID, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHRETTOT, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end)  ACCTSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTRETTOT,  " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSOTOT, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTSOTOT,  " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDEPTSPENT, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDELTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCOTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCOTOT, " +
                "count(distinct case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vt.headid else null end) trancnt, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHRETNET, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET else 0 end)  ACCTSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTRETNET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSONET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTSONET, " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET else 0 end) ACCTDEPTSPENTNET, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTDELNET, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHCONET,  " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTCONET,  " +
                "sum(vs.COST_NET) COSTNETTOT,  " +
                "SUM(case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vs.SELL_QTY else 0 end) SUMSELLQTY,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCHRDTOT,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCHRDTOT " +
                "from VETRANS vt " +
                "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                "join VESALES vs on vt.TRANSID = vs.TRANSID " +
                "where vt.DateTime >= @StartDate and vt.DateTime <= @EndDate " +
                "Group by vs.DeptID", parameters).ToList();

                return Task.FromResult(Enumerable.Range(0, sales.Count).Select(index => new Sales
                {
                    CASHSELLTOT = Math.Round(sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                    CASHRETTOT = Math.Round(sales[index].CASHRETTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTSELLTOT = Math.Round(sales[index].ACCTSELLTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTRETTOT = Math.Round(sales[index].ACCTRETTOT, 2, MidpointRounding.AwayFromZero),
                    CASHSOTOT = Math.Round(sales[index].CASHSOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTSOTOT = Math.Round(sales[index].ACCTSOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTDEPTSPENT = Math.Round(sales[index].ACCTDEPTSPENT, 2, MidpointRounding.AwayFromZero),
                    ACCTDELTOT = Math.Round(sales[index].ACCTDELTOT, 2, MidpointRounding.AwayFromZero),
                    CASHCOTOT = Math.Round(sales[index].CASHCOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTCOTOT = Math.Round(sales[index].ACCTCOTOT, 2, MidpointRounding.AwayFromZero),
                    CASHSELLNET = Math.Round(sales[index].CASHSELLNET, 2, MidpointRounding.AwayFromZero),
                    CASHRETNET = Math.Round(sales[index].CASHRETNET, 2, MidpointRounding.AwayFromZero),
                    ACCTSELLNET = Math.Round(sales[index].ACCTSELLNET, 2, MidpointRounding.AwayFromZero),
                    ACCTRETNET = Math.Round(sales[index].ACCTRETNET, 2, MidpointRounding.AwayFromZero),
                    CASHSONET = Math.Round(sales[index].CASHSONET, 2, MidpointRounding.AwayFromZero),
                    ACCTSONET = Math.Round(sales[index].ACCTSONET, 2, MidpointRounding.AwayFromZero),
                    ACCTDEPTSPENTNET = Math.Round(sales[index].ACCTDEPTSPENTNET, 2, MidpointRounding.AwayFromZero),
                    ACCTDELNET = Math.Round(sales[index].ACCTDELNET, 2, MidpointRounding.AwayFromZero),
                    CASHCONET = Math.Round(sales[index].CASHCONET, 2, MidpointRounding.AwayFromZero),
                    ACCTCONET = Math.Round(sales[index].ACCTCONET, 2, MidpointRounding.AwayFromZero),
                    COSTNETTOT = Math.Round(sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                    SUMSELLQTY = Math.Round(sales[index].SUMSELLQTY, 2, MidpointRounding.AwayFromZero),
                    CASHCHRDTOT = Math.Round(sales[index].CASHCHRDTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTCHRDTOT = Math.Round(sales[index].ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),
                    TOTAL = Math.Round(sales[index].ACCTSELLNET + sales[index].CASHSELLNET + sales[index].CASHRETNET + sales[index].ACCTRETNET + sales[index].ACCTDELNET + sales[index].CASHCHRDTOT + sales[index].ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),
                    PROFIT = Math.Round((sales[index].ACCTSELLNET + sales[index].CASHSELLNET + sales[index].CASHRETNET + sales[index].ACCTRETNET + sales[index].ACCTDELNET + sales[index].CASHCHRDTOT + sales[index].ACCTCHRDTOT) - sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero)
                }).ToArray());
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID };
                var sales = db.Query<Sales>("Select FIRST 5 vs.DeptID," +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHRETTOT, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end)  ACCTSELLTOT, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTRETTOT,  " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHSOTOT, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTSOTOT,  " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDEPTSPENT, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTDELTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCOTOT, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCOTOT, " +
                "count(distinct case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vt.headid else null end) trancnt, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHRETNET, " +
                "sum(case when vt.transtype = 'CS' and vc.CUSTID is not null then vs.SELL_NET else 0 end)  ACCTSELLNET, " +
                "sum(case when vt.transtype = 'RT' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTRETNET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHSONET, " +
                "sum(case when vt.transtype in ('SO', 'XD') and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTSONET, " +
                "sum(case when vt.transtype = 'DS' then vs.SELL_NET else 0 end) ACCTDEPTSPENTNET, " +
                "sum(case when vt.transtype = 'DV' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTDELNET, " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is null then vs.SELL_NET else 0 end) CASHCONET,  " +
                "sum(case when vt.transtype = 'CO' and vc.CUSTID is not null then vs.SELL_NET else 0 end) ACCTCONET,  " +
                "sum(vs.COST_NET) COSTNETTOT,  " +
                "SUM(case when vt.transtype in ('CS', 'RT', 'CO', 'DV', 'SO', 'XD') then vs.SELL_QTY else 0 end) SUMSELLQTY,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is null then vs.SELL_NET + vs.SELL_VAT else 0 end) CASHCHRDTOT,  " +
                "sum(case when vt.transtype = 'CHRD' and vc.CUSTID is not null then vs.SELL_NET + vs.SELL_VAT else 0 end) ACCTCHRDTOT " +
                "from VETRANS vt " +
                "left join VECUSTTRANS vc on vt.TRANSID = vc.TRANSID " +
                "join VESALES vs on vt.TRANSID = vs.TRANSID " +
                "where vt.DateTime >= @StartDate and vt.DateTime <= @EndDate and vt.BRANCHID=@BranchID " + 
                "Group by vs.DeptID", parameters).ToList();

                return Task.FromResult(Enumerable.Range(0, sales.Count).Select(index => new Sales
                {
                    CASHSELLTOT = Math.Round(sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                    CASHRETTOT = Math.Round(sales[index].CASHRETTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTSELLTOT = Math.Round(sales[index].ACCTSELLTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTRETTOT = Math.Round(sales[index].ACCTRETTOT, 2, MidpointRounding.AwayFromZero),
                    CASHSOTOT = Math.Round(sales[index].CASHSOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTSOTOT = Math.Round(sales[index].ACCTSOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTDEPTSPENT = Math.Round(sales[index].ACCTDEPTSPENT, 2, MidpointRounding.AwayFromZero),
                    ACCTDELTOT = Math.Round(sales[index].ACCTDELTOT, 2, MidpointRounding.AwayFromZero),
                    CASHCOTOT = Math.Round(sales[index].CASHCOTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTCOTOT = Math.Round(sales[index].ACCTCOTOT, 2, MidpointRounding.AwayFromZero),
                    CASHSELLNET = Math.Round(sales[index].CASHSELLNET, 2, MidpointRounding.AwayFromZero),
                    CASHRETNET = Math.Round(sales[index].CASHRETNET, 2, MidpointRounding.AwayFromZero),
                    ACCTSELLNET = Math.Round(sales[index].ACCTSELLNET, 2, MidpointRounding.AwayFromZero),
                    ACCTRETNET = Math.Round(sales[index].ACCTRETNET, 2, MidpointRounding.AwayFromZero),
                    CASHSONET = Math.Round(sales[index].CASHSONET, 2, MidpointRounding.AwayFromZero),
                    ACCTSONET = Math.Round(sales[index].ACCTSONET, 2, MidpointRounding.AwayFromZero),
                    ACCTDEPTSPENTNET = Math.Round(sales[index].ACCTDEPTSPENTNET, 2, MidpointRounding.AwayFromZero),
                    ACCTDELNET = Math.Round(sales[index].ACCTDELNET, 2, MidpointRounding.AwayFromZero),
                    CASHCONET = Math.Round(sales[index].CASHCONET, 2, MidpointRounding.AwayFromZero),
                    ACCTCONET = Math.Round(sales[index].ACCTCONET, 2, MidpointRounding.AwayFromZero),
                    COSTNETTOT = Math.Round(sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero),
                    SUMSELLQTY = Math.Round(sales[index].SUMSELLQTY, 2, MidpointRounding.AwayFromZero),
                    CASHCHRDTOT = Math.Round(sales[index].CASHCHRDTOT, 2, MidpointRounding.AwayFromZero),
                    ACCTCHRDTOT = Math.Round(sales[index].ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),
                    TOTAL = Math.Round(sales[index].ACCTSELLNET + sales[index].CASHSELLNET + sales[index].CASHRETNET + sales[index].ACCTRETNET + sales[index].ACCTDELNET + sales[index].CASHCHRDTOT + sales[index].ACCTCHRDTOT, 2, MidpointRounding.AwayFromZero),
                    PROFIT = Math.Round((sales[index].ACCTSELLNET + sales[index].CASHSELLNET + sales[index].CASHRETNET + sales[index].ACCTRETNET + sales[index].ACCTDELNET + sales[index].CASHCHRDTOT + sales[index].ACCTCHRDTOT) - sales[index].COSTNETTOT, 2, MidpointRounding.AwayFromZero)
                }).ToArray());
            }
        }


        public Task<Sales[]> GetTopBranchAsync(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
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

            var parameters = new { StartDate, EndDate };
            var sales = db.QueryFirst<Sales>("Select First(1)" +
                                                "b.Name BRANCHNAME, " +
                                                "sum(s.SELL_NET) TOTAL, " +
                                                "sum(s.SELL_QTY) SumSellQty " +
                                                "from vesales s " +
                                                "join vetrans t on t.TRANSID = s.TRANSID " +
                                                "left join vebranches b on b.BRANCHID = t.BRANCHID " +
                                                "where t.DATETIME >= @StartDate and t.DateTime <= @EndDate " +
                                                "group by b.Name " +
                                                "order by sum(s.SELL_NET) desc ", parameters);

            return Task.FromResult(Enumerable.Range(0, 1).Select(index => new Sales
            {
                BRANCHNAME = sales.BRANCHNAME,
                TOTAL = Math.Round(sales.TOTAL, 2, MidpointRounding.AwayFromZero),
                SUMSELLQTY = Math.Round(sales.SUMSELLQTY, 2, MidpointRounding.AwayFromZero)
            }).ToArray());
        }

        public Task<Sales[]> GetPromotionSalesAsync(DateTimeOffset? Start, DateTimeOffset? End, Int64 BranchID = default)
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
                var sales = db.QueryFirst<Sales>("select " +
                                                "sum(vs.Sell_NET) TOTAL, " +
                                                "sum(vs.SELL_QTY) SUMSELLQTY " +
                                                "from vesales vs " +
                                                "inner join vetrans vt on vt.TRANSID = vs.TRANSID " +
                                                "inner join vepromtrans pt on pt.TRANSID = vs.TRANSID " +
                                                "where vt.DATETIME >= @StartDate and vt.DateTime <= @EndDate ", parameters);

                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new Sales
                {
                    TOTAL = Math.Round(sales.TOTAL, 2, MidpointRounding.AwayFromZero),
                    SUMSELLQTY = Math.Round(sales.SUMSELLQTY, 2, MidpointRounding.AwayFromZero)
                }).ToArray());
            }
            else
            {
                var parameters = new { StartDate, EndDate, BranchID };
                var sales = db.QueryFirst<Sales>("select " +
                                                "sum(vs.Sell_NET) TOTAL, " +
                                                "sum(vs.SELL_QTY) SUMSELLQTY " +
                                                "from vesales vs " +
                                                "inner join vetrans vt on vt.TRANSID = vs.TRANSID " +
                                                "inner join vepromtrans pt on pt.TRANSID = vs.TRANSID " +
                                                "where vt.DATETIME >= @StartDate and vt.DateTime <= @EndDate and vt.BRANCHID=@BranchID", parameters);

                return Task.FromResult(Enumerable.Range(0, 1).Select(index => new Sales
                {
                    TOTAL = Math.Round(sales.TOTAL, 2, MidpointRounding.AwayFromZero),
                    SUMSELLQTY = Math.Round(sales.SUMSELLQTY, 2, MidpointRounding.AwayFromZero)
                }).ToArray());
            }
        }
    }
}
