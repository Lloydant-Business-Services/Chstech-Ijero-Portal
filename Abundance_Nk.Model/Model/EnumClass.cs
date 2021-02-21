using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Entity.Model
{
    public class EnumClass
    {
    }
    public enum Levels
    {
        NDI = 1,
        NDII = 2,
        HNDI = 3,
        HNDII = 4,
        NDYRI = 5,
        NDYRII = 6,
        NDYRIII = 7,
        HNDYRI = 8,
        HNDYRII = 9,
        HNDYRIII = 10,
        HNDEI = 11,
        HNDEII = 12,
        NDEI = 13,
        NDEII = 14
    }
    public enum Semesters
    {
        FirstSemester = 1,
        SecondSemester = 2
    }
    public enum Grades
    {
        PassMark = 40
    }
    public enum Sexes
    {
        Male = 1,
        Female = 2
    }

    public enum TranscriptStatusList
    {
        RequestSent = 1,
        RequestReceived = 2,
        AwaitingPaymentConfirmation = 3,
        RequestProcessed = 4,
        RequestDispatched = 5,
        RequestDelivered = 6
    }

    public enum TranscriptClearanceStatusList
    {
        DepartmentClearance = 1,
        BursaryClearance = 2,
        RegistryClearance = 3,
        Completed = 4
    }
    public enum PaymentTypeEnum
    {
        CardPayment = 1,
        OnlinePayment = 2,
    }
    

}
