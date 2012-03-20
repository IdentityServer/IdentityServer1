﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * 
 * This code is licensed under the Microsoft Permissive License (Ms-PL)
 * 
 * SEE: http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
 * 
 */

using System;
using System.Diagnostics;

namespace Thinktecture.IdentityServer
{
    /// <summary>
    /// Helper class for Tracing
    /// </summary>
    public static class Tracing
    {
        [DebuggerStepThrough]
        public static void Start(string message)
        {
            TraceEvent(TraceEventType.Start, message, false);
        }

        [DebuggerStepThrough]
        public static void Stop(string message)
        {
            TraceEvent(TraceEventType.Stop, message, false);
        }

        [DebuggerStepThrough]
        public static void Information(string message)
        {
            TraceEvent(TraceEventType.Information, message, false);
        }

        [DebuggerStepThrough]
        public static void Warning(string message)
        {
            TraceEvent(TraceEventType.Warning, message, false);
        }

        [DebuggerStepThrough]
        public static void Error(string message)
        {
            TraceEvent(TraceEventType.Error, message, false);
        }

        [DebuggerStepThrough]
        public static void Verbose(string message)
        {
            TraceEvent(TraceEventType.Verbose, message, false);
        }

        [DebuggerStepThrough]
        public static void TraceEvent(TraceEventType type, string message, bool suppressTraceService)
        {
            TraceSource ts = new TraceSource("Thinktecture.IdentityServer");

            if (Trace.CorrelationManager.ActivityId == Guid.Empty)
            {
                if (type != TraceEventType.Verbose)
                {
                    Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                }
            }

            ts.TraceEvent(type, 0, message);
        }
    }
}