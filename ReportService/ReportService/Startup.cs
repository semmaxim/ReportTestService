using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using ReportService.Data.Services;
using ReportService.Services.EmpCodeResolver;
using ReportService.Services.SalaryResolver;

#if USE_POSTGRESQL
using ReportService.PostgreSql.Services;
#elif USE_MYSQL
using ReportService.MySql.Services;
#else
#error Wrong build configuration
#endif


namespace ReportService
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
			services
				.AddLogging(loggingBuilder =>
				{
					loggingBuilder.ClearProviders();
					loggingBuilder.SetMinimumLevel(LogLevel.Trace);
					loggingBuilder.AddNLog(Configuration);
				});

			services.AddControllers();
			services.AddMvc();

			services.AddSingleton<IEmpCodeResolverService, EmpCodeResolverService>();
			services.AddSingleton<ISalaryResolverService, SalaryResolverService>();

			services.AddTransient<IDepartmentsRepository, DepartmentsRepository>();

#if USE_POSTGRESQL
			services.AddTransient<IDbConnectionFactory, PostgreSqlDbConnectionFactory>();
#elif USE_MYSQL
			services.AddTransient<IDbConnectionFactory, MySqlDbConnectionFactory>();
#else
			#error Wrong build configuration
#endif
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
