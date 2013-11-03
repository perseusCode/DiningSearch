using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebRole1
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Search.Command += new CommandEventHandler(this.SearchStart);
        }

        protected void SearchStart(object sender, CommandEventArgs e)
        {
            string searchText = TextBox1.Text;
            resultPanel.Text = this.GenerateResultHtml(GetResults(searchText));
        }

        private string GenerateResultHtml(List<SearchResult> searchResults)
        {
            string html = "<table><th>Cafe/Restaurant</th><th>Dish Name</th><th>Description</th><th>Price</th><th>Cafe Menu Url</th>";
            foreach (SearchResult result in searchResults)
            {
                html += "<tr>";
                html += string.Format("<td>{0}</td>", result.CafeName);
                html += string.Format("<td>{0}</td>", result.DishName);
                html += string.Format("<td>{0}</td>", result.Description);
                html += string.Format("<td>{0}</td>", result.Price);
                html += string.Format("<td><a href=\"{0}\">Cafe Menu</a></td>", result.CafeUrl);
                html += "</tr>";
            }
            html += "</table>";

            return html;
        }

        private List<SearchResult> GetResults(string searchText)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=diningsearchstorage;AccountKey=xeYMzXThFxrU7SsAMGbSWLdy9psLFRMk5NI8x0bx24xtEg9MPIstf/xwPdjvDm6HpHZaCPxxVFCv/7DDd5wymA==");
            AzureDirectory azureDirectory = new AzureDirectory(storageAccount, "diningsearchindex");
            IndexReader indexReader = IndexReader.Open(azureDirectory, true);
            Searcher indexSearch = new IndexSearcher(indexReader);
            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Dish", analyzer);
            var query = queryParser.Parse(searchText);

            Console.WriteLine("Searching for: " + query.ToString());
            TopDocs resultDocs = indexSearch.Search(query, 100);

            Console.WriteLine("Results Found: " + resultDocs.TotalHits);

            List<SearchResult> searchResults = new List<SearchResult>();
            var hits = resultDocs.ScoreDocs;
            foreach (var hit in hits)
            {
                Document documentFromSearcher = indexSearch.Doc(hit.Doc);

                SearchResult result = new SearchResult
                {
                    CafeName = documentFromSearcher.Get("Cafe/Restaurant"),
                    CafeUrl = documentFromSearcher.Get("URL"),
                    DishName = documentFromSearcher.Get("Dish"),
                    Price = documentFromSearcher.Get("Price"),
                    Description = documentFromSearcher.Get("Description")
                };

                searchResults.Add(result);
            }

            return searchResults;
        }

    }
}