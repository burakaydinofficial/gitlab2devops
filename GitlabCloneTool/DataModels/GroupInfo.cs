using System;
using System.Collections.Generic;
using GitlabCloneTool.DataModels.Gitlab;

namespace GitlabCloneTool.DataModels
{
    internal class GroupInfo
    {
        public InputInfo Input;
        public StoreInfo Store;
        public OutputInfo Output;

        public class InputInfo
        {
            public GitlabGroup RawGitlabInfo;
            public GitlabGroupDetails RawGitlabDetails;

            public InputInfo(GitlabGroup rawGitlabInfo)
            {
                RawGitlabInfo = rawGitlabInfo;
            }
        }

        public class StoreInfo
        {
            public string Root;
            public List<Repository> Repositories = new List<Repository>();

            public StoreInfo(string root)
            {
                Root = root;
            }

            public class Repository
            {
                public int Id;
                public bool Clone = true;
                public string CloneUrl;
                public string LocalPath;
                public string Name;
                public Project RepositoryProject;

                public DateTime CloneTimestamp = DateTime.MinValue;

                public DateTime LastFetchTimestamp = DateTime.MinValue;
                public bool LastFetchSuccessful;
                public DateTime LastSuccessfulFetch = DateTime.MinValue;

                public DateTime LastFetchLFSTimestamp = DateTime.MinValue;
                public bool LastFetchLFSSuccessful;
                public DateTime LastSuccessfulFetchLFSTimestamp = DateTime.MinValue;

                public Repository(Project repositoryProject)
                {
                    RepositoryProject = repositoryProject;
                    if (repositoryProject.id.HasValue)
                        Id = repositoryProject.id.Value;
                    CloneUrl = repositoryProject.http_url_to_repo;
                    LocalPath = repositoryProject.path;
                    Name = repositoryProject.name;
                }
            }
        }

        public class OutputInfo
        {

        }
    }
}