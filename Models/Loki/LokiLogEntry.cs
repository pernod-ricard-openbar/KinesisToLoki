using System;
using System.Collections.Generic;

namespace PR.Squid.KinesisToLoki {

    public class LokiLogEntry {

        public LokiLogEntry(Dictionary<string, string> labels, string epochNanoSeconds, string log) {
            LokiStream lokiStream = new LokiStream(labels, epochNanoSeconds, log);
            Streams = new List<LokiStream>();
            Streams.Add(lokiStream);
        }

        public LokiLogEntry(Dictionary<string, string> labels, string log) {
            string epochNanoSeconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() + "000000";
            LokiStream lokiStream = new LokiStream(labels, epochNanoSeconds, log);
            Streams = new List<LokiStream>();
            Streams.Add(lokiStream);
        }

        public LokiLogEntry(Dictionary<string, string> labels, CloudFrontLogParser cloudFrontLogParser) {
            string epochNanoSeconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() + "000000";
            LokiStream lokiStream = new LokiStream(labels, epochNanoSeconds, cloudFrontLogParser.ContentRaw);
            Streams = new List<LokiStream>();
            Streams.Add(lokiStream);
        }

        public List<LokiStream> Streams { get; set; }
    }
}