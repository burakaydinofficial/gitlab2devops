using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using GitlabCloneTool.DataModels;
using GitlabCloneTool.DataModels.Gitlab;
using Newtonsoft.Json;

namespace GitlabCloneTool
{
    internal class GitlabGroupsProcessor
    {
        private const string GITLAB_GROUPS_URL = "https://gitlab.com/api/v4/groups?per_page=500";
        private const string GITLAB_GROUP_URL_FORMAT = "https://gitlab.com/api/v4/groups/{0}";
        private const string FILE_GROUPS_RAW = "groups_raw.json";
        private const string FILE_GROUPS = "groups.json";
        private const string DIRECTORY_GROUPS = "Groups";
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

        public async Task CreateAndProcessGroups()
        {
            Console.WriteLine("Getting Group List");
            var response = await client.GetAsync(GITLAB_GROUPS_URL);
            GroupsJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Parsing Group List");
            var rawGroups = JsonConvert.DeserializeObject<GitlabGroup[]>(GroupsJson);
            Utilities.WriteAllText(config.CloneDirectory, FILE_GROUPS_RAW, GroupsJson);
            Console.WriteLine("Processing Group List");
            CreateGroupsInfo(rawGroups);
            WriteGroupsInfo();
            Console.WriteLine("Getting Group Details and Project Lists");
            await GetGroupDetails();
            WriteGroupsInfo();
            Console.WriteLine("Creating Group Folders");
            CreateGroupFolders();
            Console.WriteLine("Creating Project Folders");
            CreateProjectFolders();
            Console.WriteLine("Phase 1 - Finished");
        }

