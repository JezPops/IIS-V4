using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IIS_V4.Data;


namespace IIS_V4.Shared
{
    public partial class NavMenu
    {
        private bool collapseNavMenu = true;
        private bool expandSubNav;
        [Parameter] public string Link { get; set; }
        [Parameter] public Int64 _branchID { get; set; }
        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        public Branches[] branches;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
        protected override async Task OnInitializedAsync()
        {
            branches = await branchService.GetBranches();
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var currentUser = await _UserManager.GetUserAsync(user);
            _branchID = currentUser.UserBranch;
            await InvokeAsync(() => StateHasChanged());
        }
    }
}
