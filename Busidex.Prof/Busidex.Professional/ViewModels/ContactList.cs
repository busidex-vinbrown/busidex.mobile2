using Plugin.ContactService.Shared;
using System.Collections.Generic;

namespace Busidex.Professional.ViewModels
{
    public class ContactList : List<Contact>
    {
        public ContactList()
        {

        }

        public ContactList(List<Contact> contacts)
        {
            Contacts.AddRange(contacts);
        }

        public string Heading { get; set; }
        public List<Contact> Contacts => this;
    }
}
