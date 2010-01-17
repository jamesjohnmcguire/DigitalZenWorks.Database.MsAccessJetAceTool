<%@ Page language="c#" Codebehind="SimplePage.aspx.cs" AutoEventWireup="false" Inherits="SimplePage" %>
<HTML>
   <body>
      <form id="Form1" runat="server">
         Name:<asp:textbox id="name" runat="server" />
         <p />
         <asp:button id="MyButton" text="Click Here" OnClick="SubmitBtn_Click" runat="server" />
         <p />
         <span id="mySpan" runat="server"></span>
      </form>
   </body>
</HTML>
