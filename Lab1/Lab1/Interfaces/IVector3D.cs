using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public interface IVector3D : ISerializable
    {
        double X { get; }
        double Y { get; }
        double Z { get; }

        double Length { get; }
        IVector3D Add(IVector3D other);
        IVector3D Subtract(IVector3D other);
        double DotProduct(IVector3D other);
        IVector3D CrossProduct(IVector3D other);
    }
}
