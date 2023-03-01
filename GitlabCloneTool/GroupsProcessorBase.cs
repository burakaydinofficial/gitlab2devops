using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using GitlabCloneTool.DataModels;
using Newtonsoft.Json;

namespace GitlabCloneTool
{
    internal abstract class GroupsProcessorBase
    {
        protected const string FILE_GROUPS_RAW = "groups_raw.json";
        protected const string FILE_GROUPS = "groups.json";
        protected const string DIRECTORY_GROUPS = "Groups";
        protected readonly MainConfig config;
        protected readonly HttpClient client;
        public GroupInfoList Data;
        public List<GroupInfo> Groups => Data.Groups;

        public GroupsProcessorBase(MainConfig config)
        {
            this.config = config;
            client = new HttpClient();
        }

        protected bool ReadGroupsInfo()
        {
            var path = Path.Combine(config.CloneDirectory, FILE_GROUPS);
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                Data = JsonConvert.DeserializeObject<GroupInfoList>(json);
            }
            return true;
        }

        protected bool WriteGroupsInfo()
        {
            return Utilities.WriteAllText(config.CloneDirectory, FILE_GROUPS,
                JsonConvert.SerializeObject(Data, Formatting.Indented));
        }
    }
}