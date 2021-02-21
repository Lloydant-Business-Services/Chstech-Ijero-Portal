<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotificationOfResultBulk.aspx.cs" Inherits="Abundance_Nk.Web.Reports.Presenter.Result.NotificationOfResultBulk" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <title></title>
</head>
<body style="border: none; margin: 0; padding: 0">


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
                                        <asp:DropDownList ID="ddlSession" runat="server" AutoPostBack="True" class="form-control" Height="35px" Width="150px" OnSelectedIndexChanged="ddlSession_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlSemester" runat="server" class="form-control" Height="35px" Width="150px">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlProgramme" runat="server" AutoPostBack="True" class="form-control" Height="35px" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged1" Width="150px">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlDepartment" runat="server" class="form-control" Height="35px" Width="150px">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlLevel" runat="server" class="form-control" Height="35px" Width="150px">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group">
                                        <asp:Button ID="Display_Button" runat="server" class="btn btn-success " OnClick="Display_Button_Click1" Text="Download" Width="100px" />
                                    </div>
                                    <div class="form-group">
                                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                                            <ProgressTemplate>
                                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/Images/bx_loader.gif" />
                                            </ProgressTemplate>
                                        </asp:UpdateProgress>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>


                    <br />


                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <script src="~/Scripts/jquery-1.11.1.min.js"></script>


    </form>

</body>
</html>
