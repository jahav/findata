using System.Runtime.Serialization;

namespace JusticeRA
{
    /// <summary>
    /// A summary of a subject in the https://or.justice.cz.
    /// </summary>
    [DataContract]
    public class SubjectSummary
    {
        /// <summary>
        /// Id of a subject in the https://or.justice.cz system.
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public int SubjectId { get; set; }

        /// <summary>
        /// Name of a company.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        /// <summary>
        /// IČO.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string FictIdNumber { get; set; }
    }
}
