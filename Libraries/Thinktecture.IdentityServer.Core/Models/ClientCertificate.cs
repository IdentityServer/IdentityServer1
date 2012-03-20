/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

namespace Thinktecture.IdentityServer.Models
{
    public class ClientCertificate
    {
        public string UserName { get; set; }
        public string Thumbprint { get; set; }
        public string Description { get; set; }
    }
}
