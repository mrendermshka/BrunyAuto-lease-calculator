using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public interface IPoint3D : ISerializable
    {
        double X { get; }
        double Y { get; }
        double Z { get; }

        double DistanceTo(IPoint3D other);
    }
}
