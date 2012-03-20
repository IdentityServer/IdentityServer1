Thinktecture IdentityServer Azure Edition

Please use the below steps to setup IdSrv.


Setup Instructions
__________________________________________

Prerequisites:

- Create a new Hosted Service in the Azure Portal
- Upload your SSL cert (and optionally a separate signing cert) using the portal
    - make note of the thumbprint


Configuration:

- Open ServiceConfiguration.cloud.cscfg
    - set "StorageConnectionString" to your storage account
    - set "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" to your storage account
    - set "SigningCertificateDistinguishedName" to the distinguished name of the signing certificate you uploaded earlier (you can also use the SSL certificate)
    - set the SSL certificate thumbprint

    - set the instance count to the number of instances you want to deploy

- Upload package
    - either via the portal ("New Production deployment") or the command line (using the SDK tools)

- Make sure deployment was successful


Create admin user

If you are using SQL Azure as your user store, you need to configure the connection string in configuration\connectionStrings.config, and the right repository
in configuration\repositories.config (see the comments in those files). Use any provider user frontend to create your users and roles.

If you are using table storage, you can use the below powershell commands to set up users.
Prereqs for running the scripts:
 - Azure SDK
 - WIF


- make sure you can execute scripts (you may need to use Set-ExecutionPolicy to adjust your local settings)
- go to the "Azure Scripts" directory using Powershell
- run ".\Enable-DotNet4Access -console" (as admin)

- use the Add-User script to add an initial administrator, e.g.:

    .\Add-User -username administrator -password somePassword -isAdmin -connectionString 'connection string to storage account'


- login with this account to make sure it works and that you have access to the administration area

- create other users (without the -isAdmin switch). You can also use Add-Claims script to add claims to those newly created users, e.g.:

    .\Add-User -username alice -password somePassword -connectionString 'connection string to storage account'

    .\Add-Claims -username alice -connectionString 'connection string to storageaccount' 
           -claims http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress%alice@thinktecture.com, 
                   http://schemas.microsoft.com/ws/2008/06/identity/claims/role%foo


Now IdSrv should be ready to use...

feedback to: dominick.baier@thinktecture.com


