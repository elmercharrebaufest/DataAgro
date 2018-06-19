cd /d %~dp0
START cmd.exe /k "Molinos.DataAgro.Web.deploy.cmd /Y -setParamFile:%~dp0Web\<ambiente>.DeployParameters.xml"