using System;

namespace Vitevic.Shared.Extensions
{
    static class DateTimeStringExtensions
    {
        /// <summary>
        /// Midnight 1 January 1970.
        /// </summary>
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ParseUnixTime(this string unixTime)
        {
            return UnixEpoch.AddSeconds(long.Parse(unixTime)).ToLocalTime();
        }
    }
}
