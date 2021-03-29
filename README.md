# Firehose to Loki
A simple Azure Function that receives data from AWS Kinesis Firehose and sends them to Loki.

## Config parameters in local.settings.json

- LokiEndpoint: Loki endpoint (https://xxxxxx)
- LokiUsername: Username to access loki endpoint
- LokiPassword: Password to access loki endpoint
- FirehoseAccessKey: Access key sent by Firehose in X-Amz-Firehose-Access-Key http header. If specify we will check this value with the header send by Firehose and reject (401 http error) requests that do not match this key.

