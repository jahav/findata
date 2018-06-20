using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace JusticeRA
{
    /// <summary>
    /// The resource access to the https://or.justice.cz portal. That portal doesn't support 
    /// API (=need to crawl and parse) and has significant limitations concerning the number 
    /// of allowed requests per minute (50) and day (3000), see details at <a href="https://or.justice.cz/ias/ui/podminky">ToS</a>.
    /// </summary>
    [ServiceContract]
    public interface IJusticeRA
    {
        /// <summary>
        /// Get all documents of the subject.
        /// </summary>
        /// <param name="subjectId">ID of subject at the https://or.justice.cz web.</param>
        [OperationContract]
        Task<DocumentMetadata[]> GetDocumentListAsync(int subjectId);
    }

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
        /// Summary of the document.
        /// </summary>
        public string Summary { get; set; }

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
        public DateTime? FileDate { get; set; }

        /// <summary>
        /// Number of pages of the document.
        /// </summary>
        public int? PageCount { get; set; }

        /// <summary>
        /// Is the document in the digital form.
        /// </summary>
        public bool IsDigital { get; set; }

        /// <summary>
        /// The URI to download the document.
        /// </summary>
        public Uri DownloadUri { get; set; }        
    }
}
