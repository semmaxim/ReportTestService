using System.Threading.Tasks;

namespace ReportService.Services.EmpCodeResolver
{
	public interface IEmpCodeResolverService
	{
		Task<string> ResolveBuhCodeAsync(string inn);
	}
}