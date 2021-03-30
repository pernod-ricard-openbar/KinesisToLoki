using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace PR.Squid.KinesisToLoki {
    public class CloudFrontLogParser {

        private string[] _fieldsReceived;
        private List<string> _fieldsToDrop;
        private string[] _fieldsToLabel;
        private Dictionary<string, string> _customLabels { get; set; }

        public Dictionary<string, string> ContentDictionary { get; set; }
        public string ContentRaw { get; set; }
        public Dictionary<string, string> Labels { get; set; }

        public CloudFrontLogParser(IConfiguration config) {
            // Dictionary to store content
            ContentDictionary = new Dictionary<string, string>();
            // Fields that are received (sent by Kinesis)
            _fieldsReceived = config["KinesisFieldsReceived"].Split(',');
            // Fields received that will be dropped (not sent to Loki)
            _fieldsToDrop = new List<string>(config["KinesisFieldsToDrop"].Split(','));
            // Fields to add as label
            _fieldsToLabel = config["KinesisFieldsToLabel"].Split(',');
            // Labels
            Labels = new Dictionary<string, string>();
            // Custom label that can be added optionally 
            _customLabels = new Dictionary<string, string>();
            string[] customLabels = config["CustomLabels"].Split(';');
            foreach(string customLabel in customLabels) {
                string[] customLabelParsed = customLabel.Split(',');
                if (customLabelParsed.Length == 2) {
                    _customLabels.Add(customLabelParsed[0], customLabelParsed[1]);
                }
            }
        }

        // Load the data
        public void Load(string record) {
            // Clear the content
            ContentDictionary.Clear();
            ContentRaw = string.Empty;
            Labels.Clear();

            // Loop through the content of the record to generate the content that will be sent to Loki
            string[] recordFragments = record.Split('\t');
            for (int i = 0; i < recordFragments.Length; i++) {
                // We only add the fields that we do not want to drop
                if (!_fieldsToDrop.Contains(_fieldsReceived[i])) {
                    ContentDictionary.Add(_fieldsReceived[i], recordFragments[i]);
                    ContentRaw += recordFragments[i] + "\t";
                }
            }
            ContentRaw = ContentRaw.TrimEnd('\t'); // Trim last tab

            // Generates the labels
            GenerateLabels();
        }

        // Generates loki labels based on the list of fields that should be added as label (variable KinesisFieldsToLabel)
        private void GenerateLabels() {
            // Add fields as labels
            foreach (string labelName in _fieldsToLabel) {
                string labelValue = ContentDictionary.GetValueOrDefault<string, string>(labelName, null);
                if (labelValue != null) {
                    // Loki does not like dash "-" in label names
                    Labels.Add(labelName.Replace('-', '_'), labelValue);
                }
            }
            // Add the custom labels
            foreach (var customLabel in _customLabels) {
                Labels.Add(customLabel.Key, customLabel.Value);
            }
        }
    }
}