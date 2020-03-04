using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using PM.Entity.Models;
using PM.Entity.Services;

namespace PM.Vendor.UI
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

			services.AddDbContext<VandivierProductManagerContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("Connection")));

			// Injectable data access service
			services.AddScoped<IDbReadService, DbReadService>();
			services.AddScoped<IDbWriteService, DbWriteService>();

			string instance = Configuration["AzureAdB2C:Instance"];
			string tenant = Configuration["AzureAdB2C:Tenant"];
			string signupPolicy = Configuration["AzureAdB2C:SignUpSignInPolicyId"];

			IdentityModelEventSource.ShowPII = true;

			services.AddAuthentication(AzureADB2CDefaults.AuthenticationScheme).AddAzureADB2C(options => Configuration.Bind("AzureAdB2C", options)); ;

			var myservice = services.Configure<OpenIdConnectOptions>(AzureADB2CDefaults.OpenIdScheme, options =>
			{
				options.Authority = String.Format(instance, tenant, signupPolicy);   
				options.TokenValidationParameters.ValidateIssuer = false;
			});

			//tring auth = 

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			//if (env.IsDevelopment())
			//{
				app.UseDeveloperExceptionPage();
			//}
			//else
			//{
			//	app.UseExceptionHandler("/Home/Error");
			//	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			//	app.UseHsts();
			//}

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
