using System.Linq;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PM.Entity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using PM.Entity.Models;
using PM.UserAdmin.UI.Security;

namespace PM.UserAdmin.UI
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
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddDbContext<VandivierProductManagerContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Connection")));

		    services.AddAuthentication(AzureADDefaults.AuthenticationScheme).AddAzureAD(AzureADDefaults.AuthenticationScheme,
		        OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme, "Test",
		        options => Configuration.Bind("AzureAd", options));

            services.AddAuthorization(policies =>
            {
                policies.AddPolicy(GroupAuthorization.AdminPolicyName, policy => policy.RequireAssertion(x => GroupAuthorization.AdminPolicyAssertion(x, Configuration)));
                policies.AddPolicy(GroupAuthorization.HeadQuartersPolicyName, policy => policy.RequireAssertion(x => GroupAuthorization.HeadQuartersPolicyAssertion(x, Configuration)));
                policies.AddPolicy(GroupAuthorization.StoreManagerPolicyName, policy => policy.RequireAssertion(x => GroupAuthorization.StoreManagersPolicyAssertion(x, Configuration)));
                policies.AddPolicy(GroupAuthorization.EmployeePolicyName, policy => policy.RequireAssertion(x => GroupAuthorization.EmployeePolicyAssertion(x, Configuration)));
            });

			services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
			{
				options.Authority = options.Authority + "/v2.0/";         
				options.TokenValidationParameters.ValidateIssuer = false; 
			});

			// Injectable data access service
			services.AddScoped<IDbReadService, DbReadService>();
			services.AddScoped<IDbWriteService, DbWriteService>();

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseCookiePolicy();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Requests}/{action=Index}/{id?}");
			});
		}
	}
}
