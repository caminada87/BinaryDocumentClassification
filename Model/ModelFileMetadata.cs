using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinaryDocumentClassification.Classes;

namespace BinaryDocumentClassification.Model
{
    class ModelFileMetadata
    {
        public SortedList<string, FileMetadata> MetadataList = new SortedList<string, FileMetadata>();

        public bool SaveChanges() {
            string filesPath = ConfigurationManager.AppSettings["path"];
            string metadataPath = Path.Combine(filesPath, "metadata.csv");
            using (FileStream metadataFile = File.Create(metadataPath)) {
                using (StreamWriter sw = new StreamWriter(metadataFile)) {
                    foreach (KeyValuePair<string, FileMetadata> kvp in MetadataList) {
                        string line = string.Format("{0}; {1}", kvp.Key, kvp.Value.ToString());
                        sw.WriteLine(line);
                    }
                }
            }
            return true;
        }
    }
}
