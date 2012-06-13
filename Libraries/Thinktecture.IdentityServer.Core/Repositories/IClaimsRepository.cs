/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityServer.TokenService;

namespace Thinktecture.IdentityServer.Repositories
{
    /// <summary>
    /// Repository for emitting claims into an outgoing token and claims metadata
    /// </summary>
    public interface IClaimsRepository
    {
        IEnumerable<Claim> GetClaims(IClaimsPrincipal principal, RequestDetails requestDetails);
        IEnumerable<string> GetSupportedClaimTypes();
    }
}
