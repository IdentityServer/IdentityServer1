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
using System.Globalization;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Protocols.WSFederation.Metadata;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.IdentityModel.SecurityTokenService;
using Thinktecture.IdentityServer.Core.Swt;
using Thinktecture.IdentityServer.Repositories;

namespace Thinktecture.IdentityServer.TokenService
{
    /// <summary>
    /// Handler for dynamic generation of the WS-Federation metadata document
    /// </summary>
    public class WSFederationMetadataGenerator
    {
        Endpoints _endpoints;

        [Import]
        public IConfigurationRepository ConfigurationRepository { get; set; }

        [Import]
        public IUserRepository UserRepository { get; set; }

        [Import]
        public IClaimsRepository ClaimsRepository { get; set; }

        public WSFederationMetadataGenerator(Endpoints endpoints)
        {
            _endpoints = endpoints;
            Container.Current.SatisfyImportsOnce(this);
        }

        public string Generate()
        {
            var tokenServiceDescriptor = GetTokenServiceDescriptor();
            var id = new EntityId(ConfigurationRepository.Configuration.IssuerUri);
            var entity = new EntityDescriptor(id);
            entity.SigningCredentials = new X509SigningCredentials(ConfigurationRepository.SigningCertificate.Certificate);
            entity.RoleDescriptors.Add(tokenServiceDescriptor);

            var ser = new MetadataSerializer();
            var sb = new StringBuilder(512);

            ser.WriteMetadata(XmlWriter.Create(new StringWriter(sb), new XmlWriterSettings { OmitXmlDeclaration = true }), entity);
            return sb.ToString();
        }

        private SecurityTokenServiceDescriptor GetTokenServiceDescriptor()
        {
            var tokenService = new SecurityTokenServiceDescriptor();
            tokenService.ServiceDescription = ConfigurationRepository.Configuration.SiteName;
            tokenService.Keys.Add(GetSigningKeyDescriptor());

            tokenService.PassiveRequestorEndpoints.Add(new EndpointAddress(_endpoints.WSFederation));

            tokenService.TokenTypesOffered.Add(new Uri(Microsoft.IdentityModel.Tokens.SecurityTokenTypes.OasisWssSaml11TokenProfile11));
            tokenService.TokenTypesOffered.Add(new Uri(Microsoft.IdentityModel.Tokens.SecurityTokenTypes.OasisWssSaml2TokenProfile11));
            tokenService.TokenTypesOffered.Add(new Uri(SimpleWebToken.OasisTokenProfile));

            ClaimsRepository.GetSupportedClaimTypes().ToList().ForEach(claimType => tokenService.ClaimTypesOffered.Add(new DisplayClaim(claimType)));
            tokenService.ProtocolsSupported.Add(new Uri("http://docs.oasis-open.org/wsfed/federation/200706"));

            if (ConfigurationRepository.Endpoints.WSTrustMessage)
            {
                var addressMessageUserName = new EndpointAddress(_endpoints.WSTrustMessageUserName, null, null, CreateMetadataReader(_endpoints.WSTrustMex), null);
                tokenService.SecurityTokenServiceEndpoints.Add(addressMessageUserName);

                if (ConfigurationRepository.Configuration.EnableClientCertificates)
                {
                    var addressMessageCertificate = new EndpointAddress(_endpoints.WSTrustMessageCertificate, null, null, CreateMetadataReader(_endpoints.WSTrustMex), null);
                    tokenService.SecurityTokenServiceEndpoints.Add(addressMessageCertificate);
                }
            }
            if (ConfigurationRepository.Endpoints.WSTrustMixed)
            {
                var addressMixedUserName = new EndpointAddress(_endpoints.WSTrustMixedUserName, null, null, CreateMetadataReader(_endpoints.WSTrustMex), null);
                tokenService.SecurityTokenServiceEndpoints.Add(addressMixedUserName);

                if (ConfigurationRepository.Configuration.EnableClientCertificates)
                {
                    var addressMixedCertificate = new EndpointAddress(_endpoints.WSTrustMixedCertificate, null, null, CreateMetadataReader(_endpoints.WSTrustMex), null);
                    tokenService.SecurityTokenServiceEndpoints.Add(addressMixedCertificate);
                }
            }

            if (tokenService.SecurityTokenServiceEndpoints.Count == 0)
                tokenService.SecurityTokenServiceEndpoints.Add(new EndpointAddress(_endpoints.WSFederation));

            return tokenService;
        }

        private KeyDescriptor GetSigningKeyDescriptor()
        {
            var certificate = ConfigurationRepository.SigningCertificate.Certificate;

            var clause = new X509SecurityToken(certificate).CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>();
            var key = new KeyDescriptor(new SecurityKeyIdentifier(clause));
            key.Use = KeyType.Signing;

            return key;
        }

        private XmlDictionaryReader CreateMetadataReader(Uri mexAddress)
        {
            var metadataSet = new MetadataSet();
            var metadataReference = new MetadataReference(new EndpointAddress(mexAddress), AddressingVersion.WSAddressing10);
            var metadataSection = new MetadataSection(MetadataSection.MetadataExchangeDialect, null, metadataReference);
            metadataSet.MetadataSections.Add(metadataSection);

            var sb = new StringBuilder();
            var w = new StringWriter(sb, CultureInfo.InvariantCulture);
            var writer = XmlWriter.Create(w);

            metadataSet.WriteTo(writer);
            writer.Flush();
            w.Flush();

            var input = new StringReader(sb.ToString());
            var reader = new XmlTextReader(input);
            return XmlDictionaryReader.CreateDictionaryReader(reader);
        }
    }
}