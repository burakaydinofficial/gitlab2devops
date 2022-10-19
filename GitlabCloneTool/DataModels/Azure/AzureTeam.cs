using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitlabCloneTool.DataModels.Azure
{
    internal class AzureTeamList
    {
        public List<AzureTeam> value { get; set; }
        public int count { get; set; }
    }

    internal class AzureTeam
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string identityUrl { get; set; }
        public string projectName { get; set; }
        public string projectId { get; set; }
    }
}