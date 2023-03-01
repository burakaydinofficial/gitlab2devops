using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitlabCloneTool.DataModels
{
    internal class Common
    {
        public abstract class AzureList<T>
        {
            public int count;
            public List<T> value;
        }
    }
}
