using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApp_OpenIDConnect_DotNet {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // public void ConfigureDevelopmentServices (IServiceCollection services) 
        // {
        //     services.AddAuthorization (options => {
        //         options.DefaultPolicy = new AuthorizationPolicyBuilder ()
        //             .RequireAssertion (_ => true)
        //             .Build ();
        //     });

        //     System.Diagnostics.Trace.WriteLine("Configuring services in Development");
        //     AzureAdOptions opts = new AzureAdOptions ();
        //     Configuration.Bind ("AzureAd", opts);
        //     AzureAdOptions.Settings = opts;

        //     services.AddMvc ();
        // }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) 
        {
            services.AddAuthentication (sharedOptions => {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddAzureAd (options => {
                    Configuration.Bind ("AzureAd", options);
                    AzureAdOptions.Settings = options;
                })
                .AddCookie ();

            System.Diagnostics.Trace.WriteLine("Configuring services in Production");
            services.AddMvc ().AddSessionStateTempDataProvider ();
            services.AddSession ();
        }

        public void ConfigureDevelopment (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) 
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Trace);

            app.UseDeveloperExceptionPage ();
            app.UseStaticFiles ();

            app.UseSession ();
            app.UseAuthentication ();

            app.UseMvc (routes => {
                routes.MapRoute (
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) 
        {
            app.UseExceptionHandler ("/Home/Error");
            app.UseStaticFiles ();

            app.UseSession (); // Needs to be app.UseAuthentication() and app.UseMvc() otherwise you will get an exception "Session has not been configured for this application or request."
            app.UseAuthentication ();
            
            app.UseMvc (routes => {
                routes.MapRoute (
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}