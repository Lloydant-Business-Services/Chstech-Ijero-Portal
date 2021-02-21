<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseRegistrationCountReport.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.CourseRegistrationCountReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../../Content/bootstrap.min.css" rel="stylesheet" />
    <title></title>
</head>
<body>


    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="6000000">
     
        </asp:ScriptManager>
        <div>
 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <p>
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </p>
            <div class="form-inline">


                <div class="form-group">

                    <asp:DropDownList ID="ddlSession" class="form-control" runat="server" Height="35px" Width="200px">
                    </asp:DropDownList>
                      <asp:DropDownList ID="ddlSemester" class="form-control" runat="server" Height="35px" Width="200px">
                    </asp:DropDownList>
                   
                    <asp:Button ID="Display_Button" runat="server" Text="Display Report" Width="111px" class="btn btn-success " OnClick="Display_Button_Click1" />
                                   <div class="form-group">
                                 <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
                                <ProgressTemplate>
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/bx_loader.gif" /> Loading...
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                                 </div>
                </div>
            </div>

        </ContentTemplate>
</asp:UpdatePanel>
</div>
    <script src="~/Scripts/jquery-1.11.1.min.js"></script>
    <script src="../../Scripts/jquery-2.1.3.js"></script>
    <script src="../../Scripts/bootstrap.min.js"></script>
 
  

        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%"></rsweb:ReportViewer>
      


    </form>

    </body>
</html>
