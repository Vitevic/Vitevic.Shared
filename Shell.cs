using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
                    var existingExt = Path.GetExtension(name);
                    var paths = path.Split(PathSeparator, StringSplitOptions.RemoveEmptyEntries);

                    string TryPaths(string baseName, IEnumerable<string> exts)
                    {
                        foreach (var p in paths)
                        {
                            foreach (var ext in exts)
                            {
                                var possibleFullPath = Path.Combine(p, baseName + ext);
                                if (File.Exists(possibleFullPath))
                                {
                                    return possibleFullPath;
                                }
                            }
                        }

                        return null;
                    }

                    if (!String.IsNullOrEmpty(existingExt))
                    {
                        result = TryPaths(name.Substring(0, name.Length - existingExt.Length), new[] { existingExt });

                        if (result != null)
                        {
                            return true;
                        }
                    }

                    var pathExt = GetEnvVariable("PATHEXT", DefaultPathExt);
                    var pathExts = pathExt.Split(PathSeparator, StringSplitOptions.RemoveEmptyEntries);

                    result = TryPaths(name, pathExts);
                }
            }

            return result != null;
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
        /// Runs executable.
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="arguments"></param>
        /// <returns>(exitCode, output, err)</returns>
        public static (int, string, string) RunExe(string exePath, IEnumerable<string> arguments)
        {
            return RunExeIn(null, null, exePath, arguments);
        }
        public static (int, string, string) RunExe(string exePath, params string[] arguments)
        {
            return RunExe(exePath, arguments.AsEnumerable());
        }
        public static (int, string, string) RunExe(string exePath, IDictionary<string, string> env, params string[] arguments)
        {
            return RunExeIn(null, env, exePath, arguments.AsEnumerable());
        }

        public static (int, string, string) RunExeIn(string workingDirectory, IDictionary<string, string> env, string exePath, IEnumerable<string> arguments)
        {
            var process = new Process();
            process.StartInfo.FileName = exePath;

            var ab = new ArgumentBuilder { arguments };
            process.StartInfo.Arguments = ab.ToString();

            if (workingDirectory != null)
                process.StartInfo.WorkingDirectory = workingDirectory;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            if( env != null )
            {
                foreach(var item in env)
                {
                    if(process.StartInfo.Environment.ContainsKey(item.Key))
                    {
                        process.StartInfo.Environment[item.Key] = item.Value;
                    }
                    else
                    {
                        process.StartInfo.Environment.Add(item);
                    }
                }
            }

            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            var err = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return (process.ExitCode, output.TrimEnd(EolChars), err.TrimEnd(EolChars));
        }
        public static (int, string, string) RunExeIn(string workingDirectory, string exePath, params string[] arguments)
        {
            return RunExeIn(workingDirectory, null, exePath, arguments.AsEnumerable());
        }

        /// <summary>
        /// Runs shell command. Using cmd /c ....
        /// </summary>
        public static (int, string, string) RunCommand(string command, IEnumerable<string> arguments)
        {
            return RunCommandIn(null, null, command, arguments);
        }
        /// <summary>
        /// Runs shell command. Using cmd /c ....
        /// </summary>
        public static (int, string, string) RunCommand(string command, params string[] arguments)
        {
            return RunCommand(command, arguments.AsEnumerable());
        }

        public static (int, string, string) RunCommand(string command, IDictionary<string, string> env, params string[] arguments)
        {
            return RunCommandIn(null, env, command, arguments.AsEnumerable());
        }

        public static (int, string, string) RunCommandIn(string workingDirectory, IDictionary<string, string> env, string command, IEnumerable<string> arguments)
        {
            var args = new List<string> { "/c", command };
            args.AddRange(arguments);
            return RunExeIn(workingDirectory, env, "cmd", args);
        }
        public static (int, string, string) RunCommandIn(string workingDirectory, string command, params string[] arguments)
        {
            return RunCommandIn(workingDirectory, null, command, arguments.AsEnumerable());
        }
    }
}
