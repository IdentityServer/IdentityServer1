﻿using System.ComponentModel.DataAnnotations;

namespace Thinktecture.IdentityServer.Web.ViewModels.OAuth2
{
    public class ResourceOwnerCredentialRequest
    {
        [Required]
        public string Grant_Type { get; set; }

        [Required]
        public string Scope { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }       
    }
}