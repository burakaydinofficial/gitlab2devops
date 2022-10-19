using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GitlabCloneTool
{
    internal class AzureGroupsProcessor : GroupsProcessorBase
    {
        private const string AZURE_TEAMS_URL_FORMAT =
            "https://dev.azure.com/{0}/_apis/teams?api-version=6.0-preview.3";

        public AzureGroupsProcessor(MainConfig config) : base(config)
        {
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(
                        string.Format("{0}:{1}", /*config.AzureUserName*/ "", config.AzureAccessToken))));
        }

        public async Task<bool> GetTeams()
        {
            try
            {
                var url = string.Format(AZURE_TEAMS_URL_FORMAT, config.AzureOrganization);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }
    }
}