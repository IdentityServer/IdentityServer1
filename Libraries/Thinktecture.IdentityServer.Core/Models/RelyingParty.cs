﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Security.Cryptography.X509Certificates;

namespace Thinktecture.IdentityServer.Models
{
    public class RelyingParty
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Uri Realm { get; set; }
        public Uri ReplyTo { get; set; }
        public X509Certificate2 EncryptingCertificate { get; set; }
        public byte[] SymmetricSigningKey { get; set; }
        public string ExtraData1 { get; set; }
        public string ExtraData2 { get; set; }
        public string ExtraData3 { get; set; }
    }
}