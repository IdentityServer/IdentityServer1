﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

namespace Thinktecture.IdentityServer.Configuration
{
    /// <summary>
    /// The StarterTokenServiceSection Configuration Section.
    /// </summary>
    public partial class RepositoryConfigurationSection : global::System.Configuration.ConfigurationSection
    {
        public const string SectionName = "thinktecture.identityServer.repositories";

        #region TokenServiceConfiguration Property

        /// <summary>
        /// The XML name of the <see cref="ConfigurationProvider"/> property.
        /// </summary>
        internal const global::System.String TokenServiceConfigurationPropertyName = "tokenServiceConfiguration";

        /// <summary>
        /// Gets or sets type of the class that provides encryption certificates
        /// </summary>
        [global::System.Configuration.ConfigurationProperty(TokenServiceConfigurationPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false, DefaultValue = "Thinktecture.IdentityServer.Repositories.SqlCompactConfigurationRepository, Thinktecture.IdentityServer")]
        public global::System.String TokenServiceConfiguration
        {
            get
            {
                return (global::System.String)base[TokenServiceConfigurationPropertyName];
            }
            set
            {
                base[TokenServiceConfigurationPropertyName] = value;
            }
        }

        #endregion

        #region UserManagement Property

        /// <summary>
        /// The XML name of the <see cref="ConfigurationProvider"/> property.
        /// </summary>
        internal const global::System.String UserManagementPropertyName = "userManagement";

        /// <summary>
        /// Gets or sets type of the class that provides encryption certificates
        /// </summary>
        [global::System.Configuration.ConfigurationProperty(UserManagementPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false, DefaultValue = "Thinktecture.IdentityServer.Repositories.ProviderUserRepository, Thinktecture.IdentityServer")]
        public global::System.String UserManagement
        {
            get
            {
                return (global::System.String)base[UserManagementPropertyName];
            }
            set
            {
                base[UserManagementPropertyName] = value;
            }
        }

        #endregion

        #region ClaimsRepository Property

        /// <summary>
        /// The XML name of the <see cref="ConfigurationProvider"/> property.
        /// </summary>
        internal const global::System.String ClaimsRepositoryPropertyName = "claimsRepository";

        [global::System.Configuration.ConfigurationProperty(ClaimsRepositoryPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false, DefaultValue = "Thinktecture.IdentityServer.Repositories.ProviderClaimsRepository, Thinktecture.IdentityServer")]
        public global::System.String ClaimsRepository
        {
            get
            {
                return (global::System.String)base[ClaimsRepositoryPropertyName];
            }
            set
            {
                base[ClaimsRepositoryPropertyName] = value;
            }
        }

        #endregion

        #region RelyingParties Property

        /// <summary>
        /// The XML name of the <see cref="ConfigurationProvider"/> property.
        /// </summary>
        internal const global::System.String RelyingPartiesPropertyName = "relyingParties";

        /// <summary>
        /// Gets or sets type of the class that provides encryption certificates
        /// </summary>
        [global::System.Configuration.ConfigurationProperty(RelyingPartiesPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false, DefaultValue = "Thinktecture.IdentityServer.Repositories.SqlCompactRelyingPartyRepository, Thinktecture.IdentityServer")]
        public global::System.String RelyingParties
        {
            get
            {
                return (global::System.String)base[RelyingPartiesPropertyName];
            }
            set
            {
                base[RelyingPartiesPropertyName] = value;
            }
        }

        #endregion

        #region ClientCertificates Property

        /// <summary>
        /// The XML name of the <see cref="ConfigurationProvider"/> property.
        /// </summary>
        internal const global::System.String ClientCertificatesPropertyName = "clientCertificates";

        /// <summary>
        /// Gets or sets type of the class that provides encryption certificates
        /// </summary>
        [global::System.Configuration.ConfigurationProperty(ClientCertificatesPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false, DefaultValue = "Thinktecture.IdentityServer.Repositories.SqlCompactUserMappingsRepository, Thinktecture.IdentityServer")]
        public global::System.String ClientCertificates
        {
            get
            {
                return (global::System.String)base[ClientCertificatesPropertyName];
            }
            set
            {
                base[ClientCertificatesPropertyName] = value;
            }
        }

        #endregion

        #region Delegation Property

        /// <summary>
        /// The XML name of the <see cref="ConfigurationProvider"/> property.
        /// </summary>
        internal const global::System.String DelegationPropertyName = "delegation";

        /// <summary>
        /// Gets or sets type of the class that provides encryption certificates
        /// </summary>
        [global::System.Configuration.ConfigurationProperty(DelegationPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false, DefaultValue = "Thinktecture.IdentityServer.Repositories.SqlCompactUserMappingsRepository, Thinktecture.IdentityServer")]
        public global::System.String Delegation
        {
            get
            {
                return (global::System.String)base[DelegationPropertyName];
            }
            set
            {
                base[DelegationPropertyName] = value;
            }
        }

        #endregion

        #region Caching Property

        /// <summary>
        /// The XML name of the <see cref="ConfigurationProvider"/> property.
        /// </summary>
        internal const global::System.String CachingPropertyName = "caching";

        /// <summary>
        /// Gets or sets type of the class that provides encryption certificates
        /// </summary>
        [global::System.Configuration.ConfigurationProperty(CachingPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false, DefaultValue = "Thinktecture.IdentityServer.Repositories.MemoryCacheRepository, Thinktecture.IdentityServer")]
        public global::System.String Caching
        {
            get
            {
                return (global::System.String)base[CachingPropertyName];
            }
            set
            {
                base[CachingPropertyName] = value;
            }
        }

        #endregion

        #region Xmlns Property

        /// <summary>
        /// The XML name of the <see cref="Xmlns"/> property.
        /// </summary>
        internal const global::System.String XmlnsPropertyName = "xmlns";

        /// <summary>
        /// Gets the XML namespace of this Configuration Section.
        /// </summary>
        /// <remarks>
        /// This property makes sure that if the configuration file contains the XML namespace,
        /// the parser doesn't throw an exception because it encounters the unknown "xmlns" attribute.
        /// </remarks>
        [global::System.Configuration.ConfigurationProperty(XmlnsPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false)]
        public global::System.String Xmlns
        {
            get
            {
                return (global::System.String)base[XmlnsPropertyName];
            }
        }

        #endregion
    }
}

