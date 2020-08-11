using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ReportService.Services.SalaryResolver
{
	public class SalaryResolverService : BaseResolverByOuterService<SalaryResolverConfiguration>, ISalaryResolverService
	{
		public SalaryResolverService(IConfiguration configuration, ILoggerProvider loggerProvider) : base(configuration, loggerProvider, SalaryResolverConfiguration.ConfigurationSectionName)
		{
			if (String.IsNullOrWhiteSpace(Configuration.UrlTemplate))
				throw new ConfigurationErrorsException($"Empty UrlTemplate in \"{SalaryResolverConfiguration.ConfigurationSectionName}\" section.");
		}

		public async Task<decimal> ResolveSalaryAsync(string inn, string buhCode)
		{
			Logger.LogDebug("Requested salary by \"{buhCode}\" inn and \"{inn}\" buh code.", buhCode, inn);

			// ReSharper disable once ConvertToUsingDeclaration NOTE: Important syntax block - emulated limited critical section.
#pragma warning disable IDE0063
			using (await EnterConcurrentCriticalSection())
			{
				var url = String.Format(Configuration.UrlTemplate, inn);
				var webRequest = WebRequest.Create(url);
				webRequest.ContentType = "application/json";
				webRequest.Method = "POST";
				await using (var streamWriter = new StreamWriter(await webRequest.GetRequestStreamAsync()))
				{
					var json = JsonConvert.SerializeObject(new { BuhCode = buhCode });
					await streamWriter.WriteAsync(json);
				}

				Logger.LogTrace("Execute request to {url} by \"{inn}\" inn and \"{buhCode}\" buh code.", url, inn, buhCode);

				var response = await webRequest.GetResponseAsync();
				var responseStream = response.GetResponseStream();
				if (responseStream == null)
					throw new Exception($"Response from {url} are empty.");

				string responseText;
				using (var streamReader = new StreamReader(responseStream, true))
					responseText = await streamReader.ReadToEndAsync();

				if (!Decimal.TryParse(responseText, out var result))
					throw new Exception($"Can't parse \"{responseText}\" response from {url} as Decimal.");

				Logger.LogDebug("Response from {url} by \"{inn}\" inn and \"{buhCode}\" buh code is {result}.", url, buhCode, result);

				return result;
			}
		}
	}
}