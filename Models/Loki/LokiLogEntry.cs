using System;
using System.Collections.Generic;
using System.Text.Json;

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

        public LokiLogEntry(Dictionary<string, string> labels, CloudFrontLogParser cloudFrontLogParser, string format) {
            string epochNanoSeconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() + "000000";
            LokiStream lokiStream;
            if (format == "raw") {
                lokiStream = new LokiStream(labels, epochNanoSeconds, cloudFrontLogParser.ContentRaw);
            }
            else { // json
                string json = JsonSerializer.Serialize<Dictionary<string,string>>(cloudFrontLogParser.ContentDictionary);
                lokiStream = new LokiStream(labels, epochNanoSeconds, json);
            }
            Streams = new List<LokiStream>();
            Streams.Add(lokiStream);
        }

        public List<LokiStream> Streams { get; set; }
    }
}