using Busidex.Http;
using Busidex.Http.Utils;
using Busidex.Models.Domain;
using Busidex.Resources.String;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Busidex.Professional.ViewModels
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
                return ImageSource.FromResource("Busidex.Resources.Images.logo4.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
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
