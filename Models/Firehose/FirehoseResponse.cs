namespace PR.Squid.KinesisToLoki {

    // Basic abtract class to implement kinesis response
    public abstract class KinesisResponse {

        public KinesisResponse() { }
        public KinesisResponse(string requestId, long timestamp) {
            RequestId = requestId;
            Timestamp = timestamp;
        }

        public string RequestId { get; set; }
        public long Timestamp { get; set; }
    }
    
    // Basic class for Success kinesis response
    public class KinesisResponseSuccess: KinesisResponse {
        public KinesisResponseSuccess(): base() { }
        public KinesisResponseSuccess(string requestId, long timestamp): base(requestId, timestamp) { }
    }

    // Basic class for Error kinesis response
    public class KinesisResponseError: KinesisResponse {

        public KinesisResponseError(): base() { }
        public KinesisResponseError(string requestId, long timestamp): base(requestId, timestamp) { }
        public KinesisResponseError(string requestId, long timestamp, string errorMessage): base(requestId, timestamp) {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage {get; set; }
    }
}