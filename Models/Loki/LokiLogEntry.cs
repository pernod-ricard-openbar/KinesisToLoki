using System.Collections.Generic;

namespace PR.Squid.KinesisToLoki {

    public class LokiLogEntry {

        public LokiLogEntry(Dictionary<string, string> labels, string epochNanoSeconds, string log) {
            LokiStream lokiStream = new LokiStream(labels, epochNanoSeconds, log);
            Streams = new List<LokiStream>();
            Streams.Add(lokiStream);
        }

        public List<LokiStream> Streams { get; set; }
    }
}