﻿using System;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityServer.Core.Swt;
using Thinktecture.IdentityServer.Repositories;
using Thinktecture.IdentityServer.Web.ActionResults;
using Thinktecture.IdentityServer.Web.Security;

namespace Thinktecture.IdentityServer.Web.Controllers.Issue
{
    [ClaimsAuthorize(Constants.Actions.Issue, Constants.Resources.JSNotify)]
    public class JSNotifyController : Controller
    {
        [Import]
        public IConfigurationRepository ConfigurationRepository { get; set; }

        public JSNotifyController()
        {
            Container.Current.SatisfyImportsOnce(this);
        }

        public JSNotifyController(IConfigurationRepository configurationRepository)
        {
            ConfigurationRepository = configurationRepository;
        }

        public ActionResult Issue(string realm, string tokenType)
        {
            Tracing.Verbose("JSNotify endpoint called.");

            if (!ConfigurationRepository.Endpoints.JSNotify)
            {
                Tracing.Warning("JSNotify endpoint called, but disabled in configuration");
                return new HttpNotFoundResult();
            }

            Tracing.Information("JSNotify endpoint called for realm: " + realm);

            if (tokenType == null)
            {
                tokenType = SimpleWebToken.OasisTokenProfile;
            }

            Tracing.Information("Token type: " + tokenType);

            Uri uri;
            if (!Uri.TryCreate(realm, UriKind.Absolute, out uri))
            {
                Tracing.Error("Realm parameter is malformed.");
                return new HttpStatusCodeResult(400);
            }

            var endpoint = new EndpointAddress(uri);
            var auth = new AuthenticationHelper();

            TokenResponse response;
            if (auth.TryIssueToken(endpoint, Thread.CurrentPrincipal as IClaimsPrincipal, tokenType, out response))
            {
                var jsresponse = new AccessTokenResponse
                {
                    AccessToken = response.TokenString,
                    TokenType = response.TokenType,
                    ExpiresIn = ConfigurationRepository.Configuration.DefaultTokenLifetime * 60
                };

                Tracing.Information("JSNotify issue successful for user: " + User.Identity.Name);
                return new JSNotifyResult(jsresponse);
            }
            else
            {
                return new HttpStatusCodeResult(400);
            }
        }
    }
}
