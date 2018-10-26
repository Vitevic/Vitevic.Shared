using System;

namespace Vitevic.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ParseUnixTime(this string unixTimeSeconds)
        {
            return long.Parse(unixTimeSeconds).FromUnixTime();
        }

        public static DateTime ParseUnixTimeUtc(this string unixTimeSeconds)
        {
            return long.Parse(unixTimeSeconds).FromUnixTimeUtc();
        }

        public static DateTime FromUnixTime(this long unixTimeSeconds)
        {
            var offset = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds);
            return offset.LocalDateTime;
        }

        public static DateTime FromUnixTimeUtc(this long unixTimeSeconds)
        {
            var offset = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds);
            return offset.UtcDateTime;
        }
    }
}
