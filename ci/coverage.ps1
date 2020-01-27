Param (
  [switch] $generateReport,
  [switch] $uploadCodecov
  )

$currentPath = Split-Path $MyInvocation.MyCommand.Path
$coverageOutputDirectory = Join-Path $currentPath "Debug"
$coverageFile = "coverage-results.xml"

Remove-Item $coverageOutputDirectory -Force -Recurse -ErrorAction SilentlyContinue
Remove-Item $coverageFile -ErrorAction SilentlyContinue
nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.519 OpenCover
$openCoverConsole = "packages\OpenCover.4.6.519\tools\OpenCover.Console.exe"

# OpenCover currently not supporting portable pdbs (https://github.com/OpenCover/opencover/issues/601)

$configuration = "Debug"
Get-ChildItem -Filter .\test\PalletPackerTests |
    ForEach-Object {
      $csprojPath = $_.FullName
      $testProjectName = $_.Name
# Project name being tested is unit test project name with UnitTests removed. "UnitTests" length == 9 
      $projectName = $testProjectName -replace ".{9}$"
        cmd.exe /c $openCoverConsole `
          -target:"c:\Program Files\dotnet\dotnet.exe" `
          -targetargs:"test -c $configuration $csprojPath\$testProjectName.csproj" `
          -mergeoutput `
          -hideskipped:File `
          -output:$coverageFile `
          -oldStyle `
          -filter:"+[$projectName]* -[$testProjectName]*" `
          -searchdirs:"$csprojPath\bin\$configuration\netcoreapp2.1\" `
          -register:user
    }

If ($generateReport) 
{
  nuget install -OutputDirectory packages -Version 4.4.5 ReportGenerator
  $reportGenerator = "packages\ReportGenerator.4.4.5\tools\ReportGenerator.exe"
  cmd.exe /c $reportGenerator `
    -reports:$coverageFile `
    -targetdir:$coverageOutputDirectory `
    -verbosity:Error
}

If ($uploadCodeCov) 
{
  nuget install -OutputDirectory packages -Version 1.9.0 Codecov
  $Codecov = "packages\Codecov.1.9.0\tools\Codecov.exe"
  cmd.exe /c $Codecov -f $coverageFile
}
