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

            public StoreInfo(string root)
            {
                Root = root;
            }
        }

        public class OutputInfo
        {

        }
    }
}