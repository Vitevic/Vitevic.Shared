// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        /// <summary>
        /// Converts given path (d:\src\File.txt) to the real one (d:\src\File.txt).
        /// Resolves symlink!!!
        /// </summary>
        /// <remarks>
        /// https://stackoverflow.com/a/41409734
        /// </remarks>       
        public static string CaseSensitivePath(this string path)
        {
            // use 0 for access so we can avoid error on our metadata-only query (see dwDesiredAccess docs on CreateFile)
            // use FILE_FLAG_BACKUP_SEMANTICS for attributes so we can operate on directories (see Directories in remarks section for CreateFile docs)
            using (var handle = NativeMethods.CreateFile(path, 0,
                FileShare.ReadWrite | FileShare.Delete, IntPtr.Zero, FileMode.Open,
                (FileAttributes)NativeMethods.FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero))
            {
                var err = Marshal.GetLastWin32Error();
                if (handle.IsInvalid)
                    throw new System.ComponentModel.Win32Exception(err);

                return GetFinalPathNameBy(handle);
            }
        }

        static string GetFinalPathNameBy(SafeFileHandle fileHandle)
        {
            StringBuilder outPath = new StringBuilder(1024);

            var size = NativeMethods.GetFinalPathNameByHandle(fileHandle, outPath, (uint)outPath.Capacity, NativeMethods.FILE_NAME_NORMALIZED);
            var err = Marshal.GetLastWin32Error();
            if (size == 0 || size > outPath.Capacity)
                throw new System.ComponentModel.Win32Exception(err);

            // may be prefixed with \\?\, which we don't want
            if (outPath[0] == '\\' && outPath[1] == '\\' && outPath[2] == '?' && outPath[3] == '\\')
                return outPath.ToString(4, outPath.Length - 4);

            return outPath.ToString();
        }

        private static class NativeMethods
        {
            [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern uint GetFinalPathNameByHandle(SafeFileHandle hFile, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszFilePath, uint cchFilePath, uint dwFlags);
            public const uint FILE_NAME_NORMALIZED = 0x0;

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern SafeFileHandle CreateFile([MarshalAs(UnmanagedType.LPWStr)] string filename,
                                                     [MarshalAs(UnmanagedType.U4)] FileAccess access,
                                                     [MarshalAs(UnmanagedType.U4)] FileShare share,
                                                     IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
                                                     [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
                                                     [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
                                                     IntPtr templateFile);
            public const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
        }
    }
}