        public async Task CloneProjects()
        {
            ReadGroupsInfo();
            await CloneAllProjects();
            WriteGroupsInfo();
            DownloadReport();
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

        private bool ReadGroupsInfo()
        {
            Groups = JsonConvert.DeserializeObject<GroupInfo[]>(
                File.ReadAllText(Path.Combine(config.CloneDirectory, FILE_GROUPS)));
            return true;
        }

        private bool WriteGroupsInfo()
        {
            return Utilities.WriteAllText(config.CloneDirectory, FILE_GROUPS,
                JsonConvert.SerializeObject(Groups, Formatting.Indented));
        }

        private async Task<bool> GetGroupDetails()
        {
            try
            {
                for (var i = 0; i < Groups.Length; i++)
                {
                    var group = Groups[i];
                    var groupUrl = string.Format(GITLAB_GROUP_URL_FORMAT, group.Input.RawGitlabInfo.id);
                    var response = await client.GetAsync(groupUrl);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var groupDetails = JsonConvert.DeserializeObject<GitlabGroupDetails>(responseContent);
                    group.Input.RawGitlabDetails = groupDetails;
                    foreach (var project in groupDetails.projects)
                    {
                        if (!group.Store.Repositories.Exists(x => x.Id == project.id))
                        {
                            group.Store.Repositories.Add(new GroupInfo.StoreInfo.Repository(project));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        private bool CreateGroupFolders()
        {
            string baseDir = Path.Combine(config.CloneDirectory, DIRECTORY_GROUPS);
            for (var i = 0; i < Groups.Length; i++)
            {
                var group = Groups[i];
                string groupDir = Path.Combine(baseDir, group.Store.Root);
                if (!Directory.Exists(groupDir))
                    Directory.CreateDirectory(groupDir);
            }

            return true;
        }

        private bool CreateProjectFolders()
        {
            string baseDir = Path.Combine(config.CloneDirectory, DIRECTORY_GROUPS);
            for (var i = 0; i < Groups.Length; i++)
            {
                var group = Groups[i];
                string groupDir = Path.Combine(baseDir, group.Store.Root);
                foreach (var project in @group.Input.RawGitlabDetails.projects)
                {
                    var projectDir = Path.Combine(groupDir, project.path);
                    if (!Directory.Exists(projectDir))
                        Directory.CreateDirectory(projectDir);
                }
            }

            return true;
        }

        private async Task<bool> CloneAllProjects()
        {
            string baseDir = Path.Combine(config.CloneDirectory, DIRECTORY_GROUPS);
            for (var i = 0; i < Groups.Length; i++)
            {
                var group = Groups[i];
                string groupDir = Path.Combine(baseDir, group.Store.Root);
                Console.WriteLine("Group " + group.Input.RawGitlabDetails.name);
                var taskIndex = 0;
                List<Task<bool>> tasks = new List<Task<bool>>();
                foreach (var project in @group.Input.RawGitlabDetails.projects)
                {
                    var taskTag = "Task: " + taskIndex + " ||| ";
                    var projectReference = group.Store.Repositories.Find(x => x.Id == project.id);
                    Console.WriteLine(taskTag + "Project " + projectReference.Name);

                    if (!projectReference.Clone)
                    {
                        Console.WriteLine(taskTag + "Skipping Project. Clone Disabled in group json file");
                        continue;
                    }

                    var projectDir = Path.Combine(groupDir, projectReference.LocalPath);
                    var task = CloneAndFetchProject(projectReference, projectDir, taskTag);
                    tasks.Add(task);
                    taskIndex++;
                }
                foreach (var task in tasks)
                {
                    await task;
                }
            }

            return true;
        }

        private async Task<bool> CloneAndFetchProject(GroupInfo.StoreInfo.Repository projectReference, string projectDir, string taskTag)
        {
            Console.WriteLine(taskTag + "Cloning " + projectReference.CloneUrl);
            var cloningResult = await Utilities.Git.CloneProject(projectReference.CloneUrl, projectDir);
            if (cloningResult.HasValue && !cloningResult.Value)
            {
                Console.WriteLine(taskTag + "Cannot Clone Project");
                return false;
            }
            else if (!cloningResult.HasValue)
            {
                Console.WriteLine(taskTag + "Skipping Clone");
            }
            else
            {
                projectReference.CloneTimestamp = DateTime.Now;
            }

            Console.WriteLine(taskTag + "Fetching " + projectReference.CloneUrl);
            if (!await Utilities.Git.FetchProject(projectDir))
            {
                projectReference.LastFetchTimestamp = DateTime.Now;
                projectReference.LastFetchSuccessful = false;
                Console.WriteLine(taskTag + "Cannot Fetch Project");
                return false;
            }
            else
            {
                projectReference.LastFetchTimestamp = DateTime.Now;
                projectReference.LastFetchSuccessful = true;
                projectReference.LastSuccessfulFetch = DateTime.Now;
            }
            Console.WriteLine(taskTag + "Fetching LFS for " + projectReference.CloneUrl);
            if (!await Utilities.Git.FetchProjectLFS(projectDir))
            {
                projectReference.LastFetchLFSTimestamp = DateTime.Now;
                projectReference.LastFetchLFSSuccessful = false;
                Console.WriteLine(taskTag + "Cannot Fetch LFS Project");
                return false;
            }
            else
            {
                projectReference.LastFetchLFSTimestamp = DateTime.Now;
                projectReference.LastFetchLFSSuccessful = true;
                projectReference.LastSuccessfulFetchLFSTimestamp = DateTime.Now;
            }
            return true;
        }


        private void DownloadReport()
        {
            Console.WriteLine();
            Console.WriteLine("---------- Download Report ----------");
            int projectCount = 0;
            int successTotal = 0;
            Console.WriteLine();
            List<GroupInfo.StoreInfo.Repository> failedRepositories = new List<GroupInfo.StoreInfo.Repository>();
            for (var i = 0; i < Groups.Length; i++)
            {
                var group = Groups[i];
                Console.WriteLine(" - " + group.Input.RawGitlabDetails.full_name);
                var groupProjects = group.Store.Repositories;
                var successCount = groupProjects.FindAll(x => x.LastFetchSuccessful && x.LastFetchLFSSuccessful).Count;
                failedRepositories.AddRange(groupProjects.FindAll(x => !x.LastFetchSuccessful || !x.LastFetchLFSSuccessful));
                successTotal += successCount;
                projectCount += groupProjects.Count;
                Console.WriteLine("   Project Count: " + groupProjects.Count + " Fetch Success: " + successCount);
                Console.WriteLine();
            }
            Console.WriteLine("------ Download Report Summary ------");
            Console.WriteLine("Total Project Count: " + projectCount + " Success Count: " + successTotal);
            Console.WriteLine();
            Console.WriteLine("Failed Projects: ");
            foreach (var failedRepository in failedRepositories)
            {
                Console.WriteLine(failedRepository.Name + "\t" + failedRepository.RepositoryProject.path);
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine();
        }
    }
}