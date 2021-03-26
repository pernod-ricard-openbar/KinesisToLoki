using System.Collections.Generic;

namespace PR.Squid.FirehoseToLoki {

    // Basic structure to handle incomming Firehose data
    // https://docs.aws.amazon.com/firehose/latest/dev/httpdeliveryrequestresponse.html#requestformat
    public class FirehoseRequest {

        // Constructor
        public FirehoseRequest() {
            Records = new List<FirehoseRecord>();
        }

        public string RequestId { get; set; }
        public long Timestamp { get; set; }
        public List<FirehoseRecord> Records { get; set; }

    }
}