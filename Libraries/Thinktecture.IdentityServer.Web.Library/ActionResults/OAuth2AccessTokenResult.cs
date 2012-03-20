﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Mvc;

namespace Thinktecture.IdentityServer.Web.ActionResults
{
    public class OAuth2AccessTokenResult : ActionResult
    {
        AccessTokenResponse _response;

        public OAuth2AccessTokenResult(AccessTokenResponse response)
        {
            _response = response;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.HttpContext.Response.Cache.SetNoStore();
            context.HttpContext.Response.ContentType = "application/json";

            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "POST");
            context.HttpContext.Response.Headers.Add("Access-Control-Max-Age", "86400");
    
            var ser = new DataContractJsonSerializer(typeof(AccessTokenResponse));
            ser.WriteObject(context.HttpContext.Response.OutputStream, _response);
        }
    }
}