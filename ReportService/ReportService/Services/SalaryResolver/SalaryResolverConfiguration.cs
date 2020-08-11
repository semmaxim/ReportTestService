namespace ReportService.Services.SalaryResolver
{
	public class SalaryResolverConfiguration : BaseResolverConfiguration
	{
		public static readonly string ConfigurationSectionName = "SalaryResolver";

		public string UrlTemplate { get; set; }
	}
}