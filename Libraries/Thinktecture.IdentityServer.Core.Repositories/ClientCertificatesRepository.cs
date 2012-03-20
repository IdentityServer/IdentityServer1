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
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ClientCertificatesRepository : IClientCertificatesRepository
    {
        #region Runtime
        public bool TryGetUserNameFromThumbprint(X509Certificate2 certificate, out string userName)
        {
            userName = null;

            using (var entities = IdentityServerConfigurationContext.Get())
            {
                userName = (from mapping in entities.ClientCertificates
                            where mapping.Thumbprint.Equals(certificate.Thumbprint, StringComparison.OrdinalIgnoreCase)
                            select mapping.UserName).FirstOrDefault();

                return (userName != null);
            }
        }
        #endregion

        #region Management
        public bool SupportsWriteAccess
        {
            get { return true; }
        }

        public IEnumerable<string> List(int pageIndex, int pageSize)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var users =
                    (from user in entities.ClientCertificates
                     orderby user.UserName
                     select user.UserName)
                    .Distinct();

                if (pageIndex != -1 && pageSize != -1)
                {
                    users = users.Skip(pageIndex * pageSize).Take(pageSize);
                }

                return users.ToList();
            }
        }

        public IEnumerable<ClientCertificate> GetClientCertificatesForUser(string userName)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var certs =
                     from record in entities.ClientCertificates
                     where record.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                     select record;

                return certs.ToList().ToDomainModel();
            }
        }

        public void Add(ClientCertificate certificate)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var entity = new ClientCertificates
                {
                    UserName = certificate.UserName,
                    Thumbprint = certificate.Thumbprint,
                    Description = certificate.Description
                };

                entities.ClientCertificates.Add(entity);
                entities.SaveChanges();
            }
        }

        public void Delete(ClientCertificate certificate)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var record =
                    (from entry in entities.ClientCertificates
                     where entry.UserName.Equals(certificate.UserName, StringComparison.OrdinalIgnoreCase) &&
                           entry.Thumbprint.Equals(certificate.Thumbprint, StringComparison.OrdinalIgnoreCase)
                     select entry)
                    .Single();

                entities.ClientCertificates.Remove(record);
                entities.SaveChanges();
            }
        }
        #endregion
    }
}
