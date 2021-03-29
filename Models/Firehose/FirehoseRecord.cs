namespace PR.Squid.KinesisToLoki {

    // Basic class that represents a Firehost record
    //https://docs.aws.amazon.com/kinesis/latest/dev/httpdeliveryrequestresponse.html#requestformat
    public class KinesisRecord {

        public KinesisRecord() {
        }

        public string Data { get; set; }
    }
}