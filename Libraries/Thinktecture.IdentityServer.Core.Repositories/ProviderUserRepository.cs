/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Profile;
using System.Web.Security;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityServer.TokenService;

namespace Thinktecture.IdentityServer.Repositories
{
    public class ProviderUserRepository : IUserRepository
    {
        private const string ProfileClaimPrefix = "http://identityserver.thinktecture.com/claims/profileclaims/";

        [Import]
        public IClientCertificatesRepository Repository { get; set; }

        public ProviderUserRepository()
        {
            Container.Current.SatisfyImportsOnce(this);
        }

        public bool ValidateUser(string userName, string password)
        {
            return Membership.ValidateUser(userName, password);
        }

        public bool ValidateUser(X509Certificate2 clientCertificate, out string userName)
        {
            return Repository.TryGetUserNameFromThumbprint(clientCertificate, out userName);
        }

        public IEnumerable<string> GetRoles(string userName, RoleTypes roleType)
        {
            var returnedRoles = new List<string>();

            if (Roles.Enabled)
            {
                var roles = Roles.GetRolesForUser(userName);

                if (roleType == RoleTypes.All)
                {
                    returnedRoles = roles.ToList();
                }
                else if (roleType == RoleTypes.IdentityServer)
                {
                    returnedRoles = roles.Where(role => role.StartsWith(Constants.Roles.InternalRolesPrefix)).ToList();
                }
                else if (roleType == RoleTypes.Client)
                {
                    returnedRoles = roles.Where(role => !(role.StartsWith(Constants.Roles.InternalRolesPrefix))).ToList();
                }
            }

            return returnedRoles;
        }

        public IEnumerable<Claim> GetClaims(IClaimsPrincipal principal, RequestDetails requestDetails)
        {
            var userName = principal.Identity.Name;
            var claims = new List<Claim>();

            // email address
            string email = Membership.FindUsersByName(userName)[userName].Email;
            if (!String.IsNullOrEmpty(email))
            {
                claims.Add(new Claim(ClaimTypes.Email, email));
            }

            // roles
            GetRoles(userName, RoleTypes.Client).ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

            // profile claims
            if (ProfileManager.Enabled)
            {
                var profile = ProfileBase.Create(userName, true);
                if (profile != null)
                {
                    foreach (SettingsProperty prop in ProfileBase.Properties)
                    {
                        string value = profile.GetPropertyValue(prop.Name).ToString();
                        if (!String.IsNullOrWhiteSpace(value))
                        {
                            claims.Add(new Claim(ProfileClaimPrefix + prop.Name.ToLowerInvariant(), value));
                        }
                    }
                }
            }

            return claims;
        }

        public IEnumerable<string> GetSupportedClaimTypes()
        {
            var claimTypes = new List<string>
            {
                ClaimTypes.Name,
                ClaimTypes.Email,
                ClaimTypes.Role
            };

            if (ProfileManager.Enabled)
            {
                foreach (SettingsProperty prop in ProfileBase.Properties)
                {
                    claimTypes.Add(ProfileClaimPrefix + prop.Name.ToLowerInvariant());
                }
            }

            return claimTypes;
        }
    }
}