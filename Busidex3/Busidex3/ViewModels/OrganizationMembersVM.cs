using Busidex3.DomainModels;
using Busidex3.Services;
using Busidex3.Services.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Busidex3.ViewModels
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
