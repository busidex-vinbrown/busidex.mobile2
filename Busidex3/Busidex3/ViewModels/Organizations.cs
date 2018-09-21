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
    public delegate void OnMyOrganizationsLoadedEventHandler (List<Organization> organizations);
    public delegate void OnMyOrganizationsUpdatedEventHandler (ProgressStatus status);
    public delegate void OnMyOrganizationMembersUpdatedEventHandler (ProgressStatus status);
    public delegate void OnMyOrganizationMembersLoadedEventHandler (List<Card> cards);
    public delegate void OnMyOrganizationReferralsUpdatedEventHandler (ProgressStatus status);
    public delegate void OnMyOrganizationReferralsLoadedEventHandler (List<UserCard> cards);

    public class Organizations : BaseViewModel
    {
        
        public static event OnMyOrganizationsLoadedEventHandler OnMyOrganizationsLoaded;
        public static event OnMyOrganizationsUpdatedEventHandler OnMyOrganizationsUpdated;
        public static event OnMyOrganizationMembersUpdatedEventHandler OnMyOrganizationMembersUpdated;
        public static event OnMyOrganizationReferralsUpdatedEventHandler OnMyOrganizationReferralsUpdated;
        public static Dictionary<long, OnMyOrganizationMembersLoadedEventHandler> OrganizationMembersLoadedEventTable;
        public static Dictionary<long, OnMyOrganizationReferralsLoadedEventHandler> OrganizationReferralsLoadedEventTable;

        public static List<Organization> OrganizationList { get; set; }
        public static Dictionary<long, List<Card>> OrganizationMembers { get; set; }
        public static Dictionary<long, List<UserCard>> OrganizationReferrals { get; set; }

        private readonly string _authToken;
        private readonly OrganizationsHttpService _organizationsHttpService;
        public Organizations(string token)
        {
            _authToken = token;
            _organizationsHttpService = new OrganizationsHttpService();;
        }

        async Task<bool> LoadOrganizations()
        {
            var semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();

            try
            {
                var organizationResult = await _organizationsHttpService.GetMyOrganizations(_authToken);
                if (organizationResult != null)
                {

                    if (organizationResult.Model != null)
                    {

                        // Buid the Organization List
                        OrganizationList.Clear();
                        OrganizationList.AddRange(organizationResult.Model);

                        var savedResult = Newtonsoft.Json.JsonConvert.SerializeObject(OrganizationList);
                        SaveResponse(savedResult, Resources.MY_ORGANIZATIONS_FILE);

                        var status = new ProgressStatus();
                        status.Total = organizationResult.Model.Count;

                        OrganizationMembersLoadedEventTable =
                            new Dictionary<long, OnMyOrganizationMembersLoadedEventHandler>();
                        OrganizationReferralsLoadedEventTable =
                            new Dictionary<long, OnMyOrganizationReferralsLoadedEventHandler>();

                        foreach (Organization org in organizationResult.Model)
                        {
                            if (!OrganizationMembersLoadedEventTable.ContainsKey(org.OrganizationId))
                            {
                                OrganizationMembersLoadedEventTable.Add(org.OrganizationId, null);
                            }

                            if (!OrganizationReferralsLoadedEventTable.ContainsKey(org.OrganizationId))
                            {
                                OrganizationReferralsLoadedEventTable.Add(org.OrganizationId, null);
                            }
                        }

                        // Get Organization members and referals
                        foreach (Organization org in organizationResult.Model)
                        {

                            var fileName = org.LogoFileName + "." + org.LogoType;
                            var fImagePath = Resources.CARD_PATH + fileName;
                            if (!File.Exists(Resources.DocumentsPath + "/" + fileName))
                            {
                                try
                                {
                                    await DownloadImage(fImagePath, Resources.DocumentsPath, fileName).ContinueWith(
                                        result =>
                                        {
                                            status.Count++;
                                            OnMyOrganizationsUpdated?.Invoke(status);
                                        });
                                }
                                catch (Exception)
                                {

                                }
                            }
                            else
                            {
                                status.Count++;
                                OnMyOrganizationsUpdated?.Invoke(status);
                            }

                            // load organization members and referrals
                            await LoadOrganizationMembers(org.OrganizationId);
                            await LoadOrganizationReferrals(org.OrganizationId);
                        }
                    }

                    OnMyOrganizationsLoaded?.Invoke(OrganizationList);
                }
            }
            catch (Exception ex)
            {
                Xamarin.Insights.Report(new Exception("Error loading organization list", ex));

                if (OrganizationList.Count == 0)
                {
                    try
                    {
                        OrganizationList =
                            Serialization.LoadData<List<Organization>>(Path.Combine(Resources.DocumentsPath,
                                Resources.MY_ORGANIZATIONS_FILE));
                    }
                    catch (Exception innerEx)
                    {
                        Xamarin.Insights.Report(new Exception("Error loading organization list from file", innerEx));
                    }

                    OnMyOrganizationsLoaded?.Invoke(OrganizationList);
                }
            }
            finally
            {
                semaphore.Release();
            }

            return true;
        }

        public async Task<bool> LoadOrganizationReferrals(long organizationId)
        {
            var fileName = string.Format(Resources.ORGANIZATION_REFERRALS_FILE, organizationId);
            var semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();

            try
            {
                var result = await _organizationsHttpService.GetOrganizationReferrals(_authToken, organizationId);
                //.ContinueWith (async cards => {

                if (result != null)
                {
                    SaveResponse(Newtonsoft.Json.JsonConvert.SerializeObject(result.Model),
                        string.Format(Resources.ORGANIZATION_REFERRALS_FILE, organizationId));

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

                        var fImageUrl = Resources.THUMBNAIL_PATH + card.Card.FrontFileName;
                        var bImageUrl = Resources.THUMBNAIL_PATH + card.Card.BackFileName;
                        var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.FrontFileName;
                        var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.Card.BackFileName;
                        if (!File.Exists(Resources.DocumentsPath + "/" + fName))
                        {
                            try
                            {
                                await DownloadImage(fImageUrl, Resources.DocumentsPath, fName)
                                    .ContinueWith(r => { status.Count++; });
                            }
                            catch (Exception)
                            {

                            }
                        }
                        else
                        {
                            status.Count++;
                        }

                        if (!File.Exists(Resources.DocumentsPath + "/" + bName) &&
                            card.Card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID)
                        {
                            try
                            {
                                await DownloadImage(bImageUrl, Resources.DocumentsPath, bName);
                            }
                            catch (Exception)
                            {

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

                if (OrganizationReferralsLoadedEventTable != null &&
                    OrganizationReferralsLoadedEventTable.ContainsKey(organizationId) &&
                    OrganizationReferralsLoadedEventTable[organizationId] != null &&
                    OrganizationReferrals.ContainsKey(organizationId))
                {
                    OrganizationReferralsLoadedEventTable[organizationId](OrganizationReferrals[organizationId]);
                }

                Xamarin.Insights.Report(new Exception("Error Loading Organization Referrals", ex));
            }
            finally
            {
                semaphore.Release();
            }

            return true;
        }

        public async Task<bool> LoadOrganizationMembers(long organizationId)
        {
            var semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();

            try
            {
                var result = await _organizationsHttpService.GetOrganizationMembers(_authToken, organizationId);
                //.ContinueWith (async cards => {

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

                    SaveResponse(Newtonsoft.Json.JsonConvert.SerializeObject(result.Model),
                        string.Format(Resources.ORGANIZATION_MEMBERS_FILE, organizationId));

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

                        var fImageUrl = Resources.THUMBNAIL_PATH + card.FrontFileName;
                        var bImageUrl = Resources.THUMBNAIL_PATH + card.BackFileName;
                        var fName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.FrontFileName;
                        var bName = Resources.THUMBNAIL_FILE_NAME_PREFIX + card.BackFileName;
                        if (!File.Exists(Resources.DocumentsPath + "/" + fName))
                        {
                            try
                            {
                                await DownloadImage(fImageUrl, Resources.DocumentsPath, fName)
                                    .ContinueWith(r => { status.Count++; });
                            }
                            catch (Exception)
                            {

                            }
                        }
                        else
                        {
                            status.Count++;
                        }

                        if (!File.Exists(Resources.DocumentsPath + "/" + bName) &&
                            card.BackFileId.ToString().ToLowerInvariant() != Resources.EMPTY_CARD_ID)
                        {
                            try
                            {
                                await DownloadImage(bImageUrl, Resources.DocumentsPath, bName);
                            }
                            catch (Exception)
                            {

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
                    OrganizationMembersLoadedEventTable[organizationId](OrganizationMembers[organizationId]);
                }

                Xamarin.Insights.Report(new Exception("Error Loading Organization Members", ex));
            }
            finally
            {
                semaphore.Release();
            }

            return true;
        }

    }
}
