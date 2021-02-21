using Abundance_Nk.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class HostelAllocation
    {
        public long Id { get; set; }
        public bool Occupied { get; set; }
        public HostelRoomCorner Corner { get; set; }
        public Hostel Hostel { get; set; }
        public HostelRoom Room { get; set; }
        public HostelSeries Series { get; set; }
        public Session Session { get; set; }
        public Student Student{ get; set; }
        public Payment Payment { get; set; }
        public Person Person { get; set; }
        public int HostelId { get; set; }
        public string HostelName { get; set; }
        public int SeriesId { get; set; }
        public string SeriesName { get; set; }
        public long RoomId { get; set; }
        public string RoomName { get; set; }
        public long CornerId { get; set; }
        public string CornerName { get; set; }
        public string ReserveStatus { get; set; }
    }
}
