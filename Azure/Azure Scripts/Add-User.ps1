<#
.SYNOPSIS
   Adds a user to the IdentityServer table storage
.DESCRIPTION
   Adds a user to the IdentityServer table storage
.PARAMETER username
   Logon name of the user
.PARAMETER password
   password of the user
.PARAMETER isAdmin
   Specifies whether the user should be able to administrate IdentityServer
.PARAMETER connectionString
   Connection string to table storage
#>

[CmdletBinding()]
param(
	[Parameter(
		Mandatory=$True,
		ValueFromPipeline=$False,
		ValueFromPipelineByPropertyName=$True
	)]
	[string]$username,
	
	[Parameter(
		Mandatory=$True,
		ValueFromPipeline=$False,
		ValueFromPipelineByPropertyName=$True
	)]
	[ValidateLength(7,255)]
	[string]$password,
	
	[Parameter(
		Mandatory=$False,
		ValueFromPipeline=$False,
		ValueFromPipelineByPropertyName=$True
	)]
	[switch]$isAdmin = $false,
	
	[Parameter(
		Mandatory=$True,
		ValueFromPipeline=$False,
		ValueFromPipelineByPropertyName=$True
	)]
	[string]$connectionString
)

begin {
	Add-Type -Path "Thinktecture.IdentityServer.WindowsAzure.dll"
	
	try {
		[Thinktecture.IdentityServer.Repositories.WindowsAzure.TableStorageContext]::SetupTables($connectionString)
	}
	catch {
		Write-Error $_.Exception.ToString()
		exit
	}
}

process {
	try {
		$context = New-Object Thinktecture.IdentityServer.Repositories.WindowsAzure.TableStorageContext -arg $connectionString
		$context.AddUserAccount($username, $password, $isAdmin)
	}
	catch {
		Write-Error $_.Exception.ToString()
	}
}

