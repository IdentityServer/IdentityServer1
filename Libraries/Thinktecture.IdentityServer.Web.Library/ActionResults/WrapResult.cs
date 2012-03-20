﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityServer.Web.Security;

namespace Thinktecture.IdentityServer.Web.ActionResults
{
    public class WrapResult : ActionResult
    {
        public TokenResponse TokenResponse { get; set; }

        protected string _contentType = "text/plain";
        protected string _content;

        public WrapResult()
        { }

        public WrapResult(TokenResponse response)
        {
            TokenResponse = response;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _content = "wrap_access_token=" + Uri.EscapeDataString(TokenResponse.TokenString);

            WriteToken(context);
        }

        protected virtual void WriteToken(ControllerContext context)
        {
            var response = context.HttpContext.Response;

            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();

            response.ContentType = _contentType;
            response.Write(_content);
        }
    }
}
