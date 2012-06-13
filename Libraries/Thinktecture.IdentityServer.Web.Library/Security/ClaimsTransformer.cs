/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityServer.Repositories;

namespace Thinktecture.IdentityServer.Web.Security
{
    public class ClaimsTransformer : ClaimsAuthenticationManager
    {
        [Import]
        public IUserRepository UserRepository { get; set; }

        public ClaimsTransformer()
        {
            Container.Current.SatisfyImportsOnce(this);
        }

        public override IClaimsPrincipal Authenticate(string resourceName, IClaimsPrincipal incomingPrincipal)
        {
            if (!incomingPrincipal.Identity.IsAuthenticated)
            {
                return base.Authenticate(resourceName, incomingPrincipal);
            }

            UserRepository.GetRoles(incomingPrincipal.Identity.Name).ToList().ForEach(role =>
                incomingPrincipal.Identities[0].Claims.Add(new Claim(ClaimTypes.Role, role)));

            return incomingPrincipal;
        }
    }
}