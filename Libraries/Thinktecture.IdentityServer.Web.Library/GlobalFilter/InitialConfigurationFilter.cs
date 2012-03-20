/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Mvc;
using System.Web.Routing;
using Thinktecture.IdentityServer.Repositories;

namespace Thinktecture.IdentityServer.Web.GlobalFilter
{
    public class InitialConfigurationFilter : ActionFilterAttribute
    {
        [Import]
        public IConfigurationRepository ConfigurationRepository { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Container.Current.SatisfyImportsOnce(this);

            if (!filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("InitialConfiguration"))
            {
                if (string.IsNullOrWhiteSpace(ConfigurationRepository.SigningCertificate.SubjectDistinguishedName))
                {
                    var route = new RouteValueDictionary(new Dictionary<string, object>
                        {
                            { "Controller", "InitialConfiguration" },
                        });

                    filterContext.Result = new RedirectToRouteResult(route);
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}