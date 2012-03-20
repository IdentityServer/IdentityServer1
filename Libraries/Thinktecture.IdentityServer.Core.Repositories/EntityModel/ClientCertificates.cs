/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.ComponentModel.DataAnnotations;
    
namespace Thinktecture.IdentityServer.Repositories.Sql
{
    public class ClientCertificates
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Thumbprint { get; set; }
        
        [Required]
        public string Description { get; set; }
    }
}
