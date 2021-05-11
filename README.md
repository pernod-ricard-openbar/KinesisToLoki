# Kinesis to Loki
A simple Azure Function that receives data from AWS Kinesis Firehose and sends them to Loki.

## Config parameters in local.settings.json

- LokiEndpoint: Loki endpoint (https://xxxxxx)
- LokiUsername: Username to access loki endpoint
- LokiPassword: Password to access loki endpoint
- LokiFormat: The format the will be used to send data to Loki. Could be either json (recommanded) or raw
- KinesisAccessKey: Access key sent by Kinesis in X-Amz-Firehose-Access-Key http header. If specify we will check this value with the header send by Kinesis and reject (401 http error) requests that do not match this key.
- KinesisFieldsReceived: List of fields received (or sent by Kinesis if you prefer). Note that the order if the fields is important. This list is used for parsing, so it is important and must match the list configured in Kinesis.
- KinesisFieldsToDrop: List of fields to drop (won't be sent to Loki). Comma separated list of fields.
- CustomLabels (optional): Custom labels that will be added to all the logs sent to Loki. Format = key1,value1;key2,value2;...