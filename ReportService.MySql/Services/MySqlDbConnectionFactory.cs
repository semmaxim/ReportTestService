using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ReportService.Data.Services;

namespace ReportService.MySql.Services
{
	public class MySqlDbConnectionFactory : IDbConnectionFactory
	{
		public MySqlDbConnectionFactory(IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString(IDbConnectionFactory.ConnectionStringName);
			_ConnectionCreator = new Lazy<IDbConnection>(() =>
			{
				var connection = new MySqlConnection(connectionString);
				connection.Open();
				return connection;
			}, true);
		}

		private readonly Lazy<IDbConnection> _ConnectionCreator;

		public IDbConnection GetConnection()
		{
			return _ConnectionCreator.Value;
		}
	}
}