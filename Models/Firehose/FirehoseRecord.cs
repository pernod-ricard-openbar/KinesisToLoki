namespace PR.Squid.FirehoseToLoki {

    // Basic class that represents a Firehost record
    //https://docs.aws.amazon.com/firehose/latest/dev/httpdeliveryrequestresponse.html#requestformat
    public class FirehoseRecord {

        public FirehoseRecord() {
        }

        public string Data { get; set; }
    }
}