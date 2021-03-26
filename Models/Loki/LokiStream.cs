using System.Collections.Generic;

namespace PR.Squid.FirehoseToLoki {

    public class LokiStream {

        public LokiStream(Dictionary<string, string> labels, string epochNanoSeconds, string log) {
            Stream = labels;
            string[] logLine = {epochNanoSeconds, log};
            Values = new List<string[]>();
            Values.Add(logLine);
        }

        public Dictionary<string, string> Stream { get; set; }
        public List<string[]> Values { get; set; }
    }
}