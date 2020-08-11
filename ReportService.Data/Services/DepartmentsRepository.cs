using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ReportService.Data.Domain;

namespace ReportService.Data.Services
{
	public class DepartmentsRepository : IDepartmentsRepository
	{
		public DepartmentsRepository(IDbConnectionFactory dbConnectionFactory)
		{
			_DbConnectionFactory = dbConnectionFactory;
		}

		private IDbConnectionFactory _DbConnectionFactory;

		public async Task<IEnumerable<Department>> GetActiveDepartmentsWithEmployees()
		{
			var connection = _DbConnectionFactory.GetConnection();
			var records = await connection.QueryAsync(
				@"SELECT
						UserName = e.name,
						Inn = e.inn,
						DepartmentName = d.name
					FROM
						emps e
						LEFT JOIN deps d ON e.departmentid = d.id
					WHERE
						d.active = TRUE
			");

			var result = new Dictionary<string, Department>();

			foreach (var record in records)
			{
				var departmentName = (string) record.DepartmentName ?? "Вне отдела";
				if (!result.TryGetValue(departmentName, out var department))
					result.Add(departmentName, department = new Department {Name = departmentName});

				department.Employees.Add(
					new Employee
					{
						Name = (string) record.UserName,
						Inn = (string) record.Inn,
						Department = department
					});
			}

			return result.Values;
		}
	}
}