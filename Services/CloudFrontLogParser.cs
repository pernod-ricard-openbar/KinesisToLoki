using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace PR.Squid.KinesisToLoki {
    public class CloudFrontLogParser {

        private List<string> _fieldsToDrop;

        public CloudFrontLogParser(IConfiguration config) {
            // Dictionary to store content
            ContentDictionary = new Dictionary<string, string>();
            // Dropped fields
            _fieldsToDrop = new List<string>(config["KinesisFieldsToDrop"].Split(','));
        }

        // Load the data
        public void Load(string record) {
            // Clear the content
            ContentDictionary.Clear();
            ContentRaw = string.Empty;

            // Loop through the content of the record
            string[] recordFragments = record.Split('\t');
            for (int i = 0; i < recordFragments.Length; i++) {
                if (!_fieldsToDrop.Contains(Fields[i])) {
                    ContentDictionary.Add(Fields[i], recordFragments[i]);
                    ContentRaw += recordFragments[i];
                    if (i != recordFragments.Length - 1) {
                        ContentRaw += "\t";
                    }
                } 
            }
        }

        public Dictionary<string, string> ContentDictionary { get; set; }
        public string ContentRaw { get; set; }

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