namespace PR.Squid.FirehoseToLoki {

    // Basic abtract class to implement firehose response
    public abstract class FirehoseResponse {

        public FirehoseResponse() { }
        public FirehoseResponse(string requestId, long timestamp) {
            RequestId = requestId;
            Timestamp = timestamp;
        }

        public string RequestId { get; set; }
        public long Timestamp { get; set; }
    }
    
    // Basic class for Success firehose response
    public class FirehoseResponseSuccess: FirehoseResponse {
        public FirehoseResponseSuccess(): base() { }
        public FirehoseResponseSuccess(string requestId, long timestamp): base(requestId, timestamp) { }
    }

    // Basic class for Error firehose response
    public class FirehoseResponseError: FirehoseResponse {

        public FirehoseResponseError(): base() { }
        public FirehoseResponseError(string requestId, long timestamp): base(requestId, timestamp) { }
        public FirehoseResponseError(string requestId, long timestamp, string errorMessage): base(requestId, timestamp) {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage {get; set; }
    }
}