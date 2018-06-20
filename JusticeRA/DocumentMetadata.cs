using System;
using System.Runtime.Serialization;

namespace JusticeRA
{
    /// <summary>
    /// Metadata of a document.
    /// </summary>
    [DataContract(Namespace = "http://financnidata.cz/company/document/meta")]
    public class DocumentMetadata
    {
        /// <summary>
        /// Number of the document. Basically an identification at the court.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Summary of what the document contains.
        /// </summary>
        public string[] Content { get; set; }

        /// <summary>
        /// The date when the document was created.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// The date when the document was received by the court.
        /// </summary>
        public DateTime? ReceivedDate { get; set; }

        /// <summary>
        /// The date when the document was filed in the <span>sbírka listin</span>.
        /// </summary>
        public DateTime? FillingDate { get; set; }

        /// <summary>
        /// Number of pages of the document.
        /// </summary>
        public int? PageCount { get; set; }

        /// <summary>
        /// Is the document in the digital form.
        /// </summary>
// XXX:        public bool IsDigital { get; set; }

        /// <summary>
        /// The URI to download the document.
        /// </summary>
        public Uri DownloadUri { get; set; }        
    }
}
