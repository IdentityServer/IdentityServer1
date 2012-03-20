<#
.SYNOPSIS
   Deletes a user from the IdentityServer table storage
.DESCRIPTION
   Deletes a user from the IdentityServer table storage
.PARAMETER username
   Logon name of the user
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
	[string]$connectionString
)

begin {
	Add-Type -Path "Thinktecture.IdentityServer.WindowsAzure.dll"
}

process {
	try {
		$context = New-Object Thinktecture.IdentityServer.Repositories.WindowsAzure.TableStorageContext -arg $connectionString
		
		$context.DeleteUserAccount($username)				
	}
	catch {
		Write-Error $_.Exception.ToString()
	}
}