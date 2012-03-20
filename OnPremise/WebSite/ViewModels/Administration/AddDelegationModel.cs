/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.ComponentModel.DataAnnotations;

namespace Thinktecture.IdentityServer.Web.ViewModels.Administration
{
    public class AddDelegationModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Url)]
        public string Realm { get; set; }

        [Required]
        public string Description { get; set; }
    }
}