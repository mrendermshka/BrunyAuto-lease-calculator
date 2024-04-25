using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введіть дані (точка та вектор у форматі x1,y1,z1;x2,y2,z2):");
            string input = Console.ReadLine();

            var parts = input.Split(';');
            if (parts.Length != 2)
            {
                Console.WriteLine("Невірний формат вводу.");
                return;
            }

            // Отримання точки та вектора з вхідних даних
            var pointParts = parts[0].Split(',');
            var vectorParts = parts[1].Split(',');

            if (pointParts.Length != 3 || vectorParts.Length != 3)
            {
                Console.WriteLine("Невірний формат координат.");
                return;
            }

            // Ініціалізація точки та вектора
            double pointX = double.Parse(pointParts[0]);
            double pointY = double.Parse(pointParts[1]);
            double pointZ = double.Parse(pointParts[2]);

            double vectorX = double.Parse(vectorParts[0]);
            double vectorY = double.Parse(vectorParts[1]);
            double vectorZ = double.Parse(vectorParts[2]);

            IPoint3D point = new Point3D(pointX, pointY, pointZ);
            IVector3D vector = new Vector3D(vectorX, vectorY, vectorZ);

            // Одиничний вектор
            double length = vector.Length;
            IVector3D unitVector = new Vector3D(vector.X / length, vector.Y / length, vector.Z / length);

            // Нова точка
            IPoint3D newPoint = new Point3D(
                point.X + 2 * vector.X,
                point.Y + 2 * vector.Y,
                point.Z + 2 * vector.Z
            );

            // Результуючий рядок
            string result = $"Одиничний вектор: {unitVector.Serialize()}; " +
                            $"Нова точка: {newPoint.Serialize()}";

            Console.WriteLine(result);
        }

    }
}
