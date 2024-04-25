using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public class Point3D : IPoint3D
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double DistanceTo(IPoint3D other)
        {
            double dx = other.X - X;
            double dy = other.Y - Y;
            double dz = other.Z - Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
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
