﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityServer.Repositories;
using Thinktecture.IdentityServer.Web.ActionResults;
using Thinktecture.IdentityServer.Web.Security;

namespace Thinktecture.IdentityServer.Web.Controllers
{
    public class SimpleHttpController : Controller
    {
        [Import]
        public IConfigurationRepository ConfigurationRepository { get; set; }

        public SimpleHttpController()
        {
            Container.Current.SatisfyImportsOnce(this);
        }

        public SimpleHttpController(IConfigurationRepository configurationRepository)
        {
            ConfigurationRepository = configurationRepository;
        }

        public ActionResult Issue(string realm, string tokenType)
        {
            Tracing.Verbose("Simple HTTP endpoint called.");

            if (!ConfigurationRepository.Endpoints.SimpleHttp)
            {
                Tracing.Warning("Simple HTTP endpoint is disabled in configuration");
                return new HttpNotFoundResult();
            }

            if (tokenType == null)
            {
                tokenType = ConfigurationRepository.Configuration.DefaultTokenType;
            }

            Tracing.Information("Token type: " + tokenType);

            Uri uri;
            if (!Uri.TryCreate(realm, UriKind.Absolute, out uri))
            {
                Tracing.Error("Realm parameter is malformed.");
                return new HttpStatusCodeResult(400);
            }

            Tracing.Information("Simple HTTP endpoint called for realm: " + uri.AbsoluteUri);

            var endpoint = new EndpointAddress(uri);
            var auth = new AuthenticationHelper();
            
            IClaimsPrincipal principal;
            if (!auth.TryGetPrincipalFromHttpRequest(Request, out principal))
            {
                Tracing.Error("no or invalid credentials found.");
                return new UnauthorizedResult("Basic",  UnauthorizedResult.ResponseAction.Send401);
            }

            if (!ClaimsAuthorize.CheckAccess(principal, Constants.Actions.Issue, Constants.Resources.SimpleHttp))
            {
                Tracing.Error("User not authorized");
                return new UnauthorizedResult("Basic", UnauthorizedResult.ResponseAction.Send401);
            }

            TokenResponse tokenResponse;
            if (auth.TryIssueToken(endpoint, principal, tokenType, out tokenResponse))
            {
                return new SimpleHttpResult(tokenResponse.TokenString, tokenType);
            }
            else
            {
                return new HttpStatusCodeResult(400);
            }
        }        
    }
}
