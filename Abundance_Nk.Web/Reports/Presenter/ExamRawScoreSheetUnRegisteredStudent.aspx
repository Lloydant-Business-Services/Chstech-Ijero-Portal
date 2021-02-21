<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExamRawScoreSheetUnRegisteredStudent.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.ExamRawScoreSheetUnRegisteredStudent" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../../Content/bootstrap.min.css" rel="stylesheet" />
    <title></title>
   </head>
<body>
    <form id="form1" runat="server">
            <p>
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </p>
            <div class="form-inline">


                <div class="form-group">

                    <asp:DropDownList ID="ddlSession" class="form-control" runat="server" Height="35px" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="ddlSession_SelectedIndexChanged1">
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlSemester" class="form-control" runat="server" Height="35px" Width="200px">
                    </asp:DropDownList>
                     <asp:DropDownList ID="ddlLevel" class="form-control" runat="server" Height="35px" Width="250px">
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlProgramme" class="form-control" runat="server" Height="35px" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged1">
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlDepartment" class="form-control" runat="server" Height="35px" Width="250px"  AutoPostBack="True" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlCourse" class="form-control" runat="server" Height="35px" Width="250px">
                    </asp:DropDownList>
                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                    </asp:ScriptManager>
                    <asp:Button ID="Display_Button" runat="server" Text="Display Report" Width="111px" class="btn btn-success " OnClick="Display_Button_Click1" Visible="False" />
                </div>
                  <div class="form-group">
                               <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <p>
                <asp:Label ID="Label1" runat="server"></asp:Label>
            </p>

                <div class="form-group">
                                 <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
                                <ProgressTemplate>
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/bx_loader.gif" /> Loading...
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                                 </div>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="970px">
        </rsweb:ReportViewer>
 
        </ContentTemplate>
</asp:UpdatePanel>
            </div>

    <div>
    
    </div>
    </form>
</body>
</html>
