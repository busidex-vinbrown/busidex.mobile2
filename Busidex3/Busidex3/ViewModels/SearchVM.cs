using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;

namespace Busidex3.ViewModels
{
    public class SearchVM : BaseViewModel
    {
        private SearchHttpService _searchHttpService = new SearchHttpService();

        public List<UserCard> SearchResults { get; private set; }

        public async Task<bool> DoSearch(string criteria)
        {
            var response = await _searchHttpService.DoSearch(criteria);

            SearchResults = response.Results.Select(r => new UserCard(r)).ToList();

            return true;
        }
    }
}
