using System.Collections.Generic;
using System.Threading.Tasks;
using ReportService.Data.Domain;

namespace ReportService.Data.Services
{
	public interface IDepartmentsRepository
	{
		Task<IEnumerable<Department>> GetActiveDepartmentsWithEmployees();
	}
}