using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace JusticeRA
{
    public class JusticeRA : IJusticeRA
    {
        private static string documentDetailUrl = @"https://or.justice.cz/ias/content/download?id={0}";
        private static string prefixUrl = @"https://or.justice.cz/ias/ui/";
        private static string documentListingUrl = @"https://or.justice.cz/ias/ui/vypis-sl-firma?subjektId={0}";

        /// <inheritdoc />
        public async Task<DocumentMetadata[]> GetDocumentListAsync(int subjectId)
        {
            var web = new HtmlWeb();
            var urls = await GetDocumentUrls(subjectId);
            foreach (var url in urls)
            {
                var document = await web.LoadFromWebAsync(url);

            }
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<string>> GetDocumentUrls(int subjectId)
        {
            var url = string.Format(documentListingUrl, subjectId);
            var document = await new HtmlWeb().LoadFromWebAsync(url);
            return document.DocumentNode
                .SelectNodes(@"//table[@class='list']//tbody/tr/td[1]/a")
                .Select(x => prefixUrl + x.Attributes["href"].Value);
        }
    }
}
