using System.Net.Http;
using System.Threading.Tasks;
using GitlabCloneTool.DataModels.Gitlab;
using Newtonsoft.Json;

namespace GitlabCloneTool
{
    internal class GitlabGroupsProcessor
    {
        private const string GITLAB_GROUPS_URL = "https://gitlab.com/api/v4/groups/";
        private const string GITLAB_GROUP_URL_FORMAT = "https://gitlab.com/api/v4/groups/{0}";
        private const string FILE_GROUPS_RAW = "groups_raw.json";
        private const string FILE_GROUPS = "groups.json";
        private readonly MainConfig config;
        private readonly HttpClient client;

        public string GroupsJson;
        public GroupInfo[] Groups;

        public GitlabGroupsProcessor(MainConfig config)
        {
            this.config = config;
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", config.PrivateToken);
        }

        public async Task DownloadAndProcess()
        {
            var response = await client.GetAsync(GITLAB_GROUPS_URL);
            GroupsJson = await response.Content.ReadAsStringAsync();
            var rawGroups = JsonConvert.DeserializeObject<GitlabGroup[]>(GroupsJson);
            Utilities.WriteAllText(config.CloneDirectory, FILE_GROUPS_RAW, GroupsJson);
            CreateGroupsInfo(rawGroups);
            WriteGroupsInfo();
            await GetGroupDetails();
        }

        private bool CreateGroupsInfo(GitlabGroup[] groups)
        {
            Groups = new GroupInfo[groups.Length];
            for (var i = 0; i < groups.Length; i++)
            {
                var go = new GroupInfo();
                go.Input = new GroupInfo.InputInfo(groups[i]);
                go.Store = new GroupInfo.StoreInfo(go.Input.RawGitlabInfo.full_path);
                Groups[i] = go;
            }
            return true;
        }

        private bool WriteGroupsInfo()
        {
            return Utilities.WriteAllText(config.CloneDirectory, FILE_GROUPS,
                JsonConvert.SerializeObject(Groups, Formatting.Indented));
        }

        private async Task<bool> GetGroupDetails()
        {
            for (var i = 0; i < Groups.Length; i++)
            {
                var group = Groups[i];
                var groupUrl = string.Format(GITLAB_GROUP_URL_FORMAT, group.Input.RawGitlabInfo.id);
                var response = await client.GetAsync(groupUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
            }

            return true;
        }
    }
}