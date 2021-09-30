using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using IIS_V4.Data;

namespace IIS_V4.Pages.Charts
{
    public partial class DailyTakings
    {
        Chart<double> profitPie;
        public Takings[] daily_takings { get; set; }
        [Parameter] public Int64 userBranchID { get; set; }
        [Parameter] public string BranchName { get; set; }
        [Parameter] public DateTimeOffset? StartDate { get; set; }
        [Parameter] public DateTimeOffset? EndDate { get; set; }
        [Parameter] public bool AutoRefresh { get; set; }
        [Parameter] public string Link { get; set; }
        [Parameter] public string Icon { get; set; }

        public Timer timer;

        bool isAlreadyInitialised;

        object pieChartOptions = new
        {
            Title = new
            {
                Display = false,
                Text = "Daily Takings"
            },
            Tooltips = new
            {
                Enabled = false,
            },
            Legend = new Legend
            {
                Display = true,
                FullWidth = false,
                Reverse = true
            },

            animation = new { duration = 0 },
            responsiveAnimationDuration = 0
        };
        protected override async Task OnInitializedAsync()
        {
            if (StartDate == DateTime.MinValue)
            {
                StartDate = DateTime.Today;
                EndDate = DateTime.Now;
            }
            daily_takings = await takingsService.GetTakingsRangeAsync(StartDate, EndDate, userBranchID);
            await InvokeAsync(() => StateHasChanged());
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!isAlreadyInitialised)
            {
                isAlreadyInitialised = true;
                if (AutoRefresh)
                {
                    timer = new Timer();
                    timer.Interval = 30000;
                    timer.Elapsed += OnTimerInterval;
                    timer.AutoReset = true;
                    timer.Enabled = true;
                }
                await Task.WhenAll(HandleRedraw(profitPie, GetProfitPieChartDataset));
            }
        }
        private async void OnTimerInterval(object sender, ElapsedEventArgs e)
        {
            daily_takings = await takingsService.GetTakingsRangeAsync(StartDate, EndDate, userBranchID);
            await Task.WhenAll(HandleRedraw(profitPie, GetProfitPieChartDataset));
            await InvokeAsync(() => StateHasChanged());
        }
        PieChartDataset<double> GetProfitPieChartDataset()
        {
            return new PieChartDataset<double>
            {
                Data = GetProfitPie(),
                BackgroundColor = new List<string> { ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(54, 162, 235, 0.2f) },
                BorderColor = new List<string> { ChartColor.FromRgba(255, 99, 132, 1f), ChartColor.FromRgba(54, 162, 235, 1f) },
                BorderWidth = 1,
            };
        }
        public void Dispose()
        {
            timer?.Dispose();
        }
        async Task HandleRedraw<TDataSet, TItem, TOptions, TModel>(Blazorise.Charts.BaseChart<TDataSet, TItem, TOptions, TModel> chart, Func<TDataSet> getDataSet)
        where TDataSet : ChartDataset<TItem>
        where TOptions : ChartOptions
        where TModel : ChartModel
        {
            string Cost = Math.Round(daily_takings[0].CostNet, 2, MidpointRounding.AwayFromZero).ToString("C2");
            string Profit = Math.Round(daily_takings[0].Profit, 2, MidpointRounding.AwayFromZero).ToString("C2");
            string[] Labels = { "Profit:" + Profit, "Cost:" + Cost };
            await chart.Clear();
            await chart.AddLabelsDatasetsAndUpdate(Labels, getDataSet());

        }
        List<double> GetProfitPie()
        {
            return new List<double> { Math.Round(daily_takings[0].Profit, 2, MidpointRounding.AwayFromZero), Math.Round(daily_takings[0].CostNet, 2, MidpointRounding.AwayFromZero) };
        }
    }
}
