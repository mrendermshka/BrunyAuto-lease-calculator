using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class Vector3D : IVector3D
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double Length
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        public IVector3D Add(IVector3D other)
        {
            return new Vector3D(X + other.X, Y + other.Y, Z + other.Z);
        }

        public IVector3D Subtract(IVector3D other)
        {
            return new Vector3D(X - other.X, Y - other.Y, Z - other.Z);
        }

        public double DotProduct(IVector3D other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        public IVector3D CrossProduct(IVector3D other)
        {
            double newX = Y * other.Z - Z * other.Y;
            double newY = Z * other.X - X * other.Z;
            double newZ = X * other.Y - Y * other.X;
            return new Vector3D(newX, newY, newZ);
        }

        public string Serialize()
        {
            return $"{X},{Y},{Z}";
        }

        public void Deserialize(string data)
        {
            var parts = data.Split(',');
            if (parts.Length == 3)
            {
                X = double.Parse(parts[0]);
                Y = double.Parse(parts[1]);
                Z = double.Parse(parts[2]);
            }
        }
    }
}
