/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System.ComponentModel.Composition.Hosting;

namespace Thinktecture.IdentityServer.Repositories
{
    public static class Container
    {
        public static CompositionContainer Current { get; set; }
    }
}