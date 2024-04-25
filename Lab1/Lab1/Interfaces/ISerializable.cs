using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    public interface ISerializable
    {
        string Serialize(); // Повертає рядок, який представляє дані об'єкта
        void Deserialize(string data); // Відновлює дані з рядка
    }

}
