/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Thinktecture.IdentityServer.Models;

namespace Thinktecture.IdentityServer.Repositories
{
    public interface IClientCertificatesRepository
    {
        // run time
        bool TryGetUserNameFromThumbprint(X509Certificate2 certificate, out string userName);

        // management
        bool SupportsWriteAccess { get; }
        IEnumerable<string> List(int pageIndex, int pageSize);
        IEnumerable<ClientCertificate> GetClientCertificatesForUser(string userName);
        void Add(ClientCertificate certificate);
        void Delete(ClientCertificate certificate);
    }
}
