/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityModel.Claims;
using WIF = Microsoft.IdentityModel.Claims;

namespace Thinktecture.IdentityServer.Web.Security
{
    public class ClaimsAuthorize : AuthorizeAttribute
    {
        private string _resource;
        private string _action;
        private string[] _additionalResources;

        /// <summary>
        /// Default action claim type.
        /// </summary>
        public const string ActionType = "http://application/claims/authorization/action";

        /// <summary>
        /// Default resource claim type
        /// </summary>
        public const string ResourceType = "http://application/claims/authorization/resource";

        /// <summary>
        /// Additional resource claim type
        /// </summary>
        public const string AdditionalResourceType = "http://application/claims/authorization/additionalresource";

        public ClaimsAuthorize(string action, string resource, params string[] additionalResources)
        {
            _action = action; 
            _resource = resource;
            _additionalResources = additionalResources;
        }

        public static bool CheckAccess(string action, string resource, params string[] additionalResources)
        {
            return CheckAccess(
                Thread.CurrentPrincipal as IClaimsPrincipal,
                action,
                resource,
                additionalResources);
        }

        public static bool CheckAccess(IClaimsPrincipal principal, string action, string resource, params string[] additionalResources)
        {
            var context = CreateAuthorizationContext(
                principal,
                action,
                resource,
                additionalResources);

            return ClaimsAuthorization.CheckAccess(context);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return CheckAccess(_action, _resource, _additionalResources);
        }

        private static WIF.AuthorizationContext CreateAuthorizationContext(IClaimsPrincipal principal, string action, string resource, params string[] additionalResources)
        {
            var actionClaims = new Collection<Claim>
            {
                new Claim(ActionType, action)
            };

            var resourceClaims = new Collection<Claim>
            {
                new Claim(ResourceType, resource)
            };

            if (additionalResources != null && additionalResources.Length > 0)
            {
                additionalResources.ToList().ForEach(ar => resourceClaims.Add(new Claim(AdditionalResourceType, ar)));
            }

            return new WIF.AuthorizationContext(
                principal,
                resourceClaims,
                actionClaims);
        }
    }
}