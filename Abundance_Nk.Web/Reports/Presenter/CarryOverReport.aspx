<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CarryOverReport.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.CarryOverReport" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
    <link href="../../Content/bootstrap.min.css" rel="stylesheet" />
    <title></title>
</head>
<body>


    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="60000">
     
        </asp:ScriptManager>
        <div>
 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <p>
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </p>
            <asp:Button ID="Button1" runat="server" Text="Display Report" class="btn btn-success" OnClick="Button1_Click" />

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
   
    <script src="~/Scripts/jquery-1.11.1.min.js"></script>
    <script src="../../Scripts/jquery-2.1.3.js"></script>
    <script src="../../Scripts/bootstrap.min.js"></script>
 
     

     

    </form>
     
    </body>
</html>

