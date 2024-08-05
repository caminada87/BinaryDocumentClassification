using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryDocumentClassification.Classes
{
    public class FileMetadata
    {
        private bool _processed;
        private string _className;

        public bool Processed {
            get {
                return _processed;
            }
            set {
                _processed = value;
            }
        }

        public string ClassName {
            get {
                return _className;
            }
            set {
                _className = value;
            }
        }

        public override string ToString() {
            string cls = (ClassName == "" || ClassName == null) ? "N/A" : ClassName;
            return string.Format("{0}; {1}", Processed, cls);
        }
    }
}
