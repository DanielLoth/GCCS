$msbuildPath = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe'
$sqlPkg = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\140\SqlPackage.exe'

$projectPath = Get-Location
$outputPath = "$projectPath\bin\debug"
$dacPath = "$outputPath\GCCS.Database.dacpac"

$dbServer = '.'
$sqlDb = 'GCCS'
$profilePath = "$projectPath\Local.publish.xml"
$permissionPath = $null

#build project with MSBUILD to create dacpac sql package
& $msbuildPath $projectPath\GCCS.Database.sqlproj /t:Clean
& $msbuildPath $projectPath\GCCS.Database.sqlproj /p:Configuration=Debug

function runSqlPublish{
param(
    [string] $dbServer,
    [string] $sqlDb,
    [string] $sqlFile,
    [string] $dacPath,
    [string] $profilePath,
    [string] $permissionPath,
    [string] $sqlPkg,
    [switch] $PublishDirectly
    )

	try
	{
		#to be used with action:publish $parameters = "{DatabaseRoles | FileGroups | ExtendedProperties | RoleMembership | Users | ServerRoleMemberShip | ServerRoles}"

        $vsdbcmdArgs = "/SourceFile:""$dacPath"" /TargetConnectionString:""Data Source=$dbServer;Integrated Security=False;Initial Catalog=$sqlDb;"" /Profile:""$profilePath"""

        if ($publishDirectly)
        {
            $vsdbcmdArgs += " /Action:Publish"
        }
        else
        {
            $vsdbcmdArgs += " /Action:Script /OutputPath:""$sqlFile"""
        }

        Write-Output "$vsdbcmdArgs"

		# Build Startinfo and set options according to parameters
		$startinfo = new-object System.Diagnostics.ProcessStartInfo 
		$startinfo.FileName = $sqlPkg
		$startinfo.Arguments = $vsdbcmdArgs
		$startinfo.WindowStyle = "Hidden"
		$startinfo.CreateNoWindow = $TRUE
		$startinfo.UseShellExecute = $FALSE
		$startinfo.RedirectStandardOutput = $TRUE
		$startinfo.RedirectStandardError = $TRUE
		$process = [System.Diagnostics.Process]::Start($startinfo)
		Write-Output  $process.StandardOutput.ReadToEnd()
		Write-Output  $process.StandardError.ReadToEnd()
		$process.WaitForExit()

		#Add-PSSnapin SqlServerProviderSnapin140
		#Add-PSSnapin SqlServerCmdletSnapin140
		#Write-Output "Run '$sqlFile' on $dbServer/$sqlDb"
		#Invoke-Sqlcmd -InputFile "$sqlFile" -Database "$sqlDb" -ServerInstance "$dbServer" -Verbose -ErrorAction Stop

		#if(!([string]::IsNullOrEmpty($permissionPath)))
		#{
		#	Write-Output "Run '$sqlFile' on $deployDbServer/$deployDb"
		#	Invoke-Sqlcmd -InputFile $permissionPath -Database "$sqlDb" -ServerInstance "$dbServer"
		#}
	}

	catch
	{
		Write-Error $_.Exception.Message
		exit 1
	}    
}

function runSqlDiffReport{
param(
    [string] $dbServer,
    [string] $sqlDb,
    [string] $dacPath,
    [string] $outPutResultPath,
    [string] $profilePath,
    [string] $sqkPkg
    )

	try
	{
		Write-Output "& "$sqlPkg" /action:DeployReport /TargetConnectionString:'Data Source='$dbServer';Initial Catalog='$sqlDb'' /SourceFile:$dacPacPath /profile:$profilePath /OutputPath:$outPutResultPath"
        & "$sqlPkg" /action:DeployReport /TargetConnectionString:'Data Source='$dbServer';Initial Catalog='$sqlDb'' /SourceFile:$dacPath /profile:$profilePath /OutputPath:$outPutResultPath
	}

	catch
	{
		Write-Error $_.Exception.Message
		exit 1
	}
}

try
{
    if(!(Test-Path -Path $outputPath ))
    {
        New-Item -ItemType directory -Path $outputPath
    }

    $date = (Get-Date).ToString("s").Replace(":","-") 
 
    $outPutResultPath = $outputPath + "\" + "DiffReport" + "_" + $date + ".xml"
    $sqlFile = $outputPath + "\" + $sqlDb + "-" + $date + "_schema.sql"

    # runSqlDiffReport $dbServer $sqlDb $dacPath $outPutResultPath $profilePath $sqPkg
    runSqlPublish $dbServer $sqlDb $sqlFile $dacPath $profilePath $permissionPath $sqlPkg -PublishDirectly
}

catch
{
    Write-Error $_.Exception.Message
    exit 1
}