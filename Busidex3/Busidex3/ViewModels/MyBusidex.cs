using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;

namespace Busidex3.ViewModels
{
    public delegate void OnMyBusidexLoadedEventHandler (List<UserCard> cards);
    public delegate void OnMyBusidexUpdatedEventHandler (ProgressStatus status);

    public class MyBusidex : BaseViewModel
    {
        public static event OnMyBusidexLoadedEventHandler OnMyBusidexLoaded;
        public static event OnMyBusidexUpdatedEventHandler OnMyBusidexUpdated;

        private static MyBusidexHttpService _myBusidexHttpService;

        private readonly string _authToken;

        public MyBusidex(string token)
        {
            _authToken = token;
            _myBusidexHttpService = new MyBusidexHttpService();
        }

        private static List<UserCard> _userCards;
        public static List<UserCard> UserCards => _userCards;
        public static Card OwnedCard => null;

        async Task<bool> loadUserCards()
        {
            var semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();

            var cards = new List<UserCard>();
            var status = new ProgressStatus();

            try
            {
                var result = await _myBusidexHttpService.GetMyBusidex(_authToken);

                if (result == null)
                {
                    var fullFileName = Path.Combine(Resources.DocumentsPath, Resources.MY_BUSIDEX_FILE);
                    cards.AddRange(GetCachedResult<List<UserCard>>(fullFileName));
                }
                else
                {
                    cards.AddRange(result.MyBusidex.Busidex);
                    cards.ForEach(c => c.ExistsInMyBusidex = true);
                }

                status.Total = cards.Count;

                foreach (var item in cards)
                {
                    if (item.Card != null)
                    {

                        var fImageUrl = Resources.THUMBNAIL_PATH + item.Card.FrontFileName;
                        var bImageUrl = Resources.THUMBNAIL_PATH + item.Card.BackFileName;
                        var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.FrontFileName;
                        var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + item.Card.BackFileName;

                        if (!File.Exists(Resources.DocumentsPath + "/" + fName))
                        {
                            try
                            {
                                await DownloadImage(fImageUrl, Resources.DocumentsPath, fName).ContinueWith(t =>
                                {
                                    status.Count++;
                                    OnMyBusidexUpdated?.Invoke(status);
                                });
                            }
                            catch (Exception)
                            {

                            }
                        }
                        else
                        {
                            status.Count++;
                            OnMyBusidexUpdated?.Invoke(status);
                        }

                        if ((!File.Exists(Resources.DocumentsPath + "/" + bName)) &&
                            item.Card.BackFileId.ToString() != Resources.EMPTY_CARD_ID)
                        {
                            try
                            {
                                await DownloadImage(bImageUrl, Resources.DocumentsPath, bName);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }

                UserCards.Clear();

                // If the user has a card, make sure it's always at the top of the list
                if (OwnedCard != null)
                {
                    var ownersCard = cards.FirstOrDefault(uc => uc.Card.CardId == OwnedCard.CardId &&
                                                                (!string.IsNullOrEmpty(OwnedCard.Name) ||
                                                                 !string.IsNullOrEmpty(OwnedCard.CompanyName) ||
                                                                 OwnedCard.FrontFileId.HasValue));
                    if (ownersCard != null)
                    {
                        UserCards.Add(ownersCard);
                    }

                    UserCards.AddRange(cards.Distinct(new UserCardEqualityComparer())
                        .Where(c => c.Card.CardId != OwnedCard.CardId));

                }
                else
                {
                    UserCards.AddRange(cards.Distinct(new UserCardEqualityComparer()));
                }

                var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(UserCards);

                SaveResponse(savedResult, Resources.MY_BUSIDEX_FILE);

                // Fire event handler
                OnMyBusidexLoaded?.Invoke(UserCards);

            }
            catch (Exception ex)
            {
                Xamarin.Insights.Report(new Exception("Error Loading My Busidex", ex));

                try
                {
                    _userCards =
                        Serialization.LoadData<List<UserCard>>(Path.Combine(Resources.DocumentsPath,
                            Resources.MY_BUSIDEX_FILE));
                }
                catch (Exception innerEx)
                {
                    Xamarin.Insights.Report(new Exception("Error Loading My Busidex From File", innerEx));
                }

                OnMyBusidexLoaded?.Invoke(UserCards);
            }
            finally
            {
                semaphore.Release();
            }

            return true;
        }
    }
}
