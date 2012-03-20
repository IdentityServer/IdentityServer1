﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using Microsoft.IdentityModel.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thinktecture.IdentityServer.Models;
using Thinktecture.IdentityServer.TokenService;

namespace Thinktecture.IdentityServer.Tests
{
    [TestClass]
    public class ReplyToHandlingTest
    {
        GlobalConfiguration config;
        Request request;
        IClaimsPrincipal _alice;

        [TestInitialize]
        public void Setup()
        {
            config = ConfigurationFactory.Create(Constants.ConfigurationModes.LockedDownAllowReplyTo);
            request = new Request(config, new TestRelyingPartyRepository(), null);
            _alice = PrincipalFactory.Create(Constants.Principals.AliceUserName);
        }

        [TestMethod]
        public void IgnoreReplyToForRegisteredRPwithReplyTo()
        {
            var rst = RstFactory.Create(Constants.Realms.ExplicitReplyTo);
            rst.ReplyTo = "http://foo";
            var details = request.Analyze(rst, _alice);

            // make sure reply to is from configuration
            Assert.IsTrue(details.IsReplyToFromConfiguration);

            // reply to 
            Assert.IsTrue(details.ReplyToAddressIsWithinRealm);
            Assert.AreEqual(rst.AppliesTo.Uri.AbsoluteUri, details.ReplyToAddress.AbsoluteUri);
        }

        [TestMethod]
        public void HonourReplyToForRegisteredRPwithoutReplyTo()
        {
            var rst = RstFactory.Create(Constants.Realms.SslEncryption);
            rst.ReplyTo = Constants.Realms.SslEncryption + "subrealm/";
            var details = request.Analyze(rst, _alice);

            // make sure reply to is from configuration
            Assert.IsFalse(details.IsReplyToFromConfiguration);

            // reply to 
            Assert.IsTrue(details.ReplyToAddressIsWithinRealm);
            Assert.AreEqual(rst.ReplyTo, details.ReplyToAddress.AbsoluteUri);
        }

        [TestMethod]
        public void DetectCrossRealmRedirect()
        {
            var rst = RstFactory.Create(Constants.Realms.SslEncryption);
            rst.ReplyTo = "http://foo/";
            var details = request.Analyze(rst, _alice);

            // make sure reply to is from configuration
            Assert.IsFalse(details.IsReplyToFromConfiguration);

            // reply to 
            Assert.IsFalse(details.ReplyToAddressIsWithinRealm);
            Assert.AreEqual(rst.ReplyTo, details.ReplyToAddress.AbsoluteUri);
        }
    }
}
