param (
    [string]$ModuleName
)

if (-not $ModuleName) {
    Write-Error "Please specify a module name."
    exit
}

$SolutionName = "TeamOne.FrameUp"
$Namespace = "TeamOne.FrameUp"
$ModulePath = Join-Path -Path "." -ChildPath $ModuleName

# Function to create a project
function Add-Project {
    param (
        [string]$type,
        [string]$name,
        [string]$solutionDirectory
    )
    $fullProjectName = "$Namespace.$ModuleName.$name"
    $projectPath = Join-Path -Path $ModulePath -ChildPath $fullProjectName
    dotnet new $type -n $fullProjectName -o $projectPath
    
    $solutionFolder = Get-FinalPath -modulePath $ModulePath -directory $solutionDirectory
    dotnet sln $SolutionName.sln add $projectPath\$fullProjectName.csproj --solution-folder $solutionFolder
}

function Get-FinalPath {
    param (
        [string]$modulePath,
        [string]$directory
    )

    if (-not [string]::IsNullOrWhiteSpace($directory)) {
        return Join-Path -Path $modulePath -ChildPath $directory
    }
    else {
        return $modulePath
    }
}

# Function to add a project reference
function Add-ProjectReference {
    param (
        [string]$source,
        [string]$referenced
    )

    $fullSourceName = "$Namespace.$ModuleName.$source"
    $fullReferencedName = "$Namespace.$ModuleName.$referenced"
    
    $sourcePath = "$ModulePath\$fullSourceName\$fullSourceName.csproj"
    $referencedPath = "$ModulePath\$fullReferencedName\$fullReferencedName.csproj"

    dotnet add $sourcePath reference $referencedPath
}

# Create projects
Add-Project -type "webapi" -name "Web" -solutionDirectory "1 - Presentation"
Add-Project -type "nunit" -name "Web.Tests" -solutionDirectory "1 - Presentation"
Add-ProjectReference -source "Web.Tests" -referenced "Web"

Add-Project -type "classlib" -name "Application" -solutionDirectory "2 - Application"
Add-Project -type "nunit" -name "Application.Tests" -solutionDirectory "2 - Application"
Add-ProjectReference -source "Application.Tests" -referenced "Application"

Add-Project -type "classlib" -name "Domain" -solutionDirectory "3 - Domain"
Add-Project -type "nunit" -name "Domain.Tests" -solutionDirectory "3 - Domain"
Add-ProjectReference -source "Domain.Tests" -referenced "Domain"

Add-Project -type "classlib" -name "Infrastructure" -solutionDirectory "4 - Infrastructure"

Add-Project -type "classlib" -name "IoC"
Add-Project -type "nunit" -name "Behaviour.Tests"

# Add project references
Add-ProjectReference -source "Application" -referenced "Domain"
Add-ProjectReference -source "Infrastructure" -referenced "Domain"
Add-ProjectReference -source "Web" -referenced "Application"

Write-Host "Onion Architecture solution structure for $ModuleName created successfully."