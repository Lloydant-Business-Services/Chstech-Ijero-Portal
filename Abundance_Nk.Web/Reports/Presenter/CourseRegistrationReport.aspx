<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseRegistrationReport.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.CourseRegistrationReport" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

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
                <div class="form-group">
                    <div class="row;form-control">
                        <asp:RadioButton ID="rbnDepartment" runat="server" AutoPostBack="True" GroupName="Selector" OnCheckedChanged="rbnDepartment_CheckedChanged" Text="By Department" />
                    </div>
                    <div class="row;form-control">
                    <asp:RadioButton ID="rbnLevel" runat="server" AutoPostBack="True" GroupName="Selector" OnCheckedChanged="rbnLevel_CheckedChanged" Text="By Level" />
                    </div>
                </div>
                
                <p>
                </p>
                <div class="form-inline">
                    <div class="form-group">
                        <asp:DropDownList ID="ddlSession" runat="server" class="form-control" Height="35px" Width="200px">
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlProgramme" runat="server" AutoPostBack="True" class="form-control" Height="35px" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged1" Width="200px">
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlDepartment" runat="server" class="form-control" Height="35px" Width="200px">
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlLevel" runat="server" class="form-control" Height="35px" Width="200px">
                        </asp:DropDownList>
                        <asp:Button ID="Display_Button" runat="server" class="btn btn-success " OnClick="Display_Button_Click1" Text="Display Report" Width="111px" />
                        <div class="form-group">
                            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                                <ProgressTemplate>
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/bx_loader.gif" />
                                    Loading...
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </div>
                    </div>
                </div>
                
                <p>
                </p>
                
                <p>
                </p>
                
                <p>
                </p>
                
                <p>
                </p>
                
                <p>
                </p>
                
                <p>
                </p>
                
                <p>
                </p>
                
            </p>

        </ContentTemplate>
</asp:UpdatePanel>
</div>
    <script src="~/Scripts/jquery-1.11.1.min.js"></script>
    <script src="../../Scripts/jquery-2.1.3.js"></script>
    <script src="../../Scripts/bootstrap.min.js"></script>
 
  

        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%">
        </rsweb:ReportViewer>
 
      


    </form>

    </body>
</html>




