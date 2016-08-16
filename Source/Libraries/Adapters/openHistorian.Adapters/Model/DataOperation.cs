// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.Model
{
    public class DataOperation
    {
        public Guid? NodeID
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string AssemblyName
        {
            get;
            set;
        }

        public string TypeName
        {
            get;
            set;
        }

        [StringLength(200)]
        public string MethodName
        {
            get;
            set;
        }

        public string Arguments
        {
            get;
            set;
        }

        public int LoadOrder
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }
    }
}