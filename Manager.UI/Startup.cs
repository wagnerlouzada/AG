using IdentityServerManager.UI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServerManager.UI.Infrastructure;
using AutoMapper;
using System;

namespace IdentityServerManager.UI
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

            string connectionString = "";

            switch (DataBaseProvider.ToUpper())
            {
                case "SQLITE":
                    connectionString = Configuration.GetConnectionString("IdentitySqLiteConnection");
                    services.AddDbContext<ConfigurationDbContext>(options =>
                        options.UseSqlite(Configuration.GetConnectionString("IdentitySqLiteConnection")));
                    break;
                default:
                    connectionString = Configuration.GetConnectionString("IdentitySqlServerConnection");
                    services.AddDbContext<ConfigurationDbContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("IdentitySqlServerConnection")));

                    break;
            }

            services.AddAutoMapper();

            //services.AddDbContext<ConfigurationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("IdentitySqlServerConnection")));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
