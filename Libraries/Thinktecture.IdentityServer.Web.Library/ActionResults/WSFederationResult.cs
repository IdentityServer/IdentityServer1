/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Web.Mvc;
using Microsoft.IdentityModel.Protocols.WSFederation;

namespace Thinktecture.IdentityServer.Web.ActionResults
{
    public class WSFederationResult : ContentResult
    {
        public WSFederationResult(SignInResponseMessage message)
        {
            Content = message.WriteFormPost();
        }
    }
}