using System;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebVisualGame_MVC.Data;
using WebVisualGame_MVC.Utilities.Logger;

namespace WebVisualGame_MVC
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// Свойство, которое будет хранить конфигурацию
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

			string connection = Configuration.GetConnectionString("DefaultConnection");
			services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));

			services.AddDataProtection()
			.PersistKeysToFileSystem(new DirectoryInfo(@"c:\keys"));
			services.AddDataProtection()
			.SetDefaultKeyLifetime(TimeSpan.FromDays(14));

			services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

			services.AddDistributedMemoryCache();

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options => options.LoginPath = new PathString("/Account/Authorization"));

			// Добавление сервисов фреймворка MVC
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			if (env.IsDevelopment())
			{
                bool LogFilter(string category, LogLevel level)
                {
                    bool shouldLog =
                        (category.Contains("WebVisualGame_MVC")) ||
                        (category.Contains("System")) && level >= LogLevel.Error ||
                        (category.Contains("Microsoft") && level >= LogLevel.Error);

                    return shouldLog;
                }

                loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "log/logger.txt"), LogFilter);
				loggerFactory.CreateLogger("FileLogger").LogInformation("Logger created!");

				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
//			app.UseCookiePolicy();
			app.UseAuthentication();

			/*
			 * if(context.Session.Keys.Contains("name"))
					var test =  context.Session.GetString("name");
				else
				{
					context.Session.SetString("name", "Tom");
				}
			 */

			// Добавление компонентов mvc и определение маршрута
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
