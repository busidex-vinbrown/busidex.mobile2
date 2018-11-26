using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Busidex3.ViewModels
{
    public class SearchVM : BaseCardListViewModel
    {
        private SearchHttpService _searchHttpService = new SearchHttpService();

        public ImageSource BackgroundImage  
        {
            get => _backgroundImage;
            set
            {
                _backgroundImage = value;
                OnPropertyChanged(nameof(BackgroundImage));
            }
        }

        private ObservableRangeCollection<UserCard> _searchResults;
        public ObservableRangeCollection<UserCard> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged(nameof(SearchResults));
            }
        } 

        private bool _isSearching;
        private ImageSource _backgroundImage;

        public bool IsSearching { 
            get => _isSearching;
            set {
                _isSearching = value;
                OnPropertyChanged(nameof(IsSearching));
            }
        }

        public void ClearSearch()
        {
            SearchResults = new ObservableRangeCollection<UserCard>();
        }

        public async Task<bool> DoSearch()
        {
            IsSearching = true;

            var response = await _searchHttpService.DoSearch(SearchValue);

            SearchResults = new ObservableRangeCollection<UserCard>();
            var resultList = response.SearchModel.Results.Select(r => new UserCard(r)).ToList();
            await DownloadImages(resultList, new ProgressStatus{Count = resultList.Count});

            SearchResults.AddRange(resultList);

            SearchResults.ForEach(uc => uc.Card.Parent = uc);

            IsSearching = false;

            BackgroundImage = ImageSource.FromResource("Busidex3.Resources.card_back2.png");

            return true;
        }
    }
}
