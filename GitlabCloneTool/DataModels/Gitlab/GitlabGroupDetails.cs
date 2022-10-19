using System;
using System.Collections.Generic;

namespace GitlabCloneTool.DataModels.Gitlab
{
    public class GitlabGroupDetails
    {
        public int? id { get; set; }
        public string web_url { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string description { get; set; }
        public string visibility { get; set; }
        public bool? share_with_group_lock { get; set; }
        public bool? require_two_factor_authentication { get; set; }
        public int? two_factor_grace_period { get; set; }
        public string project_creation_level { get; set; }
        public object auto_devops_enabled { get; set; }
        public string subgroup_creation_level { get; set; }
        public object emails_disabled { get; set; }
        public object mentions_disabled { get; set; }
        public bool? lfs_enabled { get; set; }
        public int? default_branch_protection { get; set; }
        public object avatar_url { get; set; }
        public bool? request_access_enabled { get; set; }
        public string full_name { get; set; }
        public string full_path { get; set; }
        public DateTime? created_at { get; set; }
        public int? parent_id { get; set; }
        public object ldap_cn { get; set; }
        public object ldap_access { get; set; }
        public List<object> shared_with_groups { get; set; }
        public string runners_token { get; set; }
        public List<Project> projects { get; set; }
        public List<object> shared_projects { get; set; }
        public object shared_runners_minutes_limit { get; set; }
        public object extra_shared_runners_minutes_limit { get; set; }
        public object prevent_forking_outside_group { get; set; }
        public bool? membership_lock { get; set; }
    }
}