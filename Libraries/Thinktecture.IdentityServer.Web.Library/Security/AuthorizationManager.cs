/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Thinktecture.IdentityModel.Extensions;
using Thinktecture.IdentityServer.Repositories;

namespace Thinktecture.IdentityServer.Web.Security
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        [Import]
        public IConfigurationRepository ConfigurationRepository { get; set; }

        public AuthorizationManager()
        {
            Container.Current.SatisfyImportsOnce(this);
        }

        public AuthorizationManager(IConfigurationRepository configuration)
        {
            ConfigurationRepository = configuration;
        }

        public override bool CheckAccess(AuthorizationContext context)
        {
            var action = context.Action.First();
            var id = context.Principal.Identities.First();

            // if application authorization request
            if (action.ClaimType.Equals(ClaimsAuthorize.ActionType))
            {
                return AuthorizeCore(action, context.Resource, context.Principal.Identity as IClaimsIdentity);
            }

            // if ws-trust issue request
            if (action.Value.Equals(WSTrust13Constants.Actions.Issue))
            {
                return AuthorizeTokenIssuance(new Collection<Claim> { new Claim(ClaimsAuthorize.ResourceType, Constants.Resources.WSTrust) }, id);
            }

            return base.CheckAccess(context);
        }

        protected virtual bool AuthorizeCore(Claim action, Collection<Claim> resource, IClaimsIdentity id)
        {
            switch (action.Value)
            {
                case Constants.Actions.Issue:
                    return AuthorizeTokenIssuance(resource, id);
                case Constants.Actions.Administration:
                    return AuthorizeAdministration(resource, id);
            }

            return false;
        }

        protected virtual bool AuthorizeTokenIssuance(Collection<Claim> resource, IClaimsIdentity id)
        {
            if (!ConfigurationRepository.Configuration.EnforceUsersGroupMembership)
            {
                var authResult = id.IsAuthenticated;
                if (!authResult)
                {
                    Tracing.Error("Authorization for token issuance failed because the user is anonymous");
                }

                return authResult;
            }

            var roleResult = id.ClaimExists(ClaimTypes.Role, Constants.Roles.IdentityServerUsers);
            if (!roleResult)
            {
                Tracing.Error(string.Format("Authorization for token issuance failed because user {0} is not in the {1} role", id.Name, Constants.Roles.IdentityServerUsers));
            }

            return roleResult;
        }

        protected virtual bool AuthorizeAdministration(Collection<Claim> resource, IClaimsIdentity id)
        {
            var roleResult = id.ClaimExists(ClaimTypes.Role, Constants.Roles.IdentityServerAdministrators);
            if (!roleResult)
            {
                if (resource[0].Value != Constants.Resources.UI)
                {
                    Tracing.Error(string.Format("Administration authorization failed because user {0} is not in the {1} role", id.Name, Constants.Roles.IdentityServerAdministrators));
                }
            }

            return roleResult;
        }
    }
}