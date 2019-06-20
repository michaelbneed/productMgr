using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PM.Auth.GraphApi.Group;
using PM.Auth.GraphApi.User;

namespace PM.Auth.GraphApi
{
    public class GraphClient
    {
	    public IConfiguration Configuration;

	    private readonly AuthenticationContext _authenticationContext;
		private readonly ClientCredential _clientCredential;

		private readonly string _tenant;
		private readonly string _clientId;
		private readonly string _clientSecret;
	    private readonly string _instance;
	    private readonly string _graphEndpoint;
		private readonly string _graphVersion;

		
	    public GraphClient(IConfiguration configuration, bool useB2CInstance)
	    {
		    Configuration = configuration;
		    
			_tenant = useB2CInstance ? configuration.GetValue<string>("B2CGraphApi:Tenant") : configuration.GetValue<string>("AADGraphApi:Tenant");
			_clientId = useB2CInstance ? configuration.GetValue<string>("B2CGraphApi:ClientId") : configuration.GetValue<string>("AADGraphApi:ClientId"); 
			_clientSecret = useB2CInstance ? configuration.GetValue<string>("B2CGraphApi:ClientSecret") : configuration.GetValue<string>("AADGraphApi:ClientSecret");
			_instance = useB2CInstance ? configuration.GetValue<string>("B2CGraphApi:Instance") : configuration.GetValue<string>("AADGraphApi:Instance");
		    _graphEndpoint = useB2CInstance ? configuration.GetValue<string>("B2CGraphApi:GraphEndpoint") : configuration.GetValue<string>("AADGraphApi:GraphEndpoint");
			_graphVersion = useB2CInstance ? configuration.GetValue<string>("B2CGraphApi:GraphVersion") : configuration.GetValue<string>("AADGraphApi:GraphVersion");

			_authenticationContext = new AuthenticationContext($"{_instance}{_tenant}");

			_clientCredential = new ClientCredential(_clientId, _clientSecret);
        }

		public List<SimpleViewModel> GetGroups()
		{
			try
			{
				var result = JsonConvert.DeserializeObject<IEnumerable<SimpleViewModel>>(Get("/groups", ""));
				return (List<SimpleViewModel>)result;
			}
			catch
			{
				throw new JsonException();
			}
		}

		public bool CreateUser(Entity.Models.User model, out string password)
        {
            try
            {
                password = PasswordGenerator.Generate();

                var graphModel = new Create()
                {
                    AccountEnabled = true,
                    GivenName = model.FirstName,
                    Surname = model.LastName,
                    PasswordProfile = new PasswordProfile() { Password = password },
                    SignInNames = new List<SignInName>() { new SignInName() { Value = model.EmailAddress } }
                };
                var result = JObject.Parse(Post("/users", JsonConvert.SerializeObject(graphModel, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() })));

                model.AuthId = result.Value<string>("objectId");

                return true;
            }
            catch(Exception e)
            {
                model.AuthId = null;

                password = null;

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

        public string GetUserEmail(string objectId)
        {
            try
            {
                var result = JObject.Parse(Get($"/users/{objectId}"));

                return result.Value<string>("mail");
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<string> GetGroupUsersEmail(string groupId)
        {
            var result = new List<string>();

            try
            {
                var users = JsonConvert.DeserializeObject<GroupMembersResponse>(Get($"/groups/{groupId}/$links/members", null));

                foreach (var item in users.Urls)
                {
                    var userResult = JObject.Parse(Get(item.Url));

                    result.Add(userResult.Value<string>("userPrincipalName"));
                }

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private string Get(string api, string query)
        {
	        
			var url = $"{_graphEndpoint}/{_tenant}{api}?{_graphVersion}";

            if (string.IsNullOrEmpty(query) == false)
                url = $"{url}&{query}";

            var token = _authenticationContext.AcquireTokenAsync(_graphEndpoint, _clientCredential).Result;
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var httpClient = new HttpClient();

            var response = httpClient.SendAsync(request).Result;

            if (response.IsSuccessStatusCode == false)
                throw new WebException(); // TODO: Use a custom exception that will log to NLog.

            return response.Content.ReadAsStringAsync().Result;
        }

        private string Get(string url)
        {
            url = $"{url}?{_graphVersion}";

            var token = _authenticationContext.AcquireTokenAsync(_graphEndpoint, _clientCredential).Result;
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
            var url = $"{_graphEndpoint}/{_tenant}{api}?{_graphVersion}";
            var token = _authenticationContext.AcquireTokenAsync(_graphEndpoint, _clientCredential).Result;
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
            var url = $"{_graphEndpoint}/{_tenant}{api}?{_graphVersion}";
            var token = _authenticationContext.AcquireTokenAsync(_graphEndpoint, _clientCredential).Result;
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
            var url = $"{_graphEndpoint}/{_tenant}{api}?{_graphVersion}";
            var token = _authenticationContext.AcquireTokenAsync(_graphEndpoint, _clientCredential).Result;
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
