﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.ComponentModel.Composition;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Web;
using Thinktecture.IdentityServer.Repositories;
using Thinktecture.IdentityServer.TokenService;
using Thinktecture.IdentityServer.Web.Security;
using Thinktecture.IdentityServer.Web.ViewModels;

namespace Thinktecture.IdentityServer.Web.Controllers
{
    public class AccountController : Controller
    {
        [Import]
        public IUserRepository UserRepository { get; set; }

        [Import]
        public IConfigurationRepository ConfigurationRepository { get; set; }

        public AccountController()
        {
            Container.Current.SatisfyImportsOnce(this);
        }

        public AccountController(IUserRepository userRepository, IConfigurationRepository configurationRepository)
        {
            UserRepository = userRepository;
            ConfigurationRepository = configurationRepository;
        }
        
        public ActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ShowClientCertificateLink = ConfigurationRepository.Configuration.EnableClientCertificates;

            return View();
        }

        [HttpPost]
        public ActionResult SignIn(SignInModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (UserRepository.ValidateUser(model.UserName, model.Password))
                {
                    return SetPrincipalAndRedirect(model.UserName, AuthenticationMethods.Password, returnUrl, model.EnableSSO, ConfigurationRepository.Configuration.SsoCookieLifetime);
                }
            }

            ModelState.AddModelError("", "Incorrect credentials or no authorization.");

            ViewBag.ShowClientCertificateLink = ConfigurationRepository.Configuration.EnableClientCertificates;
            return View(model);
        }

        public ActionResult CertificateSignIn(string returnUrl)
        {
            if (!ConfigurationRepository.Configuration.EnableClientCertificates)
            {
                return new HttpNotFoundResult();
            }

            var clientCert = HttpContext.Request.ClientCertificate;

            if (clientCert != null && clientCert.IsPresent && clientCert.IsValid)
            {
                string userName;
                if (UserRepository.ValidateUser(new X509Certificate2(clientCert.Certificate), out userName))
                {
                    return SetPrincipalAndRedirect(userName, AuthenticationMethods.X509, returnUrl, false, ConfigurationRepository.Configuration.SsoCookieLifetime);
                }
            }

            return View("Error");
        }

        public ActionResult SignOut()
        {
            if (Request.IsAuthenticated)
            {
                FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            }

            return RedirectToAction("Index", "Home");
        }

        #region Private
        private ActionResult SetPrincipalAndRedirect(string userName, string authenticationMethod, string returnUrl, bool isPersistent, int ttl)
        {
            new AuthenticationHelper().SetSessionToken(userName, authenticationMethod, isPersistent, ttl, HttpContext.Request.Url.AbsoluteUri);

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = HttpUtility.UrlDecode(returnUrl);
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }

            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}