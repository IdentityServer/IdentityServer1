/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Web;
using System.Web.Mvc;
using WIF = Microsoft.IdentityModel;

namespace Thinktecture.IdentityServer.Web.ActionResults
{
    public class SimpleHttpResult : ContentResult
    {
        public SimpleHttpResult(string token, string tokenType)
        {
            Content = token;

            if (tokenType == WIF.Tokens.SecurityTokenTypes.OasisWssSaml11TokenProfile11 ||
                tokenType == WIF.Tokens.SecurityTokenTypes.Saml11TokenProfile11 ||
                tokenType == WIF.Tokens.SecurityTokenTypes.OasisWssSaml2TokenProfile11 ||
                tokenType == WIF.Tokens.SecurityTokenTypes.Saml2TokenProfile11)
            {
                ContentType = "text/xml";
            }
            else
            {
                ContentType = "text/plain";
            }
        }

        protected virtual void WriteToken(ControllerContext context)
        {
            var response = context.HttpContext.Response;

            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();

            base.ExecuteResult(context);
        }
    }
}