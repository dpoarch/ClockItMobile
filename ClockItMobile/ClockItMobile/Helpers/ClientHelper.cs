using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClockIt.Mobile.Helpers
{
	public static class ClientHelper
	{
		static HttpClient _client;
        const string BASE_ADDRESS = "https://meshmanager-test.documents.azure.com/dbs/clockitdb/colls/clockitcol/docs";
        const string MASTER_KEY = "OJ8oELisTcC9L7ApU2yDS7PmvKxowtlskRVPVqhGnDq02yd4G3KZ2B5sukpd5FNt6smI8kvWnlq0s6NQoy5Oiw==";
        const string RESOURCE_TYPE = "docs";
        const string RESOURCE_ID = "dbs/clockitdb/colls/clockitcol";
        public const string GET = "get";
        public const string GET_ALL = "get_all";
        public const string POST = "post";
        public const string DELETE = "delete";
        public const string PUT = "put";
        static string authGetAll = "";
        static string dateNow = DateTime.UtcNow.ToString("r");


        public static HttpClient GetClient()
		{
			if (_client == null)
			{
                /*
				var cookieContainer = new CookieContainer();
				var handler = new HttpClientHandler
				{
					CookieContainer = cookieContainer
				};*/

                //_client = new HttpClient(handler);
                _client = new HttpClient();
                /*
                _client.BaseAddress = new Uri("https://clockittest-ef2a.restdb.io/rest/clockituserstest");
                _client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json");
                _client.DefaultRequestHeaders.TryAddWithoutValidation("x-apikey", "4b442b1280f788263d9f362ecbde750348a67");
                _client.DefaultRequestHeaders.TryAddWithoutValidation("cache-control", "no-cache");
                */
                //_client.BaseAddress = new Uri("https://meshmanager-test.documents.azure.com/dbs/clockitdb/colls/clockitcol/docs");
                //_client.BaseAddress = new Uri("https://meshmanager-test.documents.azure.com/dbs/clockitdb/colls/clockitcol/docs/NWJkY2I1M2UwN2JmODY2MWE4MDcwOTY2");
                //_client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json");
                //var authHeader = DependencyService.Get<ICryptoService>().Cipher("OJ8oELisTcC9L7ApU2yDS7PmvKxowtlskRVPVqhGnDq02yd4G3KZ2B5sukpd5FNt6smI8kvWnlq0s6NQoy5Oiw==", "DELETE", "docs", "dbs/clockitdb/colls/clockitcol/docs/NWJkY2I1M2UwN2JmODY2MWE4MDcwOTY2");
                //var authHeader = DependencyService.Get<ICryptoService>().Cipher("OJ8oELisTcC9L7ApU2yDS7PmvKxowtlskRVPVqhGnDq02yd4G3KZ2B5sukpd5FNt6smI8kvWnlq0s6NQoy5Oiw==", "GET", "docs", "dbs/clockitdb/colls/clockitcol");
                //_client.DefaultRequestHeaders.Add("Authorization", authHeader);

                _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-type", "application/json");
                _client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                _client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
                _client.DefaultRequestHeaders.TryAddWithoutValidation("Host", "meshmanager-test.documents.azure.com");
                _client.DefaultRequestHeaders.Add("x-ms-date", dateNow);
                _client.DefaultRequestHeaders.Add("x-ms-version", "2015-08-06");
                //_client.DefaultRequestHeaders.Add("Content-type", "application/json");

                //_client.DefaultRequestHeaders.TryAddWithoutValidation("cache-control", "no-cache");
            }

            return _client;
		}
        public static async Task SetAuthAndHeaders(string verb,string id)
        {
            var authHeader = "";
            if (verb == GET)
            {
                _client.BaseAddress = new Uri("https://meshmanager-test.documents.azure.com/dbs/clockitdb/colls/clockitcol/docs/"+id);
                authHeader = DependencyService.Get<ICryptoService>().Cipher(MASTER_KEY, "GET", RESOURCE_TYPE, RESOURCE_ID+"/docs/"+id,dateNow);
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", authHeader);

            }
            else if (verb == GET_ALL)
            {
                _client.BaseAddress = new Uri("https://meshmanager-test.documents.azure.com/dbs/clockitdb/colls/clockitcol/docs/");
                if (authGetAll == ""||true)
                {
                    authHeader = DependencyService.Get<ICryptoService>().Cipher(MASTER_KEY, "GET", RESOURCE_TYPE, RESOURCE_ID, dateNow);
                    authGetAll = authHeader;
                }
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", authHeader);

            }
            else if (verb == POST) {

                _client.BaseAddress = new Uri("https://meshmanager-test.documents.azure.com/dbs/clockitdb/colls/clockitcol/docs/");
                authHeader = DependencyService.Get<ICryptoService>().Cipher(MASTER_KEY, "POST", RESOURCE_TYPE, RESOURCE_ID, dateNow);
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", authHeader);
            }
            else if (verb == DELETE)
            {

                _client.BaseAddress = new Uri("https://meshmanager-test.documents.azure.com/dbs/clockitdb/colls/clockitcol/docs/" + id);
                authHeader = DependencyService.Get<ICryptoService>().Cipher(MASTER_KEY, "DELETE", RESOURCE_TYPE, RESOURCE_ID + "/docs/" + id, dateNow);
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", authHeader);
            }
            else if (verb == PUT)
            {

                _client.BaseAddress = new Uri("https://meshmanager-test.documents.azure.com/dbs/clockitdb/colls/clockitcol/docs/" + id);
                authHeader = DependencyService.Get<ICryptoService>().Cipher(MASTER_KEY, "PUT", RESOURCE_TYPE, RESOURCE_ID + "/docs/" + id,dateNow);
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", authHeader);
            }
        }
    }

}
