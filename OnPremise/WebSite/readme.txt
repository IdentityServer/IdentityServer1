Before installing and using IdSrv there are some system/environment prerequisites.

General
  - IIS 7.x with support for .NET 4.0, MVC3 is required (for development Visual Studio 2010 SP1).
  - IdSrv needs to be added to an IIS site with a working SSL configuration.
  - IdSrv needs an X509 certificate to sign outgoing security tokens. This can be the same as the SSL certificate. Make sure the application pool account has read access to the signature certificate's private key.
  - IdSrv requires SSL client certificates for certain pages. To be able to configure this inside IdSrv, configuration delegation for SSL must be enabled in IIS.
  - The IIS application pool account needs read access to the IdSrv web files and modify access to the App_Data sub-directory that contains the configuration database.

User Management
  - IdSrv uses the standard ASP.NET provider system by default. If you have an existing provider database you can adjust the connection string in configuration/connectionStrings.config.
  - You can also use the aspnet_regsql.exe tool from the .NET SDK to create a new provider database.
  - The IIS application pool account needs access to the provider database.
  - By default all users of IdSrv must be in the IdentityServerUsers role. This can be changed in Global Configuration.
  - All administrators of IdSrv must be in the 'IdentityServerAdministrators' role.

Walkthrough for setting up IdSrv
  - http://claudioasanchez.blogspot.com/2011/12/walk-though-of-provisioning-identity.html

Resources
  - Private Key Access
    http://msmvps.com/blogs/luisabreu/archive/2010/09/13/grant-access-to-certificate-s-private-key-in-iis-7-5.aspx
  - Setup SSL
    http://learn.iis.net/page.aspx/144/how-to-set-up-ssl-on-iis/
  - IIS Configuration Delegation
    http://learn.iis.net/page.aspx/159/configuring-remote-administration-and-feature-delegation-in-iis-7/



Changelog

post 1.0
    - Added a "Recycle IdSrv" link to the admin area
    - Added a "UseSqlServerForConfiguration" AppSetting to make migration to SQL Server easier.
	- Minor UI bug fixes

Version 1.0a
  - fixed a bug in federation metadata generation that broke fedutil.

Version 1.0
  - release