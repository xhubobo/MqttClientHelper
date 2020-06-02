using System;

namespace SendMessageSample
{
    internal sealed class PingInfo
    {
        public string Key { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime RecvTime { get; set; }
        public TimeSpan Span => RecvTime - SendTime;
        public bool Received { get; set; }
    }
}
