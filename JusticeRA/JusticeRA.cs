using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static string baseUrl = @"https://or.justice.cz";
        private static string prefixUrl = @"https://or.justice.cz/ias/ui/";
        private static string documentListingUrl = @"https://or.justice.cz/ias/ui/vypis-sl-firma?subjektId={0}";
        private static CultureInfo czechCulture = CultureInfo.GetCultureInfo("cs-CZ");

        /// <inheritdoc />
        public async Task<DocumentMetadata[]> GetDocumentListAsync(int subjectId)
        {
            var web = new HtmlWeb();
            var urls = await GetDocumentUrls(subjectId);
            var documents = new List<DocumentMetadata>();
            foreach (var url in urls)
            {
                var htmlDocument = await web.LoadFromWebAsync(url);
                var document = ExtractMetadata(htmlDocument);
                documents.Add(document);
            }
            return documents.ToArray();
        }

        private async Task<IEnumerable<string>> GetDocumentUrls(int subjectId)
        {
            var url = string.Format(documentListingUrl, subjectId);
            var document = await new HtmlWeb().LoadFromWebAsync(url);
            return document.DocumentNode
                .SelectNodes(@"//table[@class='list']//tbody/tr/td[1]/a")
                .Select(x => HtmlEntity.DeEntitize(x.Attributes["href"].Value))
                .Select(x => x.StartsWith("./") ? x.Substring(2) : x)
                .Select(x => prefixUrl + x);
        }

        private DocumentMetadata ExtractMetadata(HtmlDocument document)
        {
            var tableNode = document.DocumentNode.SelectSingleNode("//tbody[1]");
            if (tableNode == null)
            {
                throw new FormatException("No document element at the page.");
            }

            var number = GetText(tableNode, "./tr[@class='subrow'][5]/td");
            var content = tableNode.SelectNodes("./tr[@class='subrow'][6]//span[@class='symbol']").Select(y => y.InnerText).ToArray();
            var createdDate = GetDate(tableNode, "./tr[@class='subrow'][7]/td");
            var receivedDate = GetDate(tableNode, "./tr[@class='subrow'][9]/td");
            var fillingDate = GetDate(tableNode, "./tr[@class='subrow'][10]/td");
            var downloadUri = GetUrl(tableNode, "./tr[@class='subrow'][11]/td/a");
            var pageCount = int.Parse(GetText(tableNode, "./tr[@class='subrow'][11]/td/a/span[2]"));
            return new DocumentMetadata
            {
                Number = number,
                Content = content,
                CreatedDate = createdDate,
                ReceivedDate = receivedDate,
                FillingDate = fillingDate,
                PageCount = pageCount,
                DownloadUri = downloadUri
            };
        }

        private string GetText(HtmlNode node, string xpath)
        {
            var textNode = node.SelectSingleNode(xpath);
            if (textNode == null)
            {
                throw new FormatException($"Unable to find text node {xpath}.");
            }
            var text = HtmlEntity.DeEntitize(textNode.InnerText ?? string.Empty).Trim();
            if (text == string.Empty)
            {
                return null;
            }

            return text;
        }

        private DateTime? GetDate(HtmlNode node, string xpath)
        {
            var dateNode = node.SelectSingleNode(xpath);
            if (dateNode == null)
            {
                throw new FormatException($"Unable to find date node {xpath}.");
            }
            var text = HtmlEntity.DeEntitize(dateNode.InnerText ?? string.Empty).Trim();
            if (text == string.Empty)
            {
                return null;
            }

            return DateTime.Parse(text, czechCulture);
        }

        private Uri GetUrl(HtmlNode node, string xpath)
        {
            var hrefNode = node.SelectSingleNode(xpath);
            if (hrefNode == null)
            {
                throw new FormatException($"Unable to find href node {xpath}.");
            }
            var path = HtmlEntity.DeEntitize(hrefNode.Attributes["href"]?.Value ?? string.Empty).Trim();
            return new Uri(baseUrl + path);
        }
    }
}
