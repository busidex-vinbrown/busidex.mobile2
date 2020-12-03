namespace Busidex.Models.Domain
{
    public class SearchResponse
    {
        public SearchResponse()
        {
        }

        public bool Success { get; set; }
        public SearchModel SearchModel { get; set; }
    }
}

