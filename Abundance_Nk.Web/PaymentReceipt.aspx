<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentReceipt.aspx.cs" Inherits="Abundance_Nk.Web.PaymentReceipt" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: center;">
    
        <% if(statuscode == "01" || statuscode == "00") {  %>
		<h2>Transaction Successful</h2>
		<p><b>Remita Retrieval Reference: </b> <%  Response.Write(rrr);  %></p>
		<p><b>Invoice / Reference: </b> <%  Response.Write(order_Id);  %></p>
		 <%  }else if(statuscode == "021") { %>
				<h2>RRR Generated Successfully</h2>
				<p><b>Remita Retrieval Reference: </b><% Response.Write(rrr);  %></p>
		 <%  }	else{ %>
	        <h2>Your Transaction was not Successful</h2>
		    <%  if (rrr !=null){ %>
		    <p>Your Remita Retrieval Reference is <span><b><%  Response.Write(rrr);  %></b></span><br /></p>
		    <% } %> 
		    <p><b>Reason: </b> <% Response.Write(message);  %></p>
		  <%  } %>

    </div>
    </form>
</body>
</html>
