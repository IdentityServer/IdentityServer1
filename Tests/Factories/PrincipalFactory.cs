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
using Microsoft.IdentityModel.Claims;

namespace Thinktecture.IdentityServer.Tests
{
    internal static class PrincipalFactory
    {
        public static IClaimsPrincipal Create(string type)
        {
            switch (type)
            {
                case Constants.Principals.AliceUserName:
                    return Create("Alice", AuthenticationMethods.Password);
                case Constants.Principals.Anonymous:
                    return CreateAnonymous();
                default:
                    throw new ArgumentException("type");
            }
        }

        public static IClaimsPrincipal Create(string userName, string authenticationMethod)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod)
            };

            return ClaimsPrincipal.CreateFromIdentity(new ClaimsIdentity(claims));
        }

        public static IClaimsPrincipal CreateAnonymous()
        {
            return ClaimsPrincipal.AnonymousPrincipal;
        }
    }
}
