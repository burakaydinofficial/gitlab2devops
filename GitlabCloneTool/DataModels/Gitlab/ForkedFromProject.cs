using System;
using System.Collections.Generic;

namespace GitlabCloneTool.DataModels.Gitlab
{
    public class ForkedFromProject
    {
        public int? id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string name_with_namespace { get; set; }
        public string path { get; set; }
        public string path_with_namespace { get; set; }
        public DateTime? created_at { get; set; }
        public string default_branch { get; set; }
        public List<object> tag_list { get; set; }
        public List<object> topics { get; set; }
        public string ssh_url_to_repo { get; set; }
        public string http_url_to_repo { get; set; }
        public string web_url { get; set; }
        public object readme_url { get; set; }
        public object avatar_url { get; set; }
        public int? forks_count { get; set; }
        public int? star_count { get; set; }
        public DateTime? last_activity_at { get; set; }
        public Namespace @namespace { get; set; }
    }
}