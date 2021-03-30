# Kinesis to Loki
A simple Azure Function that receives data from AWS Kinesis Kinesis and sends them to Loki.

## Config parameters in local.settings.json

- LokiEndpoint: Loki endpoint (https://xxxxxx)
- LokiUsername: Username to access loki endpoint
- LokiPassword: Password to access loki endpoint
- KinesisAccessKey: Access key sent by Kinesis in X-Amz-Firehose-Access-Key http header. If specify we will check this value with the header send by Kinesis and reject (401 http error) requests that do not match this key.
- KinesisFieldsToDrop: List of fields to drop (won't be sent to Loki). Comma separated list of fields.

