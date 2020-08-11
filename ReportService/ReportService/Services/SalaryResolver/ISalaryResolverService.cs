using System.Threading.Tasks;

namespace ReportService.Services.SalaryResolver
{
	public interface ISalaryResolverService
	{
		Task<decimal> ResolveSalaryAsync(string inn, string buhCode);
	}
}