/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Web;

namespace Thinktecture.IdentityServer.TokenService
{
    public class X509CertificateSessionSecurityTokenHandler : SessionSecurityTokenHandler
    {
        public X509CertificateSessionSecurityTokenHandler(X509Certificate2 protectionCertificate)
            : base(CreateTransforms(protectionCertificate))
        { }

        private static ReadOnlyCollection<CookieTransform> CreateTransforms(X509Certificate2 protectionCertificate)
        {
            var transforms = new List<CookieTransform>() 
               { 
                 new DeflateCookieTransform(), 
                 new RsaEncryptionCookieTransform(protectionCertificate),
                 new RsaSignatureCookieTransform(protectionCertificate),
               };

            return transforms.AsReadOnly();
        }
    }
}
