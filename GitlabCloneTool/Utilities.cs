using System;
using System.IO;
using System.Linq;

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
    }
}