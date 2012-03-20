/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Thinktecture.IdentityServer.Web.ViewModels.Administration
{
    public class AddClientCertificateModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Description { get; set; }

        public string Thumbprint { get; set; }
        public HttpPostedFileBase CertificateUpload { get; set; }
        
    }
}