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
using Thinktecture.IdentityServer.Models;

namespace Thinktecture.IdentityServer.Repositories.Sql
{
    internal static class Extensions
    {
        #region Relying Party
        public static RelyingParty ToDomainModel(this RelyingParties rpEntity)
        {
            var rp = new RelyingParty 
            {
                Id = rpEntity.Id.ToString(),
                Name = rpEntity.Name,
                Realm = new Uri("http://" + rpEntity.Realm),
                ExtraData1 = rpEntity.ExtraData1,
                ExtraData2 = rpEntity.ExtraData2,
                ExtraData3 = rpEntity.ExtraData3
            };

            if (!string.IsNullOrWhiteSpace(rpEntity.ReplyTo))
            {
                rp.ReplyTo = new Uri(rpEntity.ReplyTo);
            }

            if (!string.IsNullOrWhiteSpace(rpEntity.EncryptingCertificate))
            {
                rp.EncryptingCertificate = new X509Certificate2(Convert.FromBase64String(rpEntity.EncryptingCertificate));
            }

            if (!string.IsNullOrWhiteSpace(rpEntity.SymmetricSigningKey))
            {
                rp.SymmetricSigningKey = Convert.FromBase64String(rpEntity.SymmetricSigningKey);
            }

            return rp;
        }

        public static RelyingParties ToEntity(this RelyingParty relyingParty)
        {
            var rpEntity = new RelyingParties
            {
                Name = relyingParty.Name,
                Realm = relyingParty.Realm.AbsoluteUri.StripProtocolMoniker(),
                ExtraData1 = relyingParty.ExtraData1,
                ExtraData2 = relyingParty.ExtraData2,
                ExtraData3 = relyingParty.ExtraData3,
            };

            if (!string.IsNullOrEmpty(relyingParty.Id))
            {
                rpEntity.Id = int.Parse(relyingParty.Id);
            }

            if (relyingParty.ReplyTo != null)
            {
                rpEntity.ReplyTo = relyingParty.ReplyTo.AbsoluteUri;
            }

            if (relyingParty.EncryptingCertificate != null)
            {
                rpEntity.EncryptingCertificate = Convert.ToBase64String(relyingParty.EncryptingCertificate.RawData);
            }

            if (relyingParty.SymmetricSigningKey != null && relyingParty.SymmetricSigningKey.Length != 0)
            {
                rpEntity.SymmetricSigningKey = Convert.ToBase64String(relyingParty.SymmetricSigningKey);
            }

            return rpEntity;
        }

        public static IEnumerable<RelyingParty> ToDomainModel(this List<RelyingParties> relyingParties)
        {
            return
                (from rp in relyingParties
                 select new RelyingParty
                 {
                     Id = rp.Id.ToString(),
                     Name = rp.Name,
                     Realm = new Uri("http://" + rp.Realm)
                 }).ToList();
        }
        #endregion

        #region Client Certificates
        public static List<ClientCertificate> ToDomainModel(this List<ClientCertificates> entities)
        {
            return
                (from entity in entities
                 select new ClientCertificate
                 {
                     UserName = entity.UserName,
                     Thumbprint = entity.Thumbprint,
                     Description = entity.Description
                 }
                ).ToList();
        }
        #endregion

        #region Delegation
        public static List<DelegationSetting> ToDomainModel(this List<Delegation> entities)
        {
            return
                (from entity in entities
                 select new DelegationSetting
                 {
                     UserName = entity.UserName,
                     Realm = new Uri(entity.Realm),
                     Description = entity.Description
                 }
                ).ToList();
        }
        #endregion

        #region Misc
        public static string StripProtocolMoniker(this string uriString)
        {
            var uri = new Uri(uriString);
            string stripped = uri.AbsoluteUri.Substring(uri.Scheme.Length + 3);
            return stripped.ToLowerInvariant();
        }
        #endregion
    }
}
