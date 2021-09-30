using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IIS_V4.Data;

namespace IIS_V4.Pages.Charts
{
    public partial class LikeForLikeTrend
    {
        BarChart<double> trendChart;
        //use this as the parameter to filter the records, much easier than effing about with dates !
        public Int32 RecordIndex { get; set; }
        [Parameter] public Int64 userBranchID { get; set; }
        [Parameter] public string BranchName { get; set; }
        [Parameter] public DateTimeOffset? StartDate { get; set; }
        [Parameter] public DateTimeOffset? EndDate { get; set; }
        public Takings[] lfl_daily_takings { get; set; }
        public Takings[] daily_takings { get; set; }

        bool isAlreadyInitialised;

        object trendChartOptions => new
        {
            Title = new
            {
                Display = true,
            },
            Tooltips = new
            {
                Enabled = false,
            },
            Legend = new Legend
            {
                Display = false,
            },
            Scales = new
            {
                XAxes = new[]
                       {
                        new
                           {
                            labels =  GetDateTimeLabels()
                           }
                       }
            },
            animation = new { duration = 0 },
            responsiveAnimationDuration = 0
        };
        void OnClicked(ChartMouseEventArgs e)
        {
            var model = e.Model as BarChartModel;
            RecordIndex = e.Index;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!isAlreadyInitialised)
            {
                isAlreadyInitialised = true;
                if (daily_takings is not null)
                {
                    await Task.WhenAll(HandleRedraw());
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            if (StartDate == DateTime.MinValue || StartDate is null)
            {
                StartDate = DateTime.Today.AddDays(-7);
                EndDate = DateTime.Now;
            }

            DateTime? _starts = StartDate.HasValue ? StartDate.Value.DateTime : DateTime.Today;
            DateTime? _ends = EndDate.HasValue ? EndDate.Value.DateTime : DateTime.Now;
            DateTime Start = (DateTime)_starts;
            DateTime End = (DateTime)_ends;

            daily_takings = await takingsService.GetTakingsTrendAsync(StartDate, EndDate, userBranchID);
            lfl_daily_takings = await takingsService.GetTakingsTrendAsync(Start.AddDays(-364), End.AddDays(-364), userBranchID);
            await InvokeAsync(() => StateHasChanged());
        }

        async Task HandleRedraw()
        {
            await trendChart.Clear();
            await trendChart.AddDataSet(GetThisYearDataset());
            await trendChart.AddDataSet(GetLastYearDataset());
        }

        public string[] GetDateTimeLabels()
        {
            string[] dateLabels = daily_takings.Select(x => x.TransDate.ToString("dd/MM")).ToArray();
            return dateLabels;
        }

        BarChartDataset<double> GetThisYearDataset()
        {
            List<string> bColors = new List<string> { };
            List<string> bBorders = new List<string> { };
            var count = daily_takings.Select(value => value.Profit).ToList();
            foreach (var data in count)
            {
                bColors.Add(ChartColor.FromRgba(255, 99, 132, 0.2f));
                bBorders.Add(ChartColor.FromRgba(255, 99, 132, 1f));
            }
            return new BarChartDataset<double>
            {
                Data = daily_takings.Select(value => value.Profit).ToList(),
                BackgroundColor = bColors,
                BorderColor = bBorders,
            };
        }

        BarChartDataset<double> GetLastYearDataset()
        {
            List<string> bColors = new List<string> { };
            List<string> bBorders = new List<string> { };
            var count = lfl_daily_takings.Select(value => value.Profit).ToList();
            foreach (var data in count)
            {
                bColors.Add(ChartColor.FromRgba(54, 162, 235, 0.2f));
                bBorders.Add(ChartColor.FromRgba(54, 162, 235, 1f));
            }
            return new BarChartDataset<double>
            {
                Data = lfl_daily_takings.Select(value => value.Profit).ToList(),
                BackgroundColor = bColors,
                BorderColor = bBorders,
            };
        }
    }
}
