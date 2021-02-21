
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public enum FeeTypes
    {
        NDFullTimeApplicationForm = 1,
        AcceptanceFee = 2,
        SchoolFees = 3,
        HNDFullTimeApplicationForm = 4,
        HNDAcceptance = 9,
        CarryOverSchoolFees = 10,
        ChangeOfCourseFees = 11,
        ShortFall = 12,
        Transcript = 13,
        HostelFee = 17,
        ConvocationFee = 18,
        CerificateCollection = 14,
        LateSchoolFees = 20,
        Ewallet_Shortfall = 22
    }

    public enum CourseModes
    {
        FirstAttempt = 1,
        CarryOver = 2,
        ExtraYear = 3
    }
    public enum PersonTypes
    {
        Staff = 1,
        Parent = 2,
        Student = 3,
        Applicant = 4
    }

    public enum PaymentModes
    {
        Full = 1,
        FirstInstallment = 2,
        SecondInstallment = 3,
        ThirdInstallment = 4,
        FourthInstallment = 5
    }

    public enum Paymenttypes
    {
        CardPayment = 1,
        OnlinePayment = 2,
    }

}
