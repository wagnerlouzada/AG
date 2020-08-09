using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Identity.Data;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
//using IdentityServer4.Quickstart.UI;

namespace Ag.Api
{
    public class Startup
    {

        public String DataBaseProvider = "";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            DataBaseProvider = Configuration["DataBaseProvider"].ToString();

            //string connectionString = "";

            switch (DataBaseProvider.ToUpper())
            {
                case "SQLITE":
                    //connectionString = Configuration.GetConnectionString("IdentitySqLiteConnection");
                    services.AddDbContext<IdentityContext>(options =>
                        options.UseSqlite(Configuration.GetConnectionString("IdentitySqLiteConnection")));
                    services.AddDbContext<AgContext>(options =>
                        options.UseSqlite(Configuration.GetConnectionString("AgSqLiteConnection"))); break;
                default:
                    //connectionString = Configuration.GetConnectionString("IdentitySqlServerConnection");
                    services.AddDbContext<IdentityContext>(options => 
                        options.UseSqlServer(Configuration.GetConnectionString("IdentitySqlServerConnection")));
                    services.AddDbContext<AgContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("AgSqlServerConnection")));
                    break;
            }

            services.AddMvc();

            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());
            //});

            services.AddAuthentication()
                    .AddJwtBearer(options =>
                    {
                        options.Authority = "https://localhost:44367";
                        options.Audience = "AgDataApi";
                        options.TokenValidationParameters.NameClaimType = "name";
                    });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddCors(options => {
                options.AddPolicy("SPA", policy => {
                    policy.WithOrigins("https://localhost:44340", "https://localhost:44398")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddAuthentication(options =>
                                        {
                                            options.DefaultScheme = "Cookies";
                                            options.DefaultChallengeScheme = "oidc";
                                        })
                                .AddCookie("Cookies")
                                .AddOpenIdConnect("oidc", options =>
                                {
                                    options.SignInScheme = "Cookies";
                                    options.Authority = "https://localhost:44367";
                                    options.ClientId = "mvc";
                                    options.ClientSecret = "secret";
                                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                                    options.SaveTokens = true;
                                    options.GetClaimsFromUserInfoEndpoint = true;
                                    options.Scope.Add("AgDataApi");
                                    options.Scope.Add("offline_access");
                                    options.TokenValidationParameters.NameClaimType = "name";
                                });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseCors("SPA");

            app.UseMvc();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRewriter(new RewriteOptions().AddRedirectToHttps(301, 44367));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
