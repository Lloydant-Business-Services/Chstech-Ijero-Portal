<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DebtorsReport.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.DebtorsReport" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

        <link href="../../Content/bootstrap.css" rel="stylesheet" />
    <script src="../../Scripts/bootstrap.js"></script>
     <link href="../../Content/misc.css" rel="stylesheet" />

</head>
<body style="border:none; margin:0; padding:0">
    <form id="form1" runat="server">
     <div>
         &nbsp;<asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeOut="60000">
         </asp:ScriptManager>
         <asp:UpdatePanel ID="UpdatePanel1" runat="server">
             <ContentTemplate>

                    <p>
                        <asp:Label ID="lblMessage" runat="server" BackColor="Yellow" ForeColor="Red" ></asp:Label>
         </p>

                 <div class="contentpanel">
                <div class="panel panel-default">
                    <div class="panel-body">
                        
                        <%--<div class="row">
                            <div class="col-md-12">
                                
                                 <div class="form-inline col-md-offset-1">
                                    <div class="form-group">
                                        <label  title="Date Range">Date Range: </label> &nbsp;
                                    </div>
                                    <div class="form-group" >
                    
                                    <asp:TextBox ID="txtBoxDateFrom" runat="server" RepeatDirection="Horizontal" placeholder="From Inception" disabled ="disabled"></asp:TextBox>
                                
                                    <asp:TextBox ID="txtBoxDateTo" runat="server" RepeatDirection="Horizontal" class="txtDatePicker" placeholder="To"></asp:TextBox>
                                </div>
                        </div>

                                </div>
                            </div>
                        <br/>--%>

                        <div class="row">
                            <div class="col-md-12">
                                </div>
                            
                           <%-- <br/>
                            <div class="col-md-offset-1">
                                <p><i>For bulk report, select session and programme</i></p>
                            </div>
                            --%>

                            </div>

                        <div class="row col-md-offset-1">
                             <div class="col-md-12">
                        <div class="form-inline">
                            <div class="form-group">
                                <label class="sr-only" for="ddlSession"></label>
                                <asp:DropDownList ID="ddlSession" cssclass="form-control" runat="server"></asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label class="sr-only" for="ddlLevel"></label>
                                <asp:DropDownList ID="ddlLevel" cssclass="form-control" runat="server"></asp:DropDownList>
                            </div>
                                    
                            <div class="form-group">
                                <label class="sr-only" for="ddlProgramme"></label>
                                <asp:DropDownList ID="ddlProgramme" cssclass="form-control" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged"></asp:DropDownList>
                            </div>

                            <div class="form-group">
                                <label class="sr-only" for="ddlDepartment"></label>
                                <asp:DropDownList ID="ddlDepartment" cssclass="form-control" runat="server"></asp:DropDownList>
                            </div>

                             <div class="form-group">
                            <asp:Button ID="btnDisplayReport" cssclass="btn btn-danger mr5" runat="server" Text="Display Report" OnClick="btnDisplayReport_Click" /> &nbsp;
                                <%--<asp:Button ID="btnBulk" cssclass="btn btn-danger mr5" runat="server" Text="Bulk Report" OnClick="btnBulk_Click"/> &nbsp;
                                 <asp:Button ID="btnFullReport" cssclass="btn btn-danger mr5" runat="server" Text="All Report" OnClick="btnFullReport_Click"/> --%>
                                 
                             </div>
                             <div class="form-group">
                                 <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
                                <ProgressTemplate>
                                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/bx_loader.gif" /> Loading...
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                                 </div>
                        </div>
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
        ShowPromptAreaButton="False" ShowPrintButton="true" Height="800px" Width="100%" BackColor="White" BorderStyle="None" >
        </rsweb:ReportViewer>

                   </ContentTemplate>
         </asp:UpdatePanel>

    </div>
        
        
        <script src="~/Scripts/jquery-1.11.1.min.js"></script>
    <script src="../../Scripts/jquery-2.1.3.js"></script>
    <script src="../../Scripts/bootstrap.min.js"></script>
 
        <link href="../../Content/jquery-ui-1.10.3.css" rel="stylesheet" />
        <script src="../../Scripts/file-upload/jquery-ui-1.9.2.min.js"></script>

      <script type="text/javascript">
            $(function () {
                $('.txtDatePicker').datepicker(
                {
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: '1950:2100'
                });
            })
        </script>
        

    </form>
</body>
</html>

