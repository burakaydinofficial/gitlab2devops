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
                Console.WriteLine("Please Edit " + CONFIG_PATH + " (" + Path.Combine(Directory.GetCurrentDirectory(), CONFIG_PATH) + ")");
                Console.ReadLine();
                return;
            }

            int mode;
            do
            {
               mode = await ReadUserSelectionAndExecute(config);
            } while (mode != 6);
        }

        private static async Task<int> ReadUserSelectionAndExecute(MainConfig config)
        {
            int mode;
            if (!ReadUserSelection(out mode))
            {
                Console.WriteLine("Invalid Selection");
                Console.ReadLine();
                return mode;
            }

            Console.WriteLine();

            switch (mode)
            {
                case 0:
                {
                    var groupProcessor = new GitlabGroupsProcessor(config);
                    await groupProcessor.CreateAndProcessGroups();
                    await groupProcessor.CloneProjects();
                    break;
                }
                case 1:
                {
                    var groupProcessor = new GitlabGroupsProcessor(config);
                    await groupProcessor.CreateAndProcessGroups();
                    break;
                }
                case 2:
                {
                    var groupProcessor = new GitlabGroupsProcessor(config);
                    await groupProcessor.CloneProjects();
                    break;
                }
                case 3:
                {
                    var groupProcessor = new AzureGroupsProcessor(config);
                    await groupProcessor.GetProjects();
                    break;
                }
                default:
                    Console.WriteLine("Finished, Pressed Enter to Exit");
                    Console.ReadLine();
                    break;
            }

            return mode;
        }

        private static bool ReadUserSelection(out int mode)
        {
            Console.WriteLine("" +
                              "Do All: 0\n" +
                              "Download Group List: 1\n" +
                              "Clone Projects: 2\n" +
                              "Create Projects On Azure: 3\n" +
                              "Create Repositories On Azure: 4\n" +
                              "Upload Projects: 5\n" + 
                              "Exit: 6");
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
                    File.WriteAllText(CONFIG_PATH, config.Serialize());
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
