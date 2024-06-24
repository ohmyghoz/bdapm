using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.DataModel;
using BDA.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using System.IO;

//reporting
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using DevExpress.XtraReports.Security;
using CommonLog;
using Newtonsoft.Json.Serialization;


//dashboard
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.XtraReports.Web.Extensions;

namespace BDA
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Env = environment;
            DashboardOlapDataSource.OlapDataProvider = OlapDataProviderType.Xmla;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); // supaya validate antiforgery di semua controller
                options.Conventions.Add(new AddAuthorizeFiltersControllerConvention()); // convention auto add WSAuthorize
            }).AddNewtonsoftJson(options =>{options.SerializerSettings.ContractResolver = new DefaultContractResolver();});

            services.AddRazorPages().AddNewtonsoftJson(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });

            // supaya bisa access httpcontext
            services.AddHttpContextAccessor();
            
            //services.AddControllers().AddNewtonsoftJson(options =>
            // {
            //     options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            // });

            //prevent XSRF https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-2.2
            services.AddDataProtection();

            //dah ga perlu? ada AutoValidateAntiForgery?
            services.AddAntiforgery(opts =>
            {
                opts.Cookie.Name = "BDA_AntiforgeryCookie";
                opts.HeaderName = "XSRF-TOKEN";
                opts.Cookie.HttpOnly = true;
                opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });


            //framework EF6
            var appSettingsSection = Configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();
            appSettings.ConnString = Configuration.GetConnectionString("DataEntities");    
            appSettings.DataConnString = Configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton<AppSettings>(appSettings);
            services.AddScoped<DataEntities>();            
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication            
            var key = System.Text.Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(x =>
                {
                    x.LoginPath = new PathString("/Home/Login");
                    x.AccessDeniedPath = new PathString("/Home/Error");

                    x.ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToInt32(appSettings.LoginTimeOut));
                    x.SlidingExpiration = true;
                    x.Cookie.Name = "BDA_NetCoreCookie";
                    x.Cookie.HttpOnly = true;
                    x.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            //authorization WS Authorize
            services.AddAuthorization(o =>
            {
                o.AddPolicy("defaultpolicy", b =>
                {
                    b.Requirements.Add(new WSAuthorizeRequirement());
                    b.RequireAuthenticatedUser();
                });
                o.AddPolicy("apipolicy", b =>
                {
                    b.RequireAuthenticatedUser();
                    b.AuthenticationSchemes = new List<string> { JwtBearerDefaults.AuthenticationScheme };
                });
            });
            services.AddScoped<IAuthorizationHandler, WSAuthorizationHandler>();

            //session 
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(Convert.ToInt32(appSettings.LoginTimeOut));
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "BDA_Cookie";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            //reporting
            services.AddDevExpressControls();

            services.ConfigureReportingServices(configurator =>
            {
                configurator.ConfigureReportDesigner(designerConfigurator =>
                {
                    designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();
                });
                services.ConfigureReportingServices(configurator => {
                    configurator.DisableCheckForCustomControllers();
                    // ... 
                });
            });

            //services.AddScoped<ReportStorageWebExtension, WSReportStorageWebExtension>();
            
            ////dashboard
            //services.AddMvc().AddDefaultDashboardController((configurator, serviceProvider) => {
            //    //configurator.SetDataSourceStorage(new DataSourceInMemoryStorage());
            //    //configurator.SetDashboardStorage(new DashboardFileStorage(Path.Combine(Env.ContentRootPath, "App_Data", "Dashboards")));
            //    configurator.SetDashboardStorage(new WSDashboardStorage(Configuration.GetConnectionString("DefaultConnection")));
            //    configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(Configuration));
            //    configurator.SetDataSourceStorage(CreateDataSourceStorage());                
            //    configurator.ConfigureDataConnection += Configurator_ConfigureDataConnection;
            //    configurator.AllowExecutingCustomSql = true;                
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ReportStorageWebExtension reportStorageWebExtension)
        {
            
            //enable kalau mau semua akses dan request ke log ke tabel LOG_AKSES
            app.UseCommonLogging();

            //reporting

            ReportStorageWebExtension.RegisterExtensionGlobal(reportStorageWebExtension);
            DevExpress.XtraReports.Configuration.Settings.Default.UserDesignerOptions.DataBindingMode =
                DevExpress.XtraReports.UI.DataBindingMode.ExpressionsAdvanced;

            ScriptPermissionManager.GlobalInstance = new ScriptPermissionManager(ExecutionMode.Unrestricted);



            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Xss-Protection", "1");
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            });

            app.UseDevExpressControls();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),
                RequestPath = "/node_modules"
            });

            //errhandler
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            //app.UseHttpsRedirection();
            //app.UseHsts();
            app.UseStaticFiles();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.SameAsRequest
            });
            //session
            app.UseSession();

            //authentication
            //app.UseAuthentication();
            //app.UseRouting();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //EndpointRouteBuilderExtension.MapDashboardRoute(endpoints, "api/dashboards");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "default",
                    areaName: "WebSite",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });          

           
        }

        public DataSourceInMemoryStorage CreateDataSourceStorage()
        {
            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();

            //DashboardOlapDataSource olapDataSource = new DashboardOlapDataSource("OLAP Data Source", "olapConnection");
            //dataSourceStorage.RegisterDataSource("olapDataSource", olapDataSource.SaveToXml());
            
            return dataSourceStorage;
        }

        private void Configurator_ConfigureDataConnection(object sender, ConfigureDataConnectionWebEventArgs e)
        {
            //if (e.ConnectionName == "olapConnection")
            //{
            //    OlapConnectionParameters olapParams = new OlapConnectionParameters();
            //    olapParams.ConnectionString = "Provider=MSOLAP.7;Data Source=http://localhost/OLAP/msmdpump.dll;"
            //+ "Initial catalog=EDW_SPRINT;Cube name=DS EDW;Query Timeout=100;";
            //    e.ConnectionParameters = olapParams;
            //}
        }
    }
}
