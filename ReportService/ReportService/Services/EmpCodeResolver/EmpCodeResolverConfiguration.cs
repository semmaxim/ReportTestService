namespace ReportService.Services.EmpCodeResolver
{
	public class EmpCodeResolverConfiguration : BaseResolverConfiguration
	{
		public static readonly string ConfigurationSectionName = "EmpCodeResolver";

		public string UrlTemplate { get; set; }
	}
}