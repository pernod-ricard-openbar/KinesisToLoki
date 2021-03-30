using System.Collections.Generic;

namespace PR.Squid.KinesisToLoki {
    public class CloudFrontLogRecord {
        public CloudFrontLogRecord(string[] recordFragments) {
            Content = new Dictionary<string, string>();
            int i = 0;
            foreach(string recordFragment in recordFragments) {
                Content.Add(Fields[i], recordFragment);
                i++;
            }
        }

        public Dictionary<string, string> Content { get; set; }

        // Fields, ordered, as they are received from Kinesis
        private static string[] Fields = {
            "timestamp",
            "c-ip",
            "time-to-first-byte",
            "sc-status",
            "sc-bytes",
            "cs-method",
            "cs-protocol",
            "cs-host",
            "cs-uri-stem",
            "cs-bytes",
            "x-edge-location",
            "x-edge-request-id",
            "x-host-header",
            "time-taken",
            "cs-protocol-version",
            "c-ip-version",
            "cs-user-agent",
            "cs-referer",
            "cs-cookie",
            "cs-uri-query",
            "x-edge-response-result-type",
            "x-forwarded-for",
            "ssl-protocol",
            "ssl-cipher",
            "x-edge-result-type",
            "fle-encrypted-fields",
            "fle-status",
            "sc-content-type",
            "sc-content-len",
            "sc-range-start",
            "sc-range-end",
            "c-port",
            "x-edge-detailed-result-type",
            "c-country",
            "cs-accept-encoding",
            "cs-accept",
            "cache-behavior-path-pattern",
            "cs-headers",
            "cs-header-names",
            "cs-headers-count",
        };
    }
}