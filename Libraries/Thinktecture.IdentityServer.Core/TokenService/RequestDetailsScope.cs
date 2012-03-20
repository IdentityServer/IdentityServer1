/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.SecurityTokenService;
using Thinktecture.IdentityServer.Core.Swt;

namespace Thinktecture.IdentityServer.TokenService
{
    /// <summary>
    /// Summary description for PolicyScope
    /// </summary>
    public class RequestDetailsScope : Scope
    {
        public RequestDetails RequestDetails { get; protected set; }

        public RequestDetailsScope(RequestDetails details, SigningCredentials signingCredentials, bool requireEncryption)
            : base(details.Realm.Uri.AbsoluteUri, signingCredentials)
        {
            RequestDetails = details;

            if (RequestDetails.UsesEncryption)
            {
                EncryptingCredentials = new X509EncryptingCredentials(details.EncryptingCertificate);
            }

            if (RequestDetails.TokenType == SimpleWebToken.OasisTokenProfile)
            {
                SigningCredentials = new SymmetricSigningCredentials(details.RelyingPartyRegistration.SymmetricSigningKey);
            }

            ReplyToAddress = RequestDetails.ReplyToAddress.AbsoluteUri;
            TokenEncryptionRequired = requireEncryption;
        }
    }
}