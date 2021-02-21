using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class ApplicantSponsor
    {
        public long Person_Id { get; set; }
        public int Relationship_Id { get; set; }
        public string Sponsor_Name { get; set; }
        public string Sponsor_Contact_Address { get; set; }
        public string Sponsor_Mobile_Phone { get; set; }
        public long Application_Form_Id { get; set; }

        public virtual ApplicationForm ApplicationForm{ get; set; }
        public virtual Person Person { get; set; }
        public virtual Relationship Relationship { get; set; }
    }
}
