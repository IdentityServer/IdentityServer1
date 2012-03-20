﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thinktecture.IdentityServer.Core.Swt;

namespace Thinktecture.IdentityServer.Tests
{
    [TestClass]
    public class WSTrust
    {
        IWSTrustChannelContract _mixedUserNameClient;
        IWSTrustChannelContract _mixedCertificateClient;

        RequestSecurityToken _rst;

        string baseAddressUserName = Constants.WSTrust.LocalMixedUserName;
        //string baseAddressUserName = Constants.WSTrust.CloudMixedUserName;
        
        string baseAddressCertificate = Constants.WSTrust.LocalMixedCertificate;

        string rp = Constants.Realms.LocalRP;

        [TestInitialize]
        public void Setup()
        {
            _mixedUserNameClient = WSTrustClientFactory.CreateMixedUserNameClient(
                Constants.Credentials.ValidUserName, 
                Constants.Credentials.ValidPassword, 
                baseAddressUserName);

            _mixedCertificateClient = WSTrustClientFactory.CreateMixedCertificateClient(
                Constants.Certificates.ValidClientCertificateName,
                baseAddressCertificate);

            _rst = new RequestSecurityToken
            {
                AppliesTo = new EndpointAddress(rp),
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Symmetric
            };

        }

        [TestMethod]
        public void ValidUserNameCredential()
        {
            RequestSecurityTokenResponse rstr;
            var token = _mixedUserNameClient.Issue(_rst, out rstr);

            Assert.IsTrue(token != null);
        }

        [TestMethod]
        public void ValidClientCertificateCredential()
        {
            RequestSecurityTokenResponse rstr;
            var token = _mixedCertificateClient.Issue(_rst, out rstr);

            Assert.IsTrue(token != null);
        }

        [TestMethod]
        public void ValidUserNameCredentialSaml11()
        {
            var rst = new RequestSecurityToken
            {
                AppliesTo = new EndpointAddress(rp),
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Symmetric,

                TokenType = Microsoft.IdentityModel.Tokens.SecurityTokenTypes.Saml11TokenProfile11
            };

            RequestSecurityTokenResponse rstr;
            var token = _mixedUserNameClient.Issue(rst, out rstr) as GenericXmlSecurityToken;

            Assert.IsTrue(token != null);
            Assert.IsTrue(token.ProofToken != null);
            Assert.AreEqual(Microsoft.IdentityModel.Tokens.SecurityTokenTypes.Saml11TokenProfile11, rstr.TokenType);
        }

        [TestMethod]
        public void ValidUserNameCredentialSaml11Bearer()
        {
            var rst = new RequestSecurityToken
            {
                AppliesTo = new EndpointAddress(rp),
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Bearer,

                TokenType = Microsoft.IdentityModel.Tokens.SecurityTokenTypes.Saml11TokenProfile11
            };

            RequestSecurityTokenResponse rstr;
            var token = _mixedUserNameClient.Issue(rst, out rstr) as GenericXmlSecurityToken;

            Assert.IsTrue(token != null);
            Assert.IsTrue(token.ProofToken == null);
            Assert.AreEqual(Microsoft.IdentityModel.Tokens.SecurityTokenTypes.Saml11TokenProfile11, rstr.TokenType);
        }

        [TestMethod]
        public void ValidUserNameCredentialSaml2Bearer()
        {
            var rst = new RequestSecurityToken
            {
                AppliesTo = new EndpointAddress(rp),
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Bearer,

                TokenType = Microsoft.IdentityModel.Tokens.SecurityTokenTypes.Saml2TokenProfile11
            };

            RequestSecurityTokenResponse rstr;
            var token = _mixedUserNameClient.Issue(rst, out rstr) as GenericXmlSecurityToken;

            Assert.IsTrue(token != null);
            Assert.IsTrue(token.ProofToken == null);
            Assert.AreEqual(Microsoft.IdentityModel.Tokens.SecurityTokenTypes.Saml2TokenProfile11, rstr.TokenType);
        }

        [TestMethod]
        public void ValidUserNameCredentialSaml2()
        {
            var rst = new RequestSecurityToken
            {
                AppliesTo = new EndpointAddress(rp),
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Symmetric,

                TokenType = Microsoft.IdentityModel.Tokens.SecurityTokenTypes.Saml2TokenProfile11
            };

            RequestSecurityTokenResponse rstr;
            var token = _mixedUserNameClient.Issue(rst, out rstr) as GenericXmlSecurityToken;

            Assert.IsTrue(token != null);
            Assert.IsTrue(token.ProofToken != null);
            Assert.AreEqual(Microsoft.IdentityModel.Tokens.SecurityTokenTypes.Saml2TokenProfile11, rstr.TokenType);
        }

        [TestMethod]
        public void ValidUserNameCredentialSwtSymmetric()
        {
            var rst = new RequestSecurityToken
            {
                AppliesTo = new EndpointAddress(rp),
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Symmetric,

                TokenType = SimpleWebToken.OasisTokenProfile
            };

            RequestSecurityTokenResponse rstr;
            var token = _mixedUserNameClient.Issue(rst, out rstr) as GenericXmlSecurityToken; 

            Assert.IsTrue(token != null);
            Assert.IsTrue(token.ProofToken != null);
            Assert.AreEqual(SimpleWebToken.OasisTokenProfile, rstr.TokenType);
        }

        [TestMethod]
        public void ValidUserNameCredentialSwtBearer()
        {
            var rst = new RequestSecurityToken
            {
                AppliesTo = new EndpointAddress(rp),
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Bearer,

                TokenType = SimpleWebToken.OasisTokenProfile
            };

            RequestSecurityTokenResponse rstr;
            var token = _mixedUserNameClient.Issue(rst, out rstr) as GenericXmlSecurityToken;

            Assert.IsTrue(token != null);
            Assert.IsTrue(token.ProofToken == null);
            Assert.AreEqual(SimpleWebToken.OasisTokenProfile, rstr.TokenType);
        }


        [TestMethod]
        [ExpectedException(typeof(MessageSecurityException))]
        public void InvalidUserNameCredential()
        {
            var client = WSTrustClientFactory.CreateMixedUserNameClient(
                Constants.Credentials.ValidUserName,
                "invalid",
                baseAddressUserName);

            RequestSecurityTokenResponse rstr;
            var token = client.Issue(_rst, out rstr);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void UnauthorizedUser()
        {
            var client = WSTrustClientFactory.CreateMixedUserNameClient(
                Constants.Credentials.UnauthorizedUserName,
                Constants.Credentials.ValidPassword,
                baseAddressUserName);

            RequestSecurityTokenResponse rstr;
            var token = client.Issue(_rst, out rstr);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException<ExceptionDetail>))]
        public void NoRealm()
        {
            var rst = new RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Symmetric
            };

            RequestSecurityTokenResponse rstr;
            var token = _mixedUserNameClient.Issue(rst, out rstr);
        }
    }
}
