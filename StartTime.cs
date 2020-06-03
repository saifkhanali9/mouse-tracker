using System;
namespace WindowsFormsApp1
{
    internal class StartTime
    {
        public long getStartTime()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMs = now.ToUnixTimeMilliseconds();
            return unixTimeMs;
        }
    }
}