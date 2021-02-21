function populateFirstSittingResultDetail() {
    var firstSittingOLevelResultDetailArray = [];
    var array = $('#firstSittingTable tr:gt(0)').map(function () {
        return {
            SubjectId: $(this.cells[0]).find("select").val(),
            SubjectName: "",
            GradeId: $(this.cells[1]).find("select").val(),
            GradeName: ""
        };
    });

    for (var i = 0; i < array.length; i++) {
        var myArray = { "SubjectId": array[i].SubjectId, "SubjectName": array[i].SubjectName, "GradeId": array[i].GradeId, "GradeName": array[i].GradeName };
        firstSittingOLevelResultDetailArray.push(myArray);
    }

    return firstSittingOLevelResultDetailArray;
}

function populateSecondSittingResultDetail() {
    var secondSittingOLevelResultDetailArray = [];
    var array2 = $('#secondSittingTable tr:gt(0)').map(function () {
        return {
            SubjectId: $(this.cells[0]).find("select").val(),
            SubjectName: "",
            GradeId: $(this.cells[1]).find("select").val(),
            GradeName: ""
        };
    });

    for (var i = 0; i < array2.length; i++) {
        var myArray = { "SubjectId": array2[i].SubjectId, "SubjectName": array2[i].SubjectName, "GradeId": array2[i].GradeId, "GradeName": array2[i].GradeName };
        secondSittingOLevelResultDetailArray.push(myArray);
    }

    return secondSittingOLevelResultDetailArray;
}

function showNotification(colorName, text, placementFrom, placementAlign, animateEnter, animateExit) {
    if (colorName === null || colorName === '') { colorName = 'bg-purple'; }
    if (text === null || text === '') { text = ""; }
    if (animateEnter === null || animateEnter === '') { animateEnter = 'animated fadeInRight'; }
    if (animateExit === null || animateExit === '') { animateExit = 'animated fadeOutRight'; }

    $.notify({
        message: text
    },
        {
            type: colorName,
            allow_dismiss: true,
            newest_on_top: true,
            timer: 1000,
            placement: {
                from: placementFrom,
                align: placementAlign
            },
            animate: {
                enter: animateEnter,
                exit: animateExit
            },
            template: '<div data-notify="container" class="bootstrap-notify-container alert alert-dismissible {0} ' + (true ? "p-r-35" : "") + '" role="alert">' +
            '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
            '<span data-notify="icon"></span> ' +
            '<span data-notify="title">{1}</span> ' +
            '<span data-notify="message">{2}</span>' +
            '<div class="progress" data-notify="progressbar">' +
            '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
            '</div>' +
            '<a href="{3}" target="{4}" data-notify="url"></a>' +
            '</div>'
        });
}


