using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace Busidex3.ViewModels {
    public delegate void OnMyOrganizationsLoadedEventHandler (List<Organization> organizations);
    public delegate void OnMyOrganizationsUpdatedEventHandler (ProgressStatus status);
    public delegate void OnMyOrganizationMembersUpdatedEventHandler (ProgressStatus status);
    public delegate void OnMyOrganizationMembersLoadedEventHandler (List<Card> cards);
    public delegate void OnMyOrganizationReferralsUpdatedEventHandler (ProgressStatus status);
    public delegate void OnMyOrganizationReferralsLoadedEventHandler (List<UserCard> cards);

    // public delegate void OnDetailTappedEventHandler (Organization org);


    public class OrganizationListVM : BaseViewModel
    {
        public event OnMyOrganizationsLoadedEventHandler OnMyOrganizationsLoaded;
        public event OnMyOrganizationMembersUpdatedEventHandler OnMyOrganizationMembersUpdated;
        public event OnMyOrganizationReferralsUpdatedEventHandler OnMyOrganizationReferralsUpdated;
        // public event OnDetailTappedEventHandler OnDetailTapped;

        public Dictionary<long, OnMyOrganizationMembersLoadedEventHandler> OrganizationMembersLoadedEventTable;
        public Dictionary<long, OnMyOrganizationReferralsLoadedEventHandler> OrganizationReferralsLoadedEventTable;

        private NamedSize _headerFont;
        public NamedSize HeaderFont
        {
            get => _headerFont;
            set {
                _headerFont = value;
                OnPropertyChanged(nameof(HeaderFont));
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing { 
            get => _isRefreshing;
            set {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        private List<Organization> _organizationList;
        public List<Organization> OrganizationList
        {
            get => _organizationList;
            set
            {
                _organizationList = value;
                OnPropertyChanged(nameof(OrganizationList));
            }
        }

        public ImageSource BackgroundImage
        {
            get
            {
                return ImageSource.FromResource("Busidex3.Resources.logo4.png",
                    typeof(SearchVM).Assembly);
            }
        }

        public Dictionary<long, List<Card>> OrganizationMembers { get;  private set; }= new Dictionary<long, List<Card>>();
        public Dictionary<long, List<UserCard>> OrganizationReferrals { get; private set; } = new Dictionary<long, List<UserCard>>();

        private readonly OrganizationsHttpService _organizationsHttpService;

        public OrganizationListVM()
        {
            _organizationsHttpService = new OrganizationsHttpService();
        }

        public override async Task<bool> Init(string cachedFile)
        {
            OrganizationList = Serialization.LoadData<List<Organization>> (Path.Combine (Serialization.LocalStorageFolder, StringResources.MY_ORGANIZATIONS_FILE));
            if (OrganizationList == null || OrganizationList.Count == 0) {
                OrganizationList = new List<Organization> ();
                await LoadOrganizations ();
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> LoadOrganizations()
        {
            using (var semaphore = new SemaphoreSlim(1, 1))
            {
                await semaphore.WaitAsync();

                try
                {
                    var organizationResult = await _organizationsHttpService.GetMyOrganizations();
                    if (organizationResult != null)
                    {
                        if (organizationResult.Model != null)
                        {

                            // Build the Organization List
                            OrganizationList.Clear();
                            OrganizationList.AddRange(organizationResult.Model);

                            var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(OrganizationList);
                            Serialization.SaveResponse(savedResult, StringResources.MY_ORGANIZATIONS_FILE);

                            var status = new ProgressStatus
                            {
                                Total = organizationResult.Model.Count
                            };
                        }

                        OnMyOrganizationsLoaded?.Invoke(OrganizationList);
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);

                    if (OrganizationList.Count == 0)
                    {
                        try
                        {
                            OrganizationList =
                                Serialization.LoadData<List<Organization>>(Path.Combine(Serialization.LocalStorageFolder,
                                    StringResources.MY_ORGANIZATIONS_FILE));
                        }
                        catch (Exception innerEx)
                        {
                            Crashes.TrackError(innerEx);
                        }

                        OnMyOrganizationsLoaded?.Invoke(OrganizationList);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return true;
        }

        public async Task<bool> LoadOrganizationReferrals(long organizationId)
        {
            using (var semaphore = new SemaphoreSlim(1, 1))
            {


                await semaphore.WaitAsync();

                try
                {
                    var result = await _organizationsHttpService.GetOrganizationReferrals(organizationId);

                    if (result != null)
                    {
                        Serialization.SaveResponse(Newtonsoft.Json.JsonConvert.SerializeObject(result.Model),
                            string.Format(StringResources.ORGANIZATION_REFERRALS_FILE, organizationId));

                        OrganizationReferrals = OrganizationReferrals ?? new Dictionary<long, List<UserCard>>();
                        if (!OrganizationReferrals.ContainsKey(organizationId))
                        {
                            OrganizationReferrals.Add(organizationId,
                                result.Model.Distinct(new UserCardEqualityComparer()).ToList());
                        }
                        else
                        {
                            OrganizationReferrals[organizationId] =
                                result.Model.Distinct(new UserCardEqualityComparer()).ToList();
                        }

                        var status = new ProgressStatus
                        {
                            Total = OrganizationReferrals[organizationId].Count
                        };

                        foreach (var card in result.Model)
                        {

                            var fImageUrl = StringResources.THUMBNAIL_PATH + card.Card.FrontFileName;
                            var bImageUrl = StringResources.THUMBNAIL_PATH + card.Card.BackFileName;
                            var fName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName;
                            var bName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.BackFileName;
                            if (!File.Exists(Serialization.LocalStorageFolder + "/" + fName))
                            {
                                try
                                {
                                    await App.DownloadImage(fImageUrl, Serialization.LocalStorageFolder, fName)
                                        .ContinueWith(r => { status.Count++; });
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                            else
                            {
                                status.Count++;
                            }

                            if (!File.Exists(Serialization.LocalStorageFolder + "/" + bName) &&
                                card.Card.BackFileId.ToString().ToLowerInvariant() != StringResources.EMPTY_CARD_ID)
                            {
                                try
                                {
                                    await App.DownloadImage(bImageUrl, Serialization.LocalStorageFolder, bName);
                                }
                                catch
                                {
                                    // ignored
                                }
                            }

                            OnMyOrganizationReferralsUpdated?.Invoke(status);
                        }
                    }

                    if (OrganizationReferralsLoadedEventTable.ContainsKey(organizationId) &&
                        OrganizationReferralsLoadedEventTable[organizationId] != null)
                    {
                        OrganizationReferralsLoadedEventTable[organizationId](OrganizationReferrals[organizationId]);
                    }

                }
                catch (Exception ex)
                {
                    if (OrganizationReferrals != null && (OrganizationReferralsLoadedEventTable != null &&
                                                          OrganizationReferralsLoadedEventTable.ContainsKey(organizationId) &&
                                                          OrganizationReferralsLoadedEventTable[organizationId] != null &&
                                                          OrganizationReferrals.ContainsKey(organizationId)))
                    {
                        OrganizationReferralsLoadedEventTable[organizationId](OrganizationReferrals[organizationId]);
                    }
                    Crashes.TrackError(ex);
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return true;
        }

        public async Task<bool> LoadOrganizationMembers(long organizationId)
        {
            using (var semaphore = new SemaphoreSlim(1, 1))
            {
                await semaphore.WaitAsync();

                try
                {
                    var result = await _organizationsHttpService.GetOrganizationMembers(organizationId);

                    if (result == null)
                    {

                        if (OrganizationMembersLoadedEventTable.ContainsKey(organizationId) &&
                            OrganizationMembersLoadedEventTable[organizationId] != null)
                        {
                            OrganizationMembersLoadedEventTable[organizationId](OrganizationMembers[organizationId]);
                        }
                    }
                    else
                    {

                        Serialization.SaveResponse(Newtonsoft.Json.JsonConvert.SerializeObject(result.Model),
                            string.Format(StringResources.ORGANIZATION_MEMBERS_FILE, organizationId));

                        OrganizationMembers = OrganizationMembers ?? new Dictionary<long, List<Card>>();
                        if (!OrganizationMembers.ContainsKey(organizationId))
                        {
                            OrganizationMembers.Add(organizationId, result.Model);
                        }
                        else
                        {
                            OrganizationMembers[organizationId] = result.Model;
                        }

                        var status = new ProgressStatus
                        {
                            Total = OrganizationMembers[organizationId].Count
                        };

                        foreach (var card in result.Model)
                        {

                            var fImageUrl = StringResources.THUMBNAIL_PATH + card.FrontFileName;
                            var bImageUrl = StringResources.THUMBNAIL_PATH + card.BackFileName;
                            var fName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
                            var bName = StringResources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;
                            if (!File.Exists(Serialization.LocalStorageFolder + "/" + fName))
                            {
                                try
                                {
                                    await App.DownloadImage(fImageUrl, Serialization.LocalStorageFolder, fName)
                                        .ContinueWith(r => { status.Count++; });
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                            else
                            {
                                status.Count++;
                            }

                            if (!File.Exists(Serialization.LocalStorageFolder + "/" + bName) &&
                                card.BackFileId.ToString().ToLowerInvariant() != StringResources.EMPTY_CARD_ID)
                            {
                                try
                                {
                                    await App.DownloadImage(bImageUrl, Serialization.LocalStorageFolder, bName);
                                }
                                catch
                                {
                                    // ignored
                                }
                            }

                            OnMyOrganizationMembersUpdated?.Invoke(status);
                        }

                        if (OrganizationMembersLoadedEventTable.ContainsKey(organizationId) &&
                            OrganizationMembersLoadedEventTable[organizationId] != null)
                        {
                            OrganizationMembersLoadedEventTable[organizationId](OrganizationMembers[organizationId]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (OrganizationMembersLoadedEventTable != null &&
                        OrganizationMembersLoadedEventTable.ContainsKey(organizationId) &&
                        OrganizationMembersLoadedEventTable[organizationId] != null)
                    {
                        if (OrganizationMembers != null) OrganizationMembersLoadedEventTable[organizationId](OrganizationMembers[organizationId]);
                    }
                    Crashes.TrackError(ex);
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return true;
        }

    }
}
