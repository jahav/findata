using System.Runtime.Serialization;

namespace JusticeRA
{
    /// <summary>
    /// Search criteria on the https://or.justice.cz
    /// </summary>
    [DataContract]
    public class SearchCriteria
    {
        /// <summary>
        /// Name fo the company. Searches at the beginning of the name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }


        /// <summary>
        /// IČO. If null, not used. It doesn't look at the substring, only whole equal IČO is looked up.
        /// </summary>
        [DataMember]
        public string FictIdNumber { get; set; }
    }
}
