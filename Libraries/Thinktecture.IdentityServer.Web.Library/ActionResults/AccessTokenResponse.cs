/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Runtime.Serialization;

namespace Thinktecture.IdentityServer.Web.ActionResults
{
    [DataContract]
    public class AccessTokenResponse
    {
        [DataMember(Order = 1, Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Order = 2, Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Order = 3, Name = "expires_in")]
        public int ExpiresIn { get; set; }

        [DataMember(Order = 4, Name = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}