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
    public class OrganizationMembersVM : CardListVM
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

        public string OrganizationMembersFile { get; set; }

        private readonly OrganizationsHttpService _organizationsHttpService;

        public OrganizationMembersVM(Organization org)
        {
            _organizationsHttpService = new OrganizationsHttpService();
            Organization = org;
            OrganizationMembersFile = string.Format(StringResources.ORGANIZATION_MEMBERS_FILE, org.OrganizationId);
        }

        public override void SaveCardsToFile(string json)
        {
            Serialization.SaveResponse(json, OrganizationMembersFile);
        }

        public new ImageSource BackgroundImage {
            get {
                return ImageSource.FromResource("Busidex.Resources.Images.logo4.png",
                    typeof(Busidex.Resources.Images.ImageLoader).GetTypeInfo().Assembly);
            }
        }

        public override async Task<List<UserCard>> GetCards()
        {
            var result = await _organizationsHttpService.GetOrganizationMembers(Organization.OrganizationId);
            var list = new List<UserCard>();
            result.Model.ForEach(c =>
            {
                list.Add(new UserCard(c)
                {
                    ExistsInMyBusidex = c.ExistsInMyBusidex,
                    Card = c,
                    CardId = c.CardId
                });
            });
            return list;
        }
    }
}
