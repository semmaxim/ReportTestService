using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using ReportService.Controllers;
using ReportService.Data.Domain;
using ReportService.Data.Services;
using ReportService.Services.EmpCodeResolver;
using ReportService.Services.SalaryResolver;
using Xunit;

namespace ReportService.Tests.Controllers
{
	public class ReportControllerTests
	{
		[Fact]
		public async Task CreateReport()
		{
			var innBuhCodeToSalary = new Dictionary<string, decimal>
			{
				{"I01-B01", 70000m},
				{"I02-B02", 65000m},
				{"I03-B03", 80000m},
				{"I04-B04", 90000m},
				{"I05-B05", 50000m},
				{"I06-B06", 55000m},
				{"I07-B07", 35000m},
				{"I08-B08", 90000m},
				{"I09-B09", 120000m},
				{"I10-B10", 110000m},
				{"I11-B11", 120000m},
			};

			var department1 = new Department {Name = "��������"};
			department1.Employees.AddRange(new[]
			{
				new Employee{ Name = "������ ��������� ������", Inn = "I01", Department = department1},
				new Employee{ Name = "�������� �������� ��������", Inn = "I02", Department = department1},
				new Employee{ Name = "���� ���������� ��������", Inn = "I03", Department = department1},
				new Employee{ Name = "������� �������� �����", Inn = "I04", Department = department1}
			});
			var department2 = new Department {Name = "�����������"};
			department2.Employees.AddRange(new[]
			{
				new Employee{ Name = "������� ���������� ��������", Inn = "I05", Department = department2},
				new Employee{ Name = "������ ��������� ����������", Inn = "I06", Department = department2},
				new Employee{ Name = "������ ��������� ������", Inn = "I07", Department = department2}
			});
			var department3 = new Department {Name = "��"};
			department3.Employees.AddRange(new[]
			{
				new Employee{ Name = "���� ��������� ������", Inn = "I08", Department = department3},
				new Employee{ Name = "������� ���������� ��������", Inn = "I09", Department = department3},
				new Employee{ Name = "������ �������� ���������", Inn = "I10", Department = department3},
				new Employee{ Name = "����� ������ ������", Inn = "I11", Department = department3}
			});

			var departmentRepository = new Mock<IDepartmentsRepository>();
			departmentRepository
				.Setup(repository => repository.GetActiveDepartmentsWithEmployees())
				.ReturnsAsync(new[] {department1, department2, department3});

			var empCodeResolverService = new Mock<IEmpCodeResolverService>();
			empCodeResolverService
				.Setup(service => service.ResolveBuhCodeAsync(It.IsAny<string>()))
				.ReturnsAsync((string inn) => "B" + inn.Substring(1, 2));

			var salaryResolverService = new Mock<ISalaryResolverService>();
			salaryResolverService
				.Setup(service => service.ResolveSalaryAsync(It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync((string inn, string buhCode) => innBuhCodeToSalary[$"{inn}-{buhCode}"]);

			var loggerProvider = new Mock<ILoggerProvider>();

			var controller = new ReportController(departmentRepository.Object, empCodeResolverService.Object, salaryResolverService.Object, loggerProvider.Object);
			var result = await controller.CreateReport(2019, 5);
			Assert.Equal(@"��� 2019
--------------------------------------------
��������
������ ��������� ������ 70000�
�������� �������� �������� 65000�
���� ���������� �������� 80000�
������� �������� ����� 90000�
����� �� ������ 305000�
--------------------------------------------
�����������
������� ���������� �������� 50000�
������ ��������� ���������� 55000�
������ ��������� ������ 35000�
����� �� ������ 140000�
--------------------------------------------
��
���� ��������� ������ 90000�
������� ���������� �������� 120000�
������ �������� ��������� 110000�
����� ������ ������ 120000�
����� �� ������ 440000�
--------------------------------------------
����� �� ����������� 885000�", result);
		}
	}
}
