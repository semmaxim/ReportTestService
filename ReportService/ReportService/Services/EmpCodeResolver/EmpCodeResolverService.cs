using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ReportService.Services.EmpCodeResolver
{
	public class EmpCodeResolverService : BaseResolverByOuterService<EmpCodeResolverConfiguration>, IEmpCodeResolverService
	{
		public EmpCodeResolverService(IConfiguration configuration, ILoggerProvider loggerProvider) : base(configuration, loggerProvider, EmpCodeResolverConfiguration.ConfigurationSectionName)
		{
			if (String.IsNullOrWhiteSpace(Configuration.UrlTemplate))
				throw new ConfigurationErrorsException($"Empty UrlTemplate in \"{EmpCodeResolverConfiguration.ConfigurationSectionName}\" section.");
		}

		public async Task<string> ResolveBuhCodeAsync(string inn)
		{
			Logger.LogDebug("Requested buh code by \"{inn}\" inn.", inn);

			// ReSharper disable once ConvertToUsingDeclaration NOTE: Important syntax block - emulated limited critical section.
#pragma warning disable IDE0063
			using (await EnterConcurrentCriticalSection())
			{
				var url = String.Format(Configuration.UrlTemplate, inn);
				Logger.LogTrace("Execute request to {url} by {inn} inn.", url, inn);
				var result = await (new HttpClient()).GetStringAsync(url);
				Logger.LogDebug("Response request {result} by inn {inn}", result, inn);

				return result;
			}
		}
	}
}