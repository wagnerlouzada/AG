using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppV.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using AppV.Models;
using Microsoft.AspNetCore.Identity.UI;


//
// https://www.codeproject.com/Tips/1271379/Simple-ASP-NET-CORE-2-2-App-plusVue-JS
//
namespace AppV
{
    public class Startup : Controller
    {
        public static IConfiguration Configuration { get; set; }
        public static string DATABASE = "MsAccess";
        public static string CONNECTIOSTRING = "Provider=Odbc;Driver={Microsoft Access Driver (*.mdb, *.accdb)};DBQ={AppDir}\\Storage\\v.mdb";

        public Startup(IConfiguration configuration)
        {
             Configuration = configuration;
             DATABASE = Configuration["DATABASE"];
             CONNECTIOSTRING = Configuration["AppConnectionStrings:"+DATABASE];
        }
   
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //services.AddMvc();

            services
.AddMvc()
.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
.AddRazorPagesOptions(options =>
{
    options.Conventions.AuthorizeFolder("/Areas/Identity/Pages/Account");
});


            // ...


            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddMediatR(typeof(Startup));            

            Action<AppV.Models.ConfigurationString> configurationString = (opt =>
            {
                opt.value = Configuration["ConnectionStrings:key"];
            });
            services.Configure(configurationString);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppV.Models.ConfigurationString>>().Value);

            //
            // Register db factory
            //
            DbProviderFactories.RegisterFactory("MsSqlServer", System.Data.SqlClient.SqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("Odbc", System.Data.Odbc.OdbcFactory.Instance);
            DbProviderFactories.RegisterFactory("PostGres", Npgsql.NpgsqlFactory.Instance);
            DbProviderFactories.RegisterFactory("SqLite", System.Data.SQLite.SQLiteFactory.Instance);


            ////services.AddDefaultIdentity<IdentityUser>()
            ////services.AddIdentity<IdentityUser, IdentityRole>()
            ////    .AddDefaultUI(UIFramework.Bootstrap4)
            ////    .AddEntityFrameworkStores<ApplicationDbContext>()
            ////    .AddDefaultTokenProviders();

            ////services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            ////    .AddRazorPagesOptions(options =>
            ////    {
            ////        options.AllowAreas = true;
            ////        options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
            ////        options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            ////    });

            //services.AddMvc()
            //    .AddRazorPagesOptions(options =>
            //    {
            //        options.AllowAreas = true;
            //        options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
            //        options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            //    });

            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = $"/Identity/Account/Login";
            //    options.LogoutPath = $"/Identity/Account/Logout";
            //    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            //});
        }

        //
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseMvc();
        }

    }
}
