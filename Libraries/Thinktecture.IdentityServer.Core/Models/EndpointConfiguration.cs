﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */


namespace Thinktecture.IdentityServer.Models
{
    public class EndpointConfiguration
    {
        public bool WSFederation { get; set; }
        public bool WSTrustMessage { get; set; }
        public bool WSTrustMixed { get; set; }
        public bool SimpleHttp { get; set; }
        public bool FederationMetadata { get; set; }
        public bool OAuthWRAP { get; set; }
        public bool OAuth2 { get; set; }
        public bool JSNotify { get; set; }
        public int HttpPort { get; set; }
        public int HttpsPort { get; set; }
    }
}
