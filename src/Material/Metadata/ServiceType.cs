﻿using System;

namespace Material.Metadata
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceType : Attribute
    {
        public Type Type { get; set; }

        public ServiceType(Type type)
        {
            Type = type;
        }
    }
}
