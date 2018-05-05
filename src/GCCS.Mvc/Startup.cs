using GCCS.Mvc.Controllers;
using GCCS.Mvc.Data;
using GCCS.Mvc.Filters;
using GCCS.Mvc.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GCCS.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureMvcServices(services);
            ConfigureAuthenticationServices(services);
            ConfigureDatabaseAccess(services);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        private void ConfigureMvcServices(IServiceCollection services)
        {
            services.AddSingleton<AntiforgeryCookieResultFilter>();

            //services.AddAntiforgery(options =>
            //{
            //    options.Cookie.Name = "XSRF-TOKEN";
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            //});

            services
                .AddMvc(options =>
                {
                    options.Filters.AddService<AntiforgeryCookieResultFilter>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private void ConfigureAuthenticationServices(IServiceCollection services)
        {
            // These must be 'Scoped'. I think it's because of the way the Identity
            // services work with respect to Entity Framework Core and it's DbContext.
            // (even though we're not using EF Core, Identity still disposes these).
            // TODO: Decide if I want to just not implement Dispose() properly in
            // DefaultUserStore, and then make it a singleton anyway.
            services.AddScoped<IUserStore<ApplicationUser>, DefaultUserStore>();
            services.AddScoped<IRoleStore<ApplicationRole>, MyRoleStore>();

            services.AddIdentity<ApplicationUser, ApplicationRole>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;

                options.LoginPath = AuthenticationController.LoginRoute;
                options.SlidingExpiration = true;
            });
        }

        private void ConfigureDatabaseAccess(IServiceCollection services)
        {
            services.AddSingleton<DbConnectionProvider>();
            services.AddSingleton<DapperUserRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    //spa.UseAngularCliServer(npmScript: "start");
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
    }
}
