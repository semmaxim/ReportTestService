using System.Collections.Generic;

namespace ReportService.Data.Domain
{
	public class Department
	{
		public string Name { get; set; }

		public List<Employee> Employees { get; } = new List<Employee>();
	}
}