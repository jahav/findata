using System.Collections.Generic;
using System.Linq;
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
        /// Search a subject at https://or.justice.cz.
        /// </summary>
        /// <param name="criteria">Criteria of the search.</param>
        /// <returns>Array of found results.</returns>
        [OperationContract]
        Task<SubjectSummary[]> SearchSubjects(SearchCriteria criteria);

        /// <summary>
        /// Get all documents of the subject.
        /// </summary>
        /// <param name="subjectId">ID of subject at the https://or.justice.cz web.</param>
        [OperationContract]
        Task<DocumentMetadata[]> GetDocumentListAsync(int subjectId);
    }
}
