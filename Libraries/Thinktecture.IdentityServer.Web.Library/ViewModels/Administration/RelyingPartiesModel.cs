/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;

namespace Thinktecture.IdentityServer.Web.ViewModels.Administration
{
    public class RelyingPartiesModel
    {
        public List<RelyingPartyModel> RelyingParties { get; set; }
    }
}