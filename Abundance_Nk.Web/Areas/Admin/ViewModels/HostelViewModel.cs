using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity.Model;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class HostelViewModel
    {
        public HostelViewModel()
        {
            SessionSelectListItem = Utility.PopulateAllSessionSelectListItem();
            ProgrammeSelectListItem = Utility.PopulateAllProgrammeSelectListItem();
            //DepartmentSelectListItem = Utility.PopulateDepartmentSelectListItem(Programme);
            //HostelSeriesSelectListItem = Utility.PopulateHostelSeries();
            HostelSelectListItem = Utility.PopulateHostels();
            LevelSelectListItem = Utility.PopulateLevelSelectListItem();
            HostelTypeSelectListItem = Utility.PopulateHostelTypes();
        }
        public HostelRoom HostelRoom { get; set; }
        public HostelAllocationCriteria HostelAllocationCriteria { get; set; }
        public Hostel Hostel { get; set; }
        public HostelRoomCorner HostelRoomCorner { get; set; }
        public HostelSeries HostelSeries { get; set; }
        public List<HostelSeries> HostelSeriesList { get; set; }
        public List<HostelRoom> HostelRoomList { get; set; }
        public List<RoomSetting> RoomSettings { get; set; }
        public List<HostelType> HostelTypes { get; set; }
        public HostelType HostelType { get; set; }
        public string[] SelectedCorners { get; set; }
        public List<string> Corners { get; set; }
        public List<SelectListItem> DepartmentSelectListItem { get; set; }
        public List<SelectListItem> DepartmentOpionSelectListItem { get; set; }
        public List<SelectListItem> ProgrammeSelectListItem { get; set; }
        public List<SelectListItem> LevelSelectListItem { get; set; }
        public List<SelectListItem> SessionSelectListItem { get; set; }
        public List<SelectListItem> HostelSelectListItem { get; set; }
        public List<SelectListItem> HostelSeriesSelectListItem { get; set; }
        public List<SelectListItem> HostelRoomsSelectListItem { get; set; }
        public List<HostelAllocationCriteria> HostelAllocationCriterias { get; set; }
        public Model.Model.Student Student { get; set; }
        public Person Person { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public HostelAllocation HostelAllocation { get; set; }
        public HostelRequest HostelRequest { get; set; }
        public HostelAllocationCount HostelAllocationCount { get; set; }
        public List<HostelAllocationCount> HostelAllocationCounts { get; set; }
        public List<HostelAllocation> HostelAllocations { get; set; }
        public List<HostelRoomCorner> HostelRoomCorners { get; set; }
        public List<HostelRequest> HostelRequests { get; set; }
        public Session Session { get; set; }
        public Programme Programme { get; set; }
        public Level Level { get; set; }
        public Department Department { get; set; }
        public List<DistinctAllocation> DistinctAllocation { get; set; }
        public List<SelectListItem> HostelTypeSelectListItem { get; set; }
        public List<Hostel> Hostels { get; set; }
        public int Id { get; set; }
        public List<HostelRequestCount> HostelRequestCounts { get; set; }
    }

    public class DistinctAllocation
    {
        public string Level { get; set; }
        public string HostelType { get; set; }
        public string Hostel { get; set; }
        public string Series { get; set; }
        public long FreeAllocationCount { get; set; }
        public long ReservedAllocationAccount { get; set; }
        public long CriteriaCount { get; set; }
        public string RoomCorner { get; set; }
        public int UsedCriteriaCount { get; set; }
        public int UnusedCriteriaCount { get; set; }
    }
    public partial class ArrayJsonView
    {
        public string Description { get; set; }
        public string Date { get; set; }
        public string Activated { get; set; }
        public string HostelType { get; set; }
        public string HostelTypeName { get; set; }
        public string Capacity { get; set; }
        public string Hostel { get; set; }
        public string HostelName { get; set; }
    }
    public partial class FeeSetupJsonView
    {
        public string FeeTypeId { get; set; }
        public string FeeSetupName { get; set; }
        public string Activated { get; set; }
        public string PaymentModeId { get; set; }
        public string SessionId { get; set; }
        public string Amount { get; set; }
        public string FeeSetUpId { get; set; }
    }
    public partial class FeeJsonView
    {
        public int feeId { get; set; }
    }
}