<%@ Page
	Language           = "C#"
	AutoEventWireup    = "false"
	Inherits           = "Migrator.Web.Default"
	ValidateRequest    = "false"
	EnableSessionState = "false"
%>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Migrator.Web</title>

		<meta http-equiv="content-type" content="text/html; charset=utf-8" />
		<meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />
		<meta http-equiv="PRAGMA" content="NO-CACHE" />

		<link href="Migrator.Web.css" type="text/css" rel="stylesheet" />
		
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
		<!-- Site Code goes here! -->
			<table class="feature">

				<tr>
					<th colspan="2">
						Run Migration
					</th>
				</tr

				<tr>
					<td>
						Current Database Latest Version:
					</td>
					<td>
						<asp:Label id="_LatestVersion" runat="server" />
					</td>
				</tr>

				<tr>
					<td>
						Migrate To:
					</td>
					<td>
						<asp:DropDownList id="_availableVersions" runat="server" />
					</td>
				</tr>

				<tr>
					<td colspan="2">
						<asp:Button id="_runMigration" text="Run Migration" runat="server" />
					</td>
				</tr>

			</table>

		</form>
	</body>
</html>
