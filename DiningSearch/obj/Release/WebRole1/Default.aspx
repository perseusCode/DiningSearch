<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebRole1._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">

    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
    <asp:Button ID="Search" runat="server" Text="Button" OnCommand="SearchStart"/>
    <p></p>
    <asp:Literal runat="server" ID="resultPanel"></asp:Literal>

</asp:Content>
