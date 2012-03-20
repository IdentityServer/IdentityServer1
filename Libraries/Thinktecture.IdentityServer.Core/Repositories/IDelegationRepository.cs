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
    public interface IDelegationRepository
    {
        // run time
        bool IsDelegationAllowed(string userName, string realm);

        // management
        bool SupportsWriteAccess { get; }
        IEnumerable<string> GetAllUsers(int pageIndex, int pageSize);
        IEnumerable<DelegationSetting> GetDelegationSettingsForUser(string userName);
        void Add(DelegationSetting setting);
        void Delete(DelegationSetting setting);
    }
}
