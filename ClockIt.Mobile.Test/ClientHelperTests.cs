using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommandScheduling.Mobile.Helpers;
using Shouldly;
using System.Threading.Tasks;

namespace CommandScheduling.Mobile.Test
{
	[TestClass]
	public class ClientHelperTests
	{
		[TestMethod]
		public async Task ClientHelper_ShouldReturnANewHttpClientAsync()
		{
			var client = await ClientHelper.GetClient();

			client.ShouldNotBeNull();
			client.DefaultRequestHeaders.Contains("Accept").ShouldBeTrue();
		}

		[TestMethod]
		public async Task ClientHelper_ShouldReturnANewHttpClientWithAuthCookie()
		{
			ClientHelper.SetAuthCookie("authCoocke=authenticated");
			var client = await ClientHelper.GetClient();

			client.ShouldNotBeNull();
			client.DefaultRequestHeaders.Contains("Accept").ShouldBeTrue();
			client.DefaultRequestHeaders.Contains("Cookie").ShouldBeTrue();
		}
	}
}
