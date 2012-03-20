/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.Collections.Generic;
using Thinktecture.IdentityServer.Models;

namespace Thinktecture.IdentityServer.Repositories
{
    public interface IRelyingPartyRepository
    {
        bool TryGet(string realm, out RelyingParty relyingParty);

        // management
        bool SupportsWriteAccess { get; }
        IEnumerable<RelyingParty> List(int pageIndex, int pageSize);
        RelyingParty Get(string id);
        void Add(RelyingParty relyingParty);
        void Update(RelyingParty relyingParty);
        void Delete(string id);
    }
}
