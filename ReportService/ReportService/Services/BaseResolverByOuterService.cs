using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ReportService.Services
{
	public abstract class BaseResolverByOuterService<TConfiguration> where TConfiguration: BaseResolverConfiguration, new()
	{
		protected BaseResolverByOuterService(IConfiguration configuration, ILoggerProvider loggerProvider, string configurationSectionName)
		{
			Logger = loggerProvider.CreateLogger(GetType().FullName);

			var section = configuration.GetSection(configurationSectionName);
			Logger.LogTrace("EmpCodeResolverService resolved. Read configuration section with value {sectionValue}", section.Value);
			section.Bind(Configuration);
			if (Configuration.MaximumNumberOfConcurrentAttempts <=0)
				throw new ConfigurationErrorsException($"Wrong MaximumNumberOfConcurrentAttempts = \"{Configuration.MaximumNumberOfConcurrentAttempts}\" in \"{configurationSectionName}\" section.");

			_Semaphore = new SemaphoreSlim(Configuration.MaximumNumberOfConcurrentAttempts, Configuration.MaximumNumberOfConcurrentAttempts);
		}
		
		protected ILogger Logger { get; }

		protected TConfiguration Configuration { get; } = new TConfiguration();

		private readonly SemaphoreSlim _Semaphore;

		private class ConcurrentLimiterToken : IDisposable
		{
			public ConcurrentLimiterToken(SemaphoreSlim semaphore)
			{
				_Semaphore = semaphore;
			}

			private readonly SemaphoreSlim _Semaphore;

			public async Task InitAsync()
			{
				await _Semaphore.WaitAsync();
			}

			public void Dispose()
			{
				_Semaphore.Release();
			}
		}

		protected async Task<IDisposable> EnterConcurrentCriticalSection()
		{
			var token = new ConcurrentLimiterToken(_Semaphore);
			await token.InitAsync();
			return token;
		}
	}
}