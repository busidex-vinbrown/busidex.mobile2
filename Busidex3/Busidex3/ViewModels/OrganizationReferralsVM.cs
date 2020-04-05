using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Busidex3.ViewModels
{
    public class OrganizationReferralsVM : CardListVM
    {
        private Organization _organization;
        public Organization Organization
        {
            get => _organization;
            set
            {
                _organization = value;
                OnPropertyChanged(nameof(Organization));
            }
        }

        public string OrganizationReferralsFile { get; set; }

        private readonly OrganizationsHttpService _organizationsHttpService;

        public OrganizationReferralsVM(Organization org)
        {
            _organizationsHttpService = new OrganizationsHttpService();
            Organization = org;
            OrganizationReferralsFile = string.Format(StringResources.ORGANIZATION_REFERRALS_FILE, org.OrganizationId);
        }

        public new ImageSource BackgroundImage {
            get {
                return ImageSource.FromResource("Busidex3.Resources.logo4.png",
                    typeof(SearchVM).Assembly);
            }
        }

        public override void SaveCardsToFile(string json)
        {
            Serialization.SaveResponse(json, OrganizationReferralsFile);
        }

        public override async Task<List<UserCard>> GetCards()
        {
            var result = await _organizationsHttpService.GetOrganizationReferrals(Organization.OrganizationId);
            
            return result.Model;
        }
    }
}
