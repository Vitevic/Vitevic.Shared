using System;
using System.IO;
using System.Text;

namespace Vitevic.Shared.Extensions
{
    public static class StringPathExtensions
    {
        /// <summary>
        /// Works like Microsoft.VisualStudio.PlatformUI.PathUtil.Normalize.
        /// - Strips leading and trailing whitespaces
        /// - Replaces "/" with "\"
        /// - Removes the trailing "\", unless it's part of a root (e.g. "C:\")
        /// - Converts to lowercase
        /// </summary>
        /// <param name="path">Path to normalize</param>
        /// <returns>Normalized from path (lowercase)</returns>
        /// <remarks>
        /// See details here: https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.platformui.pathutil.normalize?view=visualstudiosdk-2017#Microsoft_VisualStudio_PlatformUI_PathUtil_Normalize_System_String
        /// </remarks>
        public static string NormalizePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            if( path.IndexOfAny(Path.GetInvalidPathChars()) != -1 )
            {
                throw new ArgumentException("The path contains an invalid character.", nameof(path));
            }

            int start = 0;
            while( start < path.Length && char.IsWhiteSpace(path[start]))
            {
                start += 1;
            }

            if( start == path.Length)
            {
                // no valid data
                return string.Empty;
            }

            int end = path.Length - 1;
            while (end >= start && char.IsWhiteSpace(path[end]))
            {
                --end;
            }

            int length = end - start + 1;
            StringBuilder resource = new StringBuilder(); // TODO: use cached one
            bool prevWasDirectorySeparator = false;
            bool pathChanged = false;
            for (int i = start; i <= end; ++i)
            {
                char ch = path[i];
                if (ch == Path.AltDirectorySeparatorChar)
                {
                    ch = Path.DirectorySeparatorChar;
                    pathChanged = true;
                }
                if (ch == Path.DirectorySeparatorChar)
                {
                    if (prevWasDirectorySeparator && i > start + 1)
                    {
                        pathChanged = true;
                        continue;
                    }
                }
                else if (char.IsUpper(ch))
                {
                    ch = char.ToLower(ch);
                    pathChanged = true;
                }
                prevWasDirectorySeparator = ch == Path.DirectorySeparatorChar;
                resource.Append(ch);
            }

            if (prevWasDirectorySeparator && resource.Length > 3)
            {
                resource.Remove(resource.Length - 1, 1);
                pathChanged = true;
            }

            if (!pathChanged && length == path.Length)
            {
                // No changes, return as is
                return path;
            }

            return resource.ToString();
        }

        public static bool PathEquals(this string path, string another)
        {
            return path.NormalizePath() == another.NormalizePath();
        }

        public static bool PathStartsWith(this string path, string another)
        {
            return path.NormalizePath().StartsWith(another.NormalizePath());
        }

        public static string SafePath(this string path)
        {
            if( path.Contains(" ") && !path.StartsWith("\"") )
            {
                return '"' + path + '"';
            }

            return path;
        }
    }
}
