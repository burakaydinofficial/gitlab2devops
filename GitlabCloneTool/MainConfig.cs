using System;
using Newtonsoft.Json;

namespace GitlabCloneTool
{
    internal class MainConfig
    {
        public string PrivateToken = " ";
        public string CloneDirectory = " ";

        public static MainConfig Parse(string json)
        {
            try
            {
                var config = JsonConvert.DeserializeObject<MainConfig>(json);
                if (config == null || string.IsNullOrWhiteSpace(config.PrivateToken) ||
                    string.IsNullOrWhiteSpace(config.CloneDirectory))
                    return null;
                return config;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}