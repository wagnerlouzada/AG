using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Quickstart.UI;
using Identity.Data;
using Identity.Domain;
using Identity.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using Api.Models;

namespace Identity.IdentityServer
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
                    services.AddDbContext<IdentityContext>(options =>
                        options.UseSqlite(Configuration.GetConnectionString("IdentitySqLiteConnection")));
                    services.AddDbContext<PersistedGrantDbContext>(options =>
                        options.UseSqlite(Configuration.GetConnectionString("IdentitySqLiteConnection")));
                    break;
                default:
                    connectionString = Configuration.GetConnectionString("IdentitySqlServerConnection");
                    services.AddDbContext<IdentityContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("IdentitySqlServerConnection")));
                    services.AddDbContext<PersistedGrantDbContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("IdentitySqlServerConnection")));
                    break;
            }

            switch (DataBaseProvider.ToUpper())
            {
                case "SQLITE":
                    //connectionString = Configuration.GetConnectionString("IdentitySqLiteConnection");
                    services.AddDbContext<AgContext>(options =>
                        options.UseSqlite(Configuration.GetConnectionString("AgSqLiteConnection"))); break;
                default:
                    //connectionString = Configuration.GetConnectionString("IdentitySqlServerConnection");
                    services.AddDbContext<AgContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("AgSqlServerConnection")));
                    break;
            }

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<IdentityContext>()
                    .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc(options => {
                options.Filters.Add(new RequireHttpsAttribute());
            });

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            switch (DataBaseProvider.ToUpper())
            {
                case "SQLITE":
                    services.AddIdentityServer()
                            .AddDeveloperSigningCredential()
                            .AddAspNetIdentity<ApplicationUser>()
                            .AddConfigurationStore(options =>
                            {
                                options.ConfigureDbContext = builder => builder.UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                            })
                            .AddOperationalStore(options =>
                            {
                                options.ConfigureDbContext = builder => builder.UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                                                    // this enables automatic token cleanup. this is optional.
                                                    options.EnableTokenCleanup = true;
                                options.TokenCleanupInterval = 30;
                            });
                    break;
                default:
                    services.AddIdentityServer()
                            .AddDeveloperSigningCredential()
                            .AddAspNetIdentity<ApplicationUser>()
                            .AddConfigurationStore(options =>
                            {
                                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                            })
                            .AddOperationalStore(options =>
                            {
                                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                                        // this enables automatic token cleanup. this is optional.
                                        options.EnableTokenCleanup = true;
                                options.TokenCleanupInterval = 30;
                            });
                    break;
            }

            String GoogleClientId="";
            String GoogleClientSecret="";
            try
            {
                GoogleClientId = Configuration["GoogleClientId"].ToString();
                GoogleClientSecret = Configuration["GoogleClientSecret"].ToString();
            }
            catch { }
            services.AddAuthentication().AddGoogle("Google", options =>
            {
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.ClientId = GoogleClientId;
                options.ClientSecret = GoogleClientSecret;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRewriter(new RewriteOptions().AddRedirectToHttps(301, 44367));

            AccountOptions.ShowLogoutPrompt = false;
            AccountOptions.AutomaticRedirectAfterSignOut = true;

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {

                var appDbContext = serviceScope.ServiceProvider.GetRequiredService<IdentityContext>();
                appDbContext.Database.EnsureCreated();
                var configDbAppCreator = (RelationalDatabaseCreator)appDbContext.Database.GetService<IDatabaseCreator>();

                var configDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                var configDbCreator = (RelationalDatabaseCreator)configDbContext.Database.GetService<IDatabaseCreator>();

                var grantDbContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                grantDbContext.Database.EnsureCreated();
                var configDbGrantContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                var configDbGrantCreator = (RelationalDatabaseCreator)configDbGrantContext.Database.GetService<IDatabaseCreator>();

                try
                {
                    configDbAppCreator.CreateTables();
                }
                catch (Exception ex)
                {
                    var x = ex;
                }

                try
                {
                    configDbGrantCreator.CreateTables();
                }
                catch (Exception ex)
                {
                    var x = ex;
                }

                try
                {
                    configDbCreator.CreateTables();
                }
                catch (Exception ex)
                {
                    var x = ex;
                }

                if (!configDbContext.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        configDbContext.Clients.Add(client.ToEntity());
                    }
                    configDbContext.SaveChanges();
                }

                if (!configDbContext.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        configDbContext.IdentityResources.Add(resource.ToEntity());
                    }
                    configDbContext.SaveChanges();
                }

                if (!configDbContext.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources())
                    {
                        configDbContext.ApiResources.Add(resource.ToEntity());
                    }
                    configDbContext.SaveChanges();
                }

            }
        }

    }
}