function assignFormValues() {

    $("#lastName").text($("#Person_LastName").val());
    $("#firstName").text($("#Person_FirstName").val());
    $("#otherName").text($("#Person_OtherName").val());
    $("#sex").text($("#Person_Sex_Id option:selected").text());
    $("#dateOfBirth").text($("#Person_DateOfBirth").val());
    $("#state").text($("#Person_State_Id option:selected").text());
    $("#localGovernment").text($("#Person_LocalGovernment_Id option:selected").text());
    $("#homeTown").text($("#Person_HomeTown").val());
    $("#mobilePhone").text($("#Person_MobilePhone").val());
    $("#email").text($("#Person_Email").val());
    $("#religion").text($("#Person_Religion_Id option:selected").text());
    $("#homeAddress").text($("#Person_HomeAddress").val());
    $("#ability").text($("#Applicant_Ability_Id option:selected").text());
    $("#otherAbility").text($("#Applicant_OtherAbility").val());
    $("#extraCurricullarActivities").text($("#Applicant_ExtraCurricullarActivities").val());
    $("#sponsorName").text($("#Sponsor_Name").val());
    $("#sponsorContactAddress").text($("#Sponsor_ContactAddress").val());
    $("#sponsorPhone").text($("#Sponsor_MobilePhone").val());
    $("#relationship").text($("#Sponsor_Relationship_Id option:selected").text());
    $("#firstSittingOLevelResultTypeName").text($("#FirstSittingOLevelResult_Type_Id option:selected").text());
    $("#firstSittingOLevelResultExamNumber").text($("#FirstSittingOLevelResult_ExamNumber").val());
    $("#firstSittingOLevelResultExamYear").text($("#FirstSittingOLevelResult_ExamYear").val());
    if ($("#SecondSittingOLevelResult_Type_Id option:selected").text() === '-- Select --') {
        $("#secondSittingOLevelResultTypeName").text("");
    }
    else {
        $("#secondSittingOLevelResultTypeName").text($("#SecondSittingOLevelResult_Type_Id option:selected").text());
    }
    if ($("#SecondSittingOLevelResult_ExamYear").val() == '0') {
        $("#secondSittingOLevelExamYear").text("");
    }
    else {
        $("#secondSittingOLevelExamYear").text($("#SecondSittingOLevelResult_ExamYear").val());
    }

    $("#secondSittingOLevelExamNumber").text($("#SecondSittingOLevelResult_ExamNumber").val());
    $("#programmeName").text($("#AppliedCourse_Programme_Name").val());
    $("#departmentName").text($("#AppliedCourse_Department_Name").val());
    $("#facultyName").text($("#AppliedCourse_Department_Faculty_Name").val());
    $("#optionName").text($("#AppliedCourse_Option_Name").val());
    $("#secondOptionName").text($("#AppliedCourse_OptionSecondChoice_Id option:selected").text());
    if ($("#PreviousEducation_PreviousSchool_Id option:selected").text() != "-- Select --")
    {
        $("#previousSchool").text($("#PreviousEducation_PreviousSchool_Id option:selected").text());
    }
    else
    {
        $("#previousSchool").text($("#PreviousEducation_SchoolName").val());
    }
    $("#previousSchoolCourse").text($("#PreviousEducation_Course").val());
    $("#previousSchoolQualification").text($("#PreviousEducation_Qualification_Id option:selected").text());
    $("#previousSchoolGrade").text($("#PreviousEducation_ResultGrade_Id option:selected").text());
    $("#firstSittingOLevelResultScratchCard").text($("#FirstSittingOLevelResult_ScratchCard").val());
    $("#secondSittingOLevelResultScratchCard").text($("#SecondSittingOLevelResult_ScratchCard").val());
    $("#firstSittingOLevelResultScratchCardSerialNo").text($("#FirstSittingOLevelResult_ScratchCardSerialNo").val());
    $("#secondSittingOLevelResultScratchCardSerialNo").text($("#SecondSittingOLevelResult_ScratchCardSerialNo").val());
    $("#itStartMonth").text($("#PreviousEducation_ITStartMonth_Id option:selected").text());
    $("#itStartYear").text($("#PreviousEducation_ITStartYear_Id").val());
    $("#itEndMonth").text($("#PreviousEducation_ITEndMonth_Id option:selected").text());
    $("#itEndYear").text($("#PreviousEducation_ITEndYear_Id").val());

}

function populateOLevel(result, type) {
    if (type === 1) {

        $("#firstSittingPreview").empty();

        for (var i = 0; i < result.length; i++) {
            if (result[i].SubjectName !== "") {
                $("#firstSittingPreview").append('<tr><td>' + result[i].SubjectName + '</td>' + '<td>' + result[i].GradeName + '</td></tr>');
            }
        }
    } else {

        $("#secondSittingPreview").empty();

        for (var j = 0; j < result.length; j++) {
            if (result[j].SubjectName !== "") {
                $("#secondSittingPreview").append('<tr><td>' + result[j].SubjectName + '</td>' + '<td>' + result[j].GradeName + '</td></tr>');
            }

        }
    }
}

function toastMessage(msg, title) {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",
        "tapToDismiss": false
    };

    var $toast = toastr["success"](msg, title);
}