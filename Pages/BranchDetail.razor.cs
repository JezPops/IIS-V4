using BlazorDateRangePicker;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IIS_V4.Data;

namespace IIS_V4.Pages
{
    public partial class BranchDetail
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        [Parameter] public Int64 ID { get; set; }
        [Parameter] public string MetricType { get; set; }
        [Parameter] public string branchName { get; set; }

        public Branches[] branches;

        bool isAlreadyInitialised;

        bool isLoading;
        [Parameter] public DateTimeOffset? StartDate { get; set; }
        [Parameter] public DateTimeOffset? EndDate { get; set; }

        DateRangePicker Picker;

        Dictionary<string, DateRange> DateRanges => new Dictionary<string, DateRange> {
            { "Today", new DateRange
                {
                    Start = new DateTime(DateTime.Now.Year ,  DateTime.Now.Month, DateTime.Now.Day),
                    End = new DateTime(DateTime.Now.Year,  DateTime.Now.Month, DateTime.Now.Day)
                }
            } ,

            { "Yesterday", new DateRange
                {
                    Start = new DateTime(DateTime.Now.Year ,DateTime.Now.Month, DateTime.Now.Day-1),
                    End = new DateTime(DateTime.Now.Year,  DateTime.Now.Month, DateTime.Now.Day)
                }
            } ,

            { "Last Week", new DateRange
                {
                    Start = new DateTime(DateTime.Now.Year ,  DateTime.Now.Month, DateTime.Now.Day).AddDays( - 6 - (int)DateTime.Now.DayOfWeek),
                    End = new DateTime(DateTime.Now.Year,  DateTime.Now.Month, DateTime.Now.Day).AddDays(-(int)DateTime.Now.DayOfWeek)
                }
            } ,

            { "This month", new DateRange
                {
                    Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddTicks(-1)
                }
            } ,
            { "Previous month" , new DateRange
                {
                    Start = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1),
                    End = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1).AddMonths(1).AddTicks(-1)
                }
            }
        };
        public async Task LoadCharts()
        {
            StartDate = Picker.StartDate;
            EndDate = Picker.EndDate;
            isLoading = true;
            await InvokeAsync(() => StateHasChanged());
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!isAlreadyInitialised)
            {
                isAlreadyInitialised = true;
                await InvokeAsync(() => StateHasChanged());
            }
            else if (isLoading)
            {
                isLoading = false;
                await InvokeAsync(() => StateHasChanged());
            }
        }
        protected override async Task OnParametersSetAsync()
        {
            if (!isLoading)
            {
                isLoading = true;
                await InvokeAsync(() => StateHasChanged());
            }
        }
        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationStateTask;
            var user = authState.User;
            var adminBranch = user.FindFirstValue(ClaimTypes.SerialNumber);
            branches = await branchService.GetBranches();
            var result = branches.FirstOrDefault(o => o.BRANCHID == ID);
            branchName = result.Name;
            await InvokeAsync(() => StateHasChanged());
        }
    }
}
