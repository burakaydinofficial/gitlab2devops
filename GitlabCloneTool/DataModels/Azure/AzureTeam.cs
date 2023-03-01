using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitlabCloneTool.DataModels.Azure
{
    internal class AzureTeamList : Common.AzureList<AzureTeam>
    {
    }

    internal class AzureTeam
    {
        public string id { get; set; }
        public string name { get; set; } // Max Length: 64
        public string url { get; set; }
        public string description { get; set; }
        public string identityUrl { get; set; }
        public string projectName { get; set; }
        public string projectId { get; set; }
    }

    internal class AzureProject
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string state { get; set; }
    }

    internal class AzureProjectList : Common.AzureList<AzureProject>
    {

    }


    internal class AzureCreateProject
    {
        public string name { get; set; }
        public string description { get; set; }
        public AzureProjectCapabilities capabilities { get; set; }
    }

    internal class AzureProjectCapabilities
    {
        public AzureProjectVersionControl versioncontrol { get; set; }
        public AzureProjectProcessTemplate processTemplate { get; set; }
    }

    internal class AzureProjectProcessTemplate
    {
        public string templateTypeId { get; set; }
    }

    internal class AzureProjectVersionControl
    {
        public string sourceControlType { get; set; }
    }

    internal class AzureCreateProjectResponse
    {
        public const string STATUS_cancelled = "cancelled";
        public const string STATUS_failed = "failed";
        public const string STATUS_inProgress = "inProgress";
        public const string STATUS_notSet = "notSet";
        public const string STATUS_queued = "queued";
        public const string STATUS_succeeded = "succeeded";

        public enum STATUS
        {
            cancelled,
            failed,
            inProgress,
            notSet,
            queued,
            succeeded
        }

        public string id { get; set; }
        public string status { get; set; }
        public string url { get; set; }

        public bool GetStatus(out STATUS st)
        {
            return Enum.TryParse(status, out st);
        }
    }
}