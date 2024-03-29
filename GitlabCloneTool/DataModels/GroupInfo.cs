﻿using System;
using System.Collections.Generic;
using GitlabCloneTool.DataModels.Azure;
using GitlabCloneTool.DataModels.Gitlab;

namespace GitlabCloneTool.DataModels
{
    internal class GroupInfoList
    {
        public List<GroupInfo> Groups;
        public AzureTeamList RawAzureTeamList;
        public AzureProjectList RawAzureProjectList;
    }

    internal class GroupInfo
    {
        public InputInfo Input;
        public StoreInfo Store;
        public OutputInfo Output = new OutputInfo();

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

                public DateTime? CloneTimestamp = null;

                public DateTime? LastFetchTimestamp = null;
                public bool LastFetchSuccessful;
                public DateTime? LastSuccessfulFetch = null;

                public DateTime? LastFetchLFSTimestamp = null;
                public bool LastFetchLFSSuccessful;
                public DateTime? LastSuccessfulFetchLFSTimestamp = null;

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
            public List<Match> Matches = new List<Match>();
            public class Match
            {
                public int FromId;
                public string ToId;
            }

            public Dictionary<int, string> CreateDictionary()
            {
                var dict = new Dictionary<int, string>();
                foreach (var match in Matches)
                {
                    dict[match.FromId] = match.ToId;
                }
                return dict;
            }
        }
    }
}