using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReportService.Data.Domain;
using ReportService.Data.Services;
using ReportService.Services.EmpCodeResolver;
using ReportService.Services.SalaryResolver;

namespace ReportService.Controllers
{
	[Route("api/[controller]")]
	public class ReportController : Controller
	{
		public ReportController(IDepartmentsRepository departmentsRepository, IEmpCodeResolverService empCodeResolverService, ISalaryResolverService salaryResolverService, ILoggerProvider loggerProvider)
		{
			_DepartmentsRepository = departmentsRepository;
			_EmpCodeResolverService = empCodeResolverService;
			_SalaryResolverService = salaryResolverService;

			_Logger = loggerProvider.CreateLogger(GetType().FullName);
		}

		private readonly IDepartmentsRepository _DepartmentsRepository;

		private readonly IEmpCodeResolverService _EmpCodeResolverService;

		private readonly ISalaryResolverService _SalaryResolverService;

		private readonly ILogger _Logger;

		[HttpGet]
		[Route("{year}/{month}")]
		public async Task<string> CreateReport(int year, int month)
		{
			_Logger.LogDebug("Create report requested with {year} year and {month} month", year, month);

			var result = new StringBuilder();

			var russianCulture = CultureInfo.GetCultureInfo("ru-RU");
			var monthName = new DateTime(year, month, 1).ToString("MMMM", russianCulture);
			result.AppendLine($"{monthName} {year}");
			result.AppendLine("--------------------------------------------");

			var departments = (await _DepartmentsRepository.GetActiveDepartmentsWithEmployees())
				.ToArray();

			var salaries = new ConcurrentDictionary<Employee, decimal>();
			await Task.WhenAll(departments
				.SelectMany(department => department.Employees)
				.Select(async employee =>
				{
					var buhCode = await _EmpCodeResolverService.ResolveBuhCodeAsync(employee.Inn);
					var salary = await _SalaryResolverService.ResolveSalaryAsync(employee.Inn, buhCode);
					salaries.TryAdd(employee, salary);
				}));
			var sum = 0.0m;
			foreach (var department in departments)
			{
				result.AppendLine(department.Name);
				var perDepartmentSum = 0.0m;

				foreach (var employee in department.Employees)
				{
					var salary = salaries[employee];
					result.AppendLine($"{employee.Name} {salary.ToString("F0", russianCulture)}р");
					perDepartmentSum += salary;
				}

				result.AppendLine($"Всего по отделу {perDepartmentSum.ToString("F0", russianCulture)}р");
				sum += perDepartmentSum;
				result.AppendLine("--------------------------------------------");
			}

			result.Append($"Всего по предприятию {sum.ToString("F0", russianCulture)}р");
			return result.ToString();
		}
	}
}
