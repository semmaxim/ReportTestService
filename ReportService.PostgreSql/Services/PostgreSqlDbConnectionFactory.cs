using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ReportService.Data.Services;

namespace ReportService.PostgreSql.Services
{
	public class PostgreSqlDbConnectionFactory : IDbConnectionFactory
	{
		public PostgreSqlDbConnectionFactory(IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString(IDbConnectionFactory.ConnectionStringName);
			_ConnectionCreator = new Lazy<IDbConnection>(() =>
			{
				var connection = new NpgsqlConnection(connectionString);
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