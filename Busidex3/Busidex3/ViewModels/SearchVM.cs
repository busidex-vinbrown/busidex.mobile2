﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class SearchVM : BaseCardListViewModel
    {
        private readonly SearchHttpService _searchHttpService = new SearchHttpService();

        public ImageSource BackgroundImage =>
            ImageSource.FromResource("Busidex3.Resources.cards_back2.png",
                typeof(SearchVM).Assembly);

        private List<UserCard> _searchResults;
        public List<UserCard> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged(nameof(SearchResults));
            }
        } 

        private bool _isSearching;

        public bool IsSearching { 
            get => _isSearching;
            set {
                _isSearching = value;
                OnPropertyChanged(nameof(IsSearching));
            }
        }

        public void ClearSearch()
        {
            SearchResults = new List<UserCard>();
        }

        public async Task<bool> DoSearch()
        {
            IsSearching = true;

            var response = await _searchHttpService.DoSearch(SearchValue);

            var results = new List<UserCard>();
            var resultList = response.SearchModel.Results.Select(r => new UserCard(r)).ToList();
            await DownloadImages(resultList, new ProgressStatus{Count = resultList.Count});

            results.AddRange(resultList);

            //results.ForEach(uc => uc.Card.Parent = uc);
            SearchResults = new List<UserCard>(results);

            IsSearching = false;         

            return true;
        }
    }
}
