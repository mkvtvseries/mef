﻿// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.Discovery;
using System.ComponentModel.Composition.Lightweight.Util;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Debugging.Proxies
{
    class ContainerConfigurationDebuggerProxy
    {
        ContainerConfiguration _configuration;
        DiscoveredPart[] _discoveredParts;
        Type[] _ignoredTypes;

        public ContainerConfigurationDebuggerProxy(ContainerConfiguration configuration)
        {
            _configuration = configuration;
        }

        [DebuggerDisplay("Added Providers")]
        public ExportDescriptorProvider[] AddedExportDescriptorProviders
        {
            get { return _configuration.DebugGetAddedExportDescriptorProviders(); }
        }

        [DebuggerDisplay("Discovered Parts")]
        public DiscoveredPart[] DiscoveredParts
        {
            get
            {
                InitDiscovery();
                return _discoveredParts;
            }
        }

        [DebuggerDisplay("Ignored Types")]
        public Type[] IgnoredTypes
        {
            get
            {
                InitDiscovery();
                return _ignoredTypes;
            }
        }

        void InitDiscovery()
        {
            if (_discoveredParts != null)
                return;

            var types = _configuration.DebugGetRegisteredTypes();
            var defaultAttributeContext = _configuration.DebugGetDefaultAttributeContext() ?? new DirectAttributeContext();
            var discovered = new List<DiscoveredPart>();
            var ignored = new List<Type>();

            foreach (var typeSet in types)
            {
                var ac = typeSet.Item2 ?? defaultAttributeContext;
                var activationFeatures = TypedPartExportDescriptorProvider.DebugGetActivationFeatures(ac);
                var inspector = new TypeInspector(ac, activationFeatures);

                foreach (var type in typeSet.Item1)
                {
                    DiscoveredPart part;
                    if (inspector.InspectTypeForPart(type, out part))
                        discovered.Add(part);
                    else
                        ignored.Add(type);
                }
            }

            _discoveredParts = discovered.ToArray();
            _ignoredTypes = ignored.ToArray();
        }
    }
}