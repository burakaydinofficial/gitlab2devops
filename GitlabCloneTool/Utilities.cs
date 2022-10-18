using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GitlabCloneTool
{
    public class Utilities
    {
        public static bool WriteAllText(string directory, string file, string contents)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    File.WriteAllText(Path.Combine(directory, file), contents);
                }
                else
                {
                    File.WriteAllText(file, contents);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static bool WriteAllText(string path, string contents)
        {
            if (path.Contains('/') || path.Contains('\\'))
                return WriteAllText(Path.GetDirectoryName(path), Path.GetFileName(path));
            else
                return WriteAllText(null, path, contents);
        }


        public class Git
        {
            public static async Task<bool?> CloneProject(string gitUrl, string projectDir)
            {
                if (!Directory.Exists(Path.Combine(projectDir, ".git")))
                {
                    ProcessStartInfo gitStartInfo = new ProcessStartInfo()
                    {
                        FileName = "git.exe",
                        Arguments = "clone " + gitUrl + " \"" + projectDir + "\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = false,
                    };

                    var process = new Process()
                    {
                        StartInfo = gitStartInfo
                    };

                    process.Start();

                    while (!process.HasExited)
                    {
                        Thread.Sleep(500);
                        while (!process.StandardOutput.EndOfStream)
                        {
                            var text = await process.StandardOutput.ReadLineAsync();
                            //Console.WriteLine(text);
                        }
                    }

                    return process.ExitCode == 0;
                }

                return null;
            }
            public static async Task<bool> FetchProject(string projectDir)
            {
                if (Directory.Exists(Path.Combine(projectDir, ".git")))
                {
                    ProcessStartInfo gitStartInfo = new ProcessStartInfo()
                    {
                        FileName = "git.exe",
                        Arguments = "-C " + "\"" + projectDir + "\"" + " fetch --all",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = false,
                    };

                    var process = new Process()
                    {
                        StartInfo = gitStartInfo
                    };

                    process.Start();

                    while (!process.HasExited)
                    {
                        Thread.Sleep(500);
                        while (!process.StandardOutput.EndOfStream)
                        {
                            var text = await process.StandardOutput.ReadLineAsync();
                            Console.WriteLine(text);
                        }
                    }

                    return process.ExitCode == 0;
                }

                return false;
            }
            public static async Task<bool> FetchProjectLFS(string projectDir)
            {
                if (Directory.Exists(Path.Combine(projectDir, ".git")))
                {
                    ProcessStartInfo gitStartInfo = new ProcessStartInfo()
                    {
                        FileName = "git.exe",
                        Arguments = "-C " + "\"" + projectDir + "\"" + " lfs fetch --all",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = false,
                    };

                    var process = new Process()
                    {
                        StartInfo = gitStartInfo
                    };

                    process.Start();

                    while (!process.HasExited)
                    {
                        Thread.Sleep(500);
                        while (!process.StandardOutput.EndOfStream)
                        {
                            var text = await process.StandardOutput.ReadLineAsync();
                            Console.WriteLine(text);
                        }
                    }

                    return process.ExitCode == 0;
                }

                return false;
            }
        }
    }
}