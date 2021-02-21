using System;

namespace Abundance_Nk.Model.Model
{
    public class FeeDetailAudit
    {
        public int Id { get; set; }
        public FeeDetail FeeDetail { get; set; }
        public Fee Fee { get; set; }
        public FeeType FeeType { get; set; }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Level Level { get; set; }
        public Session Session { get; set; }
        public User User { get; set; }
        public string Operatiion { get; set; }
        public string Action { get; set; }
        public string Client { get; set; }
        public DateTime DateUploaded { get; set; }
    }
}
