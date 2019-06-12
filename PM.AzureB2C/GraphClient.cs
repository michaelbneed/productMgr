using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace PM.AzureB2C
{
    public class GraphClient
    {
        private readonly AuthenticationContext _authenticationContext;

        private readonly ClientCredential _clientCredential;

        private readonly string _tenant;

        private readonly string _azureActiveDirectoryInstance = "https://login.microsoftonline.com/";

        private readonly string _azureActiveDirectoryGraphEndpoint = "https://graph.windows.net";

        private readonly string _azureActiveDirectoryGraphVersion = "api-version=1.6";

        public GraphClient()
        {
            _tenant = ConfigurationManager.AppSettings["aad:Tenant"];

            _authenticationContext = new AuthenticationContext($"{_azureActiveDirectoryInstance}{_tenant}");

            _clientCredential = new ClientCredential(ConfigurationManager.AppSettings["aad:ClientId"], ConfigurationManager.AppSettings["aad:ClientSecret"]);
        }

        //public List<TradeCycle.Portal.Domain.Models.Azure.Group.SimpleViewModel> GetGroups()
        //{
        //    try
        //    {
        //        var result = JsonConvert.DeserializeObject<IEnumerable<TradeCycle.Portal.Domain.Models.Azure.Group.SimpleViewModel>>(Get("/groups", ""));
        //        return (List<TradeCycle.Portal.Domain.Models.Azure.Group.SimpleViewModel>)result;
        //    }
        //    catch
        //    {
        //        throw new JsonException();
        //    }
        //}

        public bool CreateUser(Models.User.Create model, out string objectId)
        {
            try
            {
                var result = JObject.Parse(Post("/users", JsonConvert.SerializeObject(model, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() })));

                objectId = result.Value<string>("objectId");

                return true;
            }
            catch
            {
                objectId = null;

                return false;
            }
        }

        public bool UpdateUser(string objectId, string json)
        {
            try
            {
                Patch($"/users/{objectId}", json);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool DeleteUser(string objectId)
        {
            try
            {
                Delete($"/users/{objectId}");

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string Get(string api, string query)
        {
            var url = $"{_azureActiveDirectoryGraphEndpoint}/{_tenant}{api}?{_azureActiveDirectoryGraphVersion}";

            if (string.IsNullOrEmpty(query) == false)
                url = $"{url}&{query}";

            var token = _authenticationContext.AcquireTokenAsync(_azureActiveDirectoryGraphEndpoint, _clientCredential).Result;
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var httpClient = new HttpClient();

            var response = httpClient.SendAsync(request).Result;

            if (response.IsSuccessStatusCode == false)
                throw new WebException(); // TODO: Use a custom exception that will log to NLog.

            return response.Content.ReadAsStringAsync().Result;
        }

        private string Post(string api, string json)
        {
            var url = $"{_azureActiveDirectoryGraphEndpoint}/{_tenant}{api}?{_azureActiveDirectoryGraphVersion}";
            var token = _authenticationContext.AcquireTokenAsync(_azureActiveDirectoryGraphEndpoint, _clientCredential).Result;
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();

            var response = httpClient.SendAsync(request).Result;

            if (response.IsSuccessStatusCode == false)
                throw new WebException(); // TODO: Use a custom exception that will log to NLog.

            return response.Content.ReadAsStringAsync().Result;
        }

        private string Patch(string api, string json)
        {
            var url = $"{_azureActiveDirectoryGraphEndpoint}/{_tenant}{api}?{_azureActiveDirectoryGraphVersion}";
            var token = _authenticationContext.AcquireTokenAsync(_azureActiveDirectoryGraphEndpoint, _clientCredential).Result;
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();

            var response = httpClient.SendAsync(request).Result;

            if (response.IsSuccessStatusCode == false)
                throw new WebException(); // TODO: Use a custom exception that will log to NLog.

            return response.Content.ReadAsStringAsync().Result;
        }

        private string Delete(string api)
        {
            var url = $"{_azureActiveDirectoryGraphEndpoint}/{_tenant}{api}?{_azureActiveDirectoryGraphVersion}";
            var token = _authenticationContext.AcquireTokenAsync(_azureActiveDirectoryGraphEndpoint, _clientCredential).Result;
            var request = new HttpRequestMessage(HttpMethod.Delete, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var httpClient = new HttpClient();

            var response = httpClient.SendAsync(request).Result;

            if (response.IsSuccessStatusCode == false)
                throw new WebException(); // TODO: Use a custom exception that will log to NLog.

            return response.Content.ReadAsStringAsync().Result;
        }

        // HACK: This is needed because the Microsoft.IdentityModel.Clients.ActiveDirectory.Platform DLL will not copy to the output folder.
        public static void DoNotRemoveThisFunctionOtherwiseYourLifeWillSuck()
        {
            var type = typeof(Microsoft.IdentityModel.Clients.ActiveDirectory.AdalOption);
        }
    }
}
