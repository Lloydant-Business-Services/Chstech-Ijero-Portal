//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Abundance_Nk.Model.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class E_CHAT_RESPONPSE
    {
        public long E_Chat_Response_Id { get; set; }
        public string Response { get; set; }
        public Nullable<long> Student_Id { get; set; }
        public Nullable<long> User_Id { get; set; }
        public bool Active { get; set; }
        public long E_Chat_Topic_Id { get; set; }
        public System.DateTime Response_Time { get; set; }
        public string Upload { get; set; }
    
        public virtual E_CHAT_TOPIC E_CHAT_TOPIC { get; set; }
        public virtual STUDENT STUDENT { get; set; }
        public virtual USER USER { get; set; }
    }
}
