using System.Data;

namespace ReportService.Data.Services
{
	public interface IDbConnectionFactory
	{
		IDbConnection GetConnection();

		const string ConnectionStringName = "DefaultConnection";
	}
}