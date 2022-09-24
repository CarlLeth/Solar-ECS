using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Contacts
{
    public class ContactInfo
    {
        public string Name { get; private set; }
        public string Title { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public string Organization { get; private set; }

        public ContactInfo(string name, string title, string phone, string email, string organization)
        {
            this.Title = title;
            this.Phone = phone;
            this.Name = name;
            this.Email = email;
            this.Organization = organization;
        }

        private ContactInfo() { }
    }
}
