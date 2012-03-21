﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Thinktecture.IdentityServer.Models;

namespace Thinktecture.IdentityServer.Web.ViewModels.Administration
{
    internal static class Extensions
    {
        static Extensions()
        {
            Mapper.CreateMap<EndpointConfiguration, EndpointConfigurationModel>();
            Mapper.CreateMap<EndpointConfigurationModel, EndpointConfiguration>();

            Mapper.CreateMap<GlobalConfigurationModel, GlobalConfiguration>();
            Mapper.CreateMap<GlobalConfiguration, GlobalConfigurationModel>();

            Mapper.AssertConfigurationIsValid();
        }

        #region RelyingParty
        public static RelyingPartyModel ToViewModel(this RelyingParty relyingParty)
        {
            var model = new RelyingPartyModel
            {
                Name = relyingParty.Name,
                Realm = relyingParty.Realm.AbsoluteUri,
                ExtraData1 = relyingParty.ExtraData1,
                ExtraData2 = relyingParty.ExtraData2,
                ExtraData3 = relyingParty.ExtraData3
            };

            if (relyingParty.EncryptingCertificate != null)
            {
                model.EncryptingCertificate = Convert.ToBase64String(relyingParty.EncryptingCertificate.RawData);
                model.EncryptingCertificateName = relyingParty.EncryptingCertificate.Subject;
            };

            if (relyingParty.ReplyTo != null)
            {
                model.ReplyTo = relyingParty.ReplyTo.AbsoluteUri;
            }

            if (relyingParty.SymmetricSigningKey != null && relyingParty.SymmetricSigningKey.Length != 0)
            {
                model.SymmetricSigningKey = Convert.ToBase64String(relyingParty.SymmetricSigningKey);
            }

            return model;
        }

        public static RelyingParty ToDomainModel(this RelyingPartyModel model)
        {
            var rp = new RelyingParty
            {
                Id = model.Id,
                Name = model.Name,
                Realm = new Uri(model.Realm),
                ExtraData1 = model.ExtraData1,
                ExtraData2 = model.ExtraData2,
                ExtraData3 = model.ExtraData3,
            };

            if (!string.IsNullOrWhiteSpace(model.ReplyTo))
            {
                rp.ReplyTo = new Uri(model.ReplyTo);
            }

            if (!string.IsNullOrWhiteSpace(model.EncryptingCertificate))
            {
                rp.EncryptingCertificate = new X509Certificate2(Convert.FromBase64String(model.EncryptingCertificate));
            }

            if (!string.IsNullOrWhiteSpace(model.SymmetricSigningKey))
            {
                rp.SymmetricSigningKey = Convert.FromBase64String(model.SymmetricSigningKey);
            }

            return rp;
        }

        public static RelyingPartiesModel ToViewModel(this IEnumerable<RelyingParty> relyingParties)
        {
            var model = new RelyingPartiesModel
            {
                RelyingParties =
                    (from rp in relyingParties
                     select new RelyingPartyModel
                     {
                         Id = rp.Id,
                         Name = rp.Name,
                         Realm = rp.Realm.AbsoluteUri
                     })
                    .ToList()
            };

            return model;
        }
        #endregion

        #region GlobalConfiguration
        public static GlobalConfigurationModel ToViewModel(this GlobalConfiguration configuration)
        {
            return Mapper.Map<GlobalConfiguration, GlobalConfigurationModel>(configuration);
        }

        public static GlobalConfiguration ToDomainModel(this GlobalConfigurationModel model)
        {
            return Mapper.Map<GlobalConfigurationModel, GlobalConfiguration>(model);
        }
        #endregion

        #region EndpointConfiguration
        public static EndpointConfigurationModel ToViewModel(this EndpointConfiguration endpoints)
        {
            return Mapper.Map<EndpointConfiguration, EndpointConfigurationModel>(endpoints);
        }

        public static EndpointConfiguration ToDomainModel(this EndpointConfigurationModel model)
        {
            return Mapper.Map<EndpointConfigurationModel, EndpointConfiguration>(model);
        }
        #endregion
    }
}