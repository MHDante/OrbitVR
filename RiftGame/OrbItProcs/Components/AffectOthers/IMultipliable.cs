using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrbItProcs
{
    public interface IMultipliable
    {
        Node parent { get; set; }
        bool active { get; set; }
        float multiplier { get; set; }
    }
}
