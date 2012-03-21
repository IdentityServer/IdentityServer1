/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Thinktecture.IdentityServer.Web.ViewModels
{
    public class InitialConfigurationModel
    {
        [DisplayName("Site name")]
        [Required]
        public string SiteName { get; set; }

        [DisplayName("Issuer URI")]
        [Required]
        public string IssuerUri { get; set; }

        [DisplayName("Signing Certificate")]
        [Required]
        public string SigningCertificate { get; set; }

        public List<string> AvailableCertificates { get; set; }

        public List<SelectListItem> AvailableCertificatesList
        {
            get
            {
                if (AvailableCertificates != null)
                {
                    return
                        (from c in AvailableCertificates
                         select new SelectListItem
                         {
                             Text = c,
                             Value = c
                         })
                        .ToList();
                }

                return null;
            }
        }
    }
}