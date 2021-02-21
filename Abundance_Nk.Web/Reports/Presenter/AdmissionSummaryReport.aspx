<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdmissionSummaryReport.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.AdmissionSummaryReport" %>


<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../Content/bootstrap.css" rel="stylesheet" />
    <script src="../../Scripts/bootstrap.js"></script>
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

                    <div class="contentpanel">

                        <div class="panel panel-default">

                            <div class="panel-body">
                                <div class="form-inline">
                                    <div class="form-group">
                                        <label class="sr-only" for="ddlSession">Session</label>
                                        <asp:DropDownList ID="ddlSession" class="form-control" runat="server"></asp:DropDownList>
                                    </div>

                                    <div class="form-group">
                                        <asp:Button ID="btnDisplayReport" CssClass="btn btn-success mr5" runat="server" Text="Display Report" OnClick="btnDisplayReport_Click" />
                                    </div>
                                    <div class="form-group">
                                        <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
                                            <ProgressTemplate>
                                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/bx_loader.gif" />
                                                Loading...
                                            </ProgressTemplate>
                                        </asp:UpdateProgress>
                                    </div>

                                   </div>
                            </div>
                        </div>
                    </div>

                    <rsweb:ReportViewer ID="rv" runat="server"
                        Font-Names="Verdana" Font-Size="8pt"
                        ShowBackButton="False" ShowCredentialPrompts="False"
                        ShowDocumentMapButton="False" ShowFindControls="False"
                        ShowPageNavigationControls="False" ShowParameterPrompts="False"
                        ShowPromptAreaButton="False" ShowPrintButton="true" Height="800px" Width="100%" BackColor="White" BorderStyle="None">
                    </rsweb:ReportViewer>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
