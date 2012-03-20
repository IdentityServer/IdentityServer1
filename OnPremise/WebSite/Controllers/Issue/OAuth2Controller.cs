using System;
using System.ComponentModel.Composition;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityServer.Core.Swt;
using Thinktecture.IdentityServer.Repositories;
using Thinktecture.IdentityServer.Web.ActionResults;
using Thinktecture.IdentityServer.Web.Security;
using Thinktecture.IdentityServer.Web.ViewModels.OAuth2;

namespace Thinktecture.IdentityServer.Web.Controllers
{
    public class OAuth2Controller : Controller
    {
        [Import]
        public IUserRepository UserRepository { get; set; }

        [Import]
        public IConfigurationRepository ConfigurationRepository { get; set; }

        public OAuth2Controller()
        {
            Container.Current.SatisfyImportsOnce(this);
        }

        public OAuth2Controller(IUserRepository userRepository, IConfigurationRepository configurationRepository)
        {
            UserRepository = userRepository;
            ConfigurationRepository = configurationRepository;
        }

        [HttpPost]
        public ActionResult Token(ResourceOwnerCredentialRequest request)
        {
            Tracing.Verbose("OAuth2 endpoint called.");

            if (!ConfigurationRepository.Endpoints.OAuth2)
            {
                Tracing.Error("OAuth2 endpoint called, but disabled in configuration");
                return new HttpNotFoundResult();
            }

            if (!ModelState.IsValid)
            {
                Tracing.Error("OAuth2 called with malformed request");
                return new HttpStatusCodeResult(400);
            }

            var auth = new AuthenticationHelper();

            Uri uri;
            if (!Uri.TryCreate(request.Scope, UriKind.Absolute, out uri))
            {
                Tracing.Error("OAuth2 endpoint called with malformed realm: " + request.Scope);
                return new HttpStatusCodeResult(400);
            }

            IClaimsPrincipal principal = null;
            if (auth.TryGetPrincipalFromOAuth2Request(Request, request, out principal))
            {
                if (!ClaimsAuthorize.CheckAccess(principal, Constants.Actions.Issue, Constants.Resources.OAuth2))
                {
                    Tracing.Error("User not authorized");
                    return new UnauthorizedResult("OAuth2", UnauthorizedResult.ResponseAction.Send401);
                }

                SecurityToken token;
                if (auth.TryIssueToken(new EndpointAddress(uri), principal, SimpleWebToken.OasisTokenProfile, out token))
                {
                    var swt = token as SimpleWebToken;
                    var response = new AccessTokenResponse
                    {
                        AccessToken = swt.RawToken,
                        TokenType = SimpleWebToken.OasisTokenProfile,
                        ExpiresIn = ConfigurationRepository.Configuration.DefaultTokenLifetime * 60,
                    };

                    Tracing.Information("OAuth2 issue successful for user: " + request.UserName);
                    return new OAuth2AccessTokenResult(response);
                }

                return new HttpStatusCodeResult(400);
            }

            Tracing.Error("OAuth2 endpoint authentication failed for user: " + request.UserName);
            return new UnauthorizedResult("OAuth2", UnauthorizedResult.ResponseAction.Send401);

            //if (UserRepository.ValidateUser(request.UserName ?? "", request.Password ?? ""))
            //{
            //    var principal = auth.CreatePrincipal(request.UserName, AuthenticationMethods.Password);

            //    if (!ClaimsAuthorize.CheckAccess(principal, Constants.Actions.Issue, Constants.Resources.OAuth2))
            //    {
            //        Tracing.Error("User not authorized");
            //        return new UnauthorizedResult("OAuth2", UnauthorizedResult.ResponseAction.Send401);
            //    }

            //    SecurityToken token;
            //    if (auth.TryIssueToken(new EndpointAddress(uri), principal, SimpleWebToken.OasisTokenProfile, out token))
            //    {
            //        var swt = token as SimpleWebToken;
            //        var response = new AccessTokenResponse
            //        {
            //            AccessToken = swt.RawToken,
            //            TokenType = SimpleWebToken.OasisTokenProfile,
            //            ExpiresIn = ConfigurationRepository.Configuration.DefaultTokenLifetime * 60,
            //        };

            //        Tracing.Information("OAuth2 issue successful for user: " + request.UserName);
            //        return new OAuth2AccessTokenResult(response);
            //    }

            //    return new HttpStatusCodeResult(400);
            //}

            //Tracing.Error("OAuth2 endpoint authentication failed for user: " + request.UserName);
            //return new UnauthorizedResult("OAuth2", UnauthorizedResult.ResponseAction.Send401);
        }
    }
}
