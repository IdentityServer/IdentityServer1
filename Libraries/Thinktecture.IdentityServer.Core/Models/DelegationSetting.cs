/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;

namespace Thinktecture.IdentityServer.Models
{
    public class DelegationSetting
    {
        public string UserName { get; set; }
        public Uri Realm { get; set; }
        public string Description { get; set; }
    }
}
