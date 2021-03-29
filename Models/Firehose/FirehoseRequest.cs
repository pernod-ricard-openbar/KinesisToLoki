using System.Collections.Generic;

namespace PR.Squid.KinesisToLoki {

    // Basic structure to handle incomming Kinesis data
    // https://docs.aws.amazon.com/kinesis/latest/dev/httpdeliveryrequestresponse.html#requestformat
    public class KinesisRequest {

        // Constructor
        public KinesisRequest() {
            Records = new List<KinesisRecord>();
        }

        public string RequestId { get; set; }
        public long Timestamp { get; set; }
        public List<KinesisRecord> Records { get; set; }

    }
}