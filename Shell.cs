using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vitevic.Shared
{
    public static class Shell
    {
        const string DefaultPathExt = ".exe;.bat;.cmd";
        static readonly char[] PathSeparator = new[] { ';' };
        static readonly char[] EolChars = Environment.NewLine.ToCharArray();

        /// <summary>
        /// Looks for an executable in PATH. Executable might be a script or exe.
        /// </summary>
        /// <param name="name">Name of an executable, without extension. For example: "git"</param>
        /// <param name="result">Full path or null.</param>
        /// <returns>False, if not executable found.</returns>
        public static bool FindExecutable(string name, out string result)
        {
            result = null;

            if( !string.IsNullOrWhiteSpace(name) )
            {
                var path = Environment.GetEnvironmentVariable("PATH");
                if( path != null )
                {
                    var pathExt = GetEnvVariable("PATHEXT", DefaultPathExt);
                    var exts = pathExt.Split(PathSeparator, StringSplitOptions.RemoveEmptyEntries);

                    foreach( var p in path.Split(PathSeparator, StringSplitOptions.RemoveEmptyEntries) )
                    {
                        foreach(var ext in exts)
                        {
                            var possibleFullPath = Path.Combine(p, name + ext);
                            if( File.Exists(possibleFullPath) )
                            {
                                result = possibleFullPath;
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static string GetEnvVariable(string name, string defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if( value == null )
            {
                value = defaultValue;
            }
            return value;
        }

        public static string GetUserHomeDir()
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            if( home != null )
            {
                home = Environment.ExpandEnvironmentVariables(home);
            }
            else
            {
                var homeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
                var homePath = Environment.GetEnvironmentVariable("HOMEPATH");
                if( homeDrive != null && homePath != null )
                {
                    home = homeDrive + homePath;
                }
            }

            if( home != null && Directory.Exists(home) )
            {
                return home;
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        /// <summary>
        /// Calcs file names in user home directory.
        /// </summary>
        /// <param name="path">Should be relative</param>
        /// <returns></returns>
        public static string HomeRelative(params string[] path)
        {
            return Path.Combine(GetUserHomeDir(), Path.Combine(path));
        }

        /// <summary>
        /// Runs shell command. Use this method only for very simple commands!!!
        /// </summary>
        /// <param name="command"></param>
        /// <param name="arguments"></param>
        /// <returns>(exitCode, output, err)</returns>
        public static (int, string, string) RunCommand(string command, params string[] arguments)
        {
            return RunCommandIn(null, command, arguments);
        }

        public static (int, string, string) RunCommandIn(string workingDirectory, string command, params string[] arguments)
        {
            var process = new Process();
            process.StartInfo.FileName = command;

            var ab = new ArgumentBuilder { arguments };
            process.StartInfo.Arguments = ab.ToString();

            if( workingDirectory != null )
                process.StartInfo.WorkingDirectory = workingDirectory;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            var err = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return (process.ExitCode, output.TrimEnd(EolChars), err.TrimEnd(EolChars));
        }
    }
}
