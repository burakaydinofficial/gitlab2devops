using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace GitlabCloneTool
{
    class Program
    {
        private const string CONFIG_PATH = "config.json";

        static async Task Main(string[] args)
        {
            MainConfig config = TryParseConfig();
            if (config == null)
            {
                Console.WriteLine("Please Edit " + CONFIG_PATH + " (" + Path.Combine(Directory.GetCurrentDirectory(), CONFIG_PATH) +")");
                Console.ReadLine();
                return;
            }

            //int mode;
            //if (!ReadUserSelection(out mode))
            //{
            //    Console.WriteLine("Invalid Selection");
            //    Console.ReadLine();
            //    return;
            //}

            var groupProcessor = new GitlabGroupsProcessor(config);
            await groupProcessor.DownloadAndProcess();

            Console.ReadLine();
        }

        private static bool ReadUserSelection(out int mode)
        {
            Console.WriteLine("Download Group List: 1\nDownload Project List: 2\nClone Projects: 3\nCreate Projects On Azure: 4\nCreate Repositories On Azure\nUpload Projects: 5");
            var input = Console.ReadLine();
            return Int32.TryParse(input, out mode);
        }


        public static MainConfig TryParseConfig()
        {
            try
            {
                if (File.Exists(CONFIG_PATH))
                {
                    var json = File.ReadAllText(CONFIG_PATH);
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return null;
                    }

                    var config = MainConfig.Parse(json);
                    return config;
                }
                else
                {
                    File.WriteAllText(CONFIG_PATH, new MainConfig().Serialize());
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                File.WriteAllText(CONFIG_PATH, new MainConfig().Serialize());
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}
