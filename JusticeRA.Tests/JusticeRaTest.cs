using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace JusticeRA.Tests
{
    public class JusticeRaTest
    {
        private int profinitSubjectId = 910176;

        [Fact(Skip = "Downloads real data, I don't want to get banned.")]
        public async Task DownloadDocumentList()
        {
            var justiceRa = new JusticeRA();
            var list = await justiceRa.GetDocumentListAsync(profinitSubjectId);
        }

        [Fact]
        public async Task SearchWorks()
        {
            var justiceRa = new JusticeRA();
            var list = await justiceRa.SearchSubjects(new SearchCriteria {
                Name = "Pro"
            });
        }
    }
}
