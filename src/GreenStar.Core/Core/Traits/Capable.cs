using System;
using GreenStar.Core.Persistence;

namespace GreenStar.Core.Traits
{
    public class Capable : Trait
    {
        private int[] _values;
        
        public string[] CapabilityNames { get; }
        
        public Capable(string[] capabilityNames)
        {
            if (capabilityNames == null)
            {
                throw new System.ArgumentNullException(nameof(capabilityNames));
            }

            CapabilityNames = capabilityNames;
            _values = new int[capabilityNames.Length];
        }

        public int Of(string name)
        {
            var position = Array.IndexOf(CapabilityNames, name);

            return position >= 0 ? _values[position] : 0;
        }

        public int Of(string name, int value)
        {
            var position = Array.IndexOf(CapabilityNames, name);

            if (position >= 0)
            {
                _values[position] = value;
            }

            return position >= 0 ? _values[position] : 0;
        }
    }
}