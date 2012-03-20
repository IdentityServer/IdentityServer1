/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using Thinktecture.IdentityServer.Models;

namespace Thinktecture.IdentityServer.Repositories
{
    public interface IConfigurationRepository
    {
        GlobalConfiguration Configuration { get; }
        EndpointConfiguration Endpoints { get; }
        CertificateConfiguration SslCertificate { get; }
        CertificateConfiguration SigningCertificate { get; }

        bool SupportsWriteAccess { get; }
        void UpdateConfiguration(GlobalConfiguration configuration);
        void UpdateEndpoints(EndpointConfiguration endpoints);
        void UpdateCertificates(string sslSubjectName, string signingSubjectName);
    }
}