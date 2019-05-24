using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Vitevic.Shared
{
    public static class Argument
    {
        [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(T arg, string argName) where T : class
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }
    }
}
