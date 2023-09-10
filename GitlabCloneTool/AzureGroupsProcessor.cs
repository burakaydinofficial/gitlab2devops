using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GitlabCloneTool.DataModels;
using GitlabCloneTool.DataModels.Azure;
using Newtonsoft.Json;

namespace GitlabCloneTool
{
    internal class AzureGroupsProcessor : GroupsProcessorBase
    {
        private const string AZURE_TEAMS_URL_FORMAT =
            "https://dev.azure.com/{0}/_apis/teams?api-version=6.0-preview.3";

        private const string AZURE_PROJECTS_URL_FORMAT =
            "https://dev.azure.com/{0}/_apis/projects?api-version=6.0";

        private const string AZURE_CREATE_PROJECT_URL_FORMAT =
            "https://dev.azure.com/{0}/_apis/projects?api-version=6.0";

        public AzureGroupsProcessor(MainConfig config) : base(config)
        {
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(
                        string.Format("{0}:{1}", /*config.AzureUserName*/ "", config.AzureAccessToken))));
        }

        public async Task<AzureTeamList> GetTeams()
        {
            try
            {
                var url = string.Format(AZURE_TEAMS_URL_FORMAT, config.AzureOrganization);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var teamList = JsonConvert.DeserializeObject<AzureTeamList>(responseContent);
                ReadGroupsInfo();
                Data.RawAzureTeamList = teamList;
                WriteGroupsInfo();
                return teamList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public async Task<AzureProjectList> GetProjects()
        {
            try
            {
                var url = string.Format(AZURE_PROJECTS_URL_FORMAT, config.AzureOrganization);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var projectList = JsonConvert.DeserializeObject<AzureProjectList>(responseContent);
                ReadGroupsInfo();
                Data.RawAzureProjectList = projectList;
                WriteGroupsInfo();
                return projectList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public async Task<AzureCreateProjectResponse> CreateProject(AzureCreateProject projectInfo)
        {
            try
            {
                var url = string.Format(AZURE_CREATE_PROJECT_URL_FORMAT, config.AzureOrganization);
                var request = HttpWebRequest.Create(url);
                request.Method = "POST";

                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.NullValueHandling = NullValueHandling.Ignore;

                var json = JsonConvert.SerializeObject(projectInfo, Formatting.Indented, serializerSettings);
                var data = Encoding.UTF8.GetBytes(json);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                var reqStream = await request.GetRequestStreamAsync();
                await reqStream.WriteAsync(data, 0, data.Length);
                var response = await request.GetResponseAsync();
                var respStream = response.GetResponseStream();
                var reader = new StreamReader(respStream);
                
                var responseContent = await reader.ReadToEndAsync();
                var responseObj = JsonConvert.DeserializeObject<AzureCreateProjectResponse>(responseContent);
                return responseObj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public async Task<bool> CreateMatchingProjects()
        {
            try
            {
                ReadGroupsInfo();
                foreach (var dataGroup in Data.Groups)
                {
                    if (dataGroup.Output == null)
                        dataGroup.Output = new GroupInfo.OutputInfo();

                    var currentDict = dataGroup.Output.CreateDictionary();
                    foreach (var repository in dataGroup.Store.Repositories)
                    {
                        if (currentDict.ContainsKey(repository.Id))
                            continue;

                        var proj = new AzureCreateProject();
                        proj.description = repository.LocalPath;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }
    }
}