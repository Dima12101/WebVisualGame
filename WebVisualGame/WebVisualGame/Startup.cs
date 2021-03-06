using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebVisualGame
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

			services.AddDbContext<Data.Repository>(
				options => options.UseSqlServer(@"Server=MOI;Database=WebVisualGame;Trusted_Connection=True;"));

			services.AddDataProtection()
			.PersistKeysToFileSystem(new DirectoryInfo(@"c:\keys"));
			services.AddDataProtection()
			.SetDefaultKeyLifetime(TimeSpan.FromDays(14));

			services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			// ��������� ������ HTTP
			app.UseStatusCodePagesWithReExecute("/Error/{0}");

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			//app.UseRequestLocalization();
			//app.UseResponseCompression();
			app.UseMvc();
			
		}
	}
}
