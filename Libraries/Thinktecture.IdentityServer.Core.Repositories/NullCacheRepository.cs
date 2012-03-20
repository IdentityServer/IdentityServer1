/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

namespace Thinktecture.IdentityServer.Repositories
{   
    public class NullCacheRepository : ICacheRepository
    {
        public void Put(string name, object value, int ttl)
        { }

        public object Get(string name)
        {
            return null;
        }

        public void Invalidate(string name)
        { }
    }
}
