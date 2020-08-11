namespace ReportService.Services
{
	public abstract class BaseResolverConfiguration
	{
		public int MaximumNumberOfConcurrentAttempts { get; set; }
	}
}