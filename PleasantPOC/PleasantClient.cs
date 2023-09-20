using Microsoft.Extensions.Configuration;
using PleasantPOC.Attributes;
using PleasantPOC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PleasantPOC
{
    public class PleasantClient
    {
        private readonly HttpClient _httpClient;
        private const string PleasantOTPProvierHeader = "X-Pleasant-OTP-Provider";
        private const string PleasantOTPHeader = "X-Pleasant-OTP";
        
        private readonly IConfiguration _config;
        public PleasantClient()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();
            string url = _config["PleasantServer:Url"] ?? string.Empty;
            _ = int.TryParse(_config["PleasantServer:Port"], out int port);
            _httpClient = new()
            {
                BaseAddress = new Uri($"{url}:{port}")
            };
        }

        public async Task LoginAsync(LoginModel login)
        {
            using HttpContent content = FormUrlEncodedContent(login);
            string route = "OAuth2/Token";

            HttpResponseMessage response = await _httpClient.PostAsync(route, content);
            response.Headers.TryGetValues(PleasantOTPProvierHeader, out IEnumerable<string?>? providers);

            if (providers is null || providers.Any() is false)
                throw new Exception($"Could not find any 2FA providers in the ${PleasantOTPProvierHeader} header or it was missing");

            content.Headers.Add(PleasantOTPHeader, login.TwoFactorToken);
            content.Headers.Add(PleasantOTPProvierHeader, providers);

            string test = await content.ReadAsStringAsync();

            response = await _httpClient.PostAsync(route, content);
            LoginResponseModel loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseModel>() ??
                throw new Exception("Could not extract access token from response");

            _httpClient.DefaultRequestHeaders.Authorization = new(loginResponse.TokenType, loginResponse.Token);
        }

        public async Task<CredentialGroup?> GetEnvironmentsCredentialGroupAsync()
        {
            if (Guid.TryParse(_config["PleasantServer:EnvironmentsCredentialGroupId"] ?? string.Empty, out Guid credentialGroupId))
                return await GetCredentialGroupAsync(credentialGroupId);

            return null;
        }

        private async Task<CredentialGroup> GetCredentialGroupAsync(Guid credentialGroupId)
        {
            CredentialGroup credentialGroup = await _httpClient
                .GetFromJsonAsync<CredentialGroup>($"api/v6/rest/folders/{credentialGroupId}") ??
                throw new Exception($"Could not find any credentialgroup with the following id: {credentialGroupId}");

            return credentialGroup;
        }

        public async Task<List<Credential>> GetEnvironmentCredentialsAsync(Guid credentialGroupId)
        {
            CredentialGroup credentialGroup = await GetCredentialGroupAsync(credentialGroupId);

            return credentialGroup.Credentials;
        }

        public async Task<Credential> GetCredentialsAsync(Guid credentialId)
        {
            return await _httpClient.GetFromJsonAsync<Credential>($"api/v6/rest/entries/{credentialId}") ??
                throw new Exception($"Could not find any entry with the id: {credentialId}");
        }

        private static FormUrlEncodedContent FormUrlEncodedContent(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            List<KeyValuePair<string, string>> values = new();
            foreach (PropertyInfo property in properties)
            {
                JsonPropertyNameAttribute? attribute = property
                    .GetCustomAttribute<JsonPropertyNameAttribute>();

                if (attribute is null)
                    continue;

                values.Add(new(attribute.Name, property.GetValue(obj) as string ?? string.Empty));
            }

            return new FormUrlEncodedContent(values);
        }
    }
}
