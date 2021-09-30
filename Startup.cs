using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using IIS_V4.Areas.Identity;
using IIS_V4.Data;
using Microsoft.EntityFrameworkCore;
using FirebirdSql.Data.FirebirdClient;

namespace IIS_V4
{
    public class Startup
    {
        public static string VectorConnection { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBlazorise(options =>
            {
                options.ChangeTextOnKeyPress = true; // optional
            })
                  .AddBootstrapProviders()
                  .AddFontAwesomeIcons();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);//We set Time here 
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter; options.SlidingExpiration = true;
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ApplicationDbContext>(options => options.UseFirebird(Configuration.GetConnectionString("VectorConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 7;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;
                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.SignIn.RequireConfirmedAccount = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultUI().AddDefaultTokenProviders();

            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            });
            services.AddRazorPages();

            services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

            });
            services.AddAuthorization(config =>
            {
                config.AddPolicy("Branch", policy => policy.AddRequirements(new ValidUserBranch(0)));
            });
            services.AddTransient<IEmailSender, DashboardMailer>();


            services.AddTransient<IAuthorizationHandler, BranchAuthHandler>();
            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddSingleton<TakingsService>();
            services.AddSingleton<ProductsService>();
            services.AddSingleton<DepartmentService>();
            services.AddSingleton<OperatorService>();
            services.AddSingleton<CustomerOrderService>();
            services.AddSingleton<PurchaseOrderService>();
            services.AddSingleton<PaymentsService>();
            services.AddSingleton<SalesService>();
            services.AddSingleton<BranchService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            VectorConnection = Configuration.GetConnectionString("VectorConnection");
            IDbConnection db = new FbConnection(VectorConnection);

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
