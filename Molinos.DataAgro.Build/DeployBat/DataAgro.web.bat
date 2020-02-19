cd /d %~dp0
START cmd.exe /k "WebDataAgro.deploy.cmd /Y -setParamFile:%~dp0Web\<ambiente>.DeployParameters.xml"