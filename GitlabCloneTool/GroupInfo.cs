using GitlabCloneTool.DataModels.Gitlab;

namespace GitlabCloneTool
{
    internal class GroupInfo
    {
        public InputInfo Input;
        public StoreInfo Store;
        public OutputInfo Output;

        public class InputInfo
        {
            public GitlabGroup RawGitlabInfo;

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