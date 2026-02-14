using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Practica3
{
    // Clase para mostrar información (Principio SOLID: Single Responsibility)
    public class UserDisplayService
    {
        public void DisplayUser(User user, int index)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n┌─── Usuario #{index} ───────────────────────────────────");
            Console.ResetColor();

            Console.WriteLine($"│ 👤 Nombre completo: {user.name}");
            Console.WriteLine($"│ 🏷️  Username: {user.username}");
            Console.WriteLine($"│ 📧 Email: {user.email}");
            Console.WriteLine($"│ 🏢 Compañía: {user.company.name}");
            Console.WriteLine($"│ 🌍 Ciudad: {user.address.city}");
            Console.WriteLine($"│ 📍 Dirección: {user.address.street}, {user.address.suite}");
            Console.WriteLine($"│ 📞 Teléfono: {user.phone}");
            Console.WriteLine($"│ 🌐 Website: {user.website}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("└────────────────────────────────────────────────────────");
            Console.ResetColor();
        }

        public void DisplayProgress(int current, int total)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\r⏳ Obteniendo usuarios... [{current}/{total}] ");

            // Barra de progreso
            int progressWidth = 30;
            int progress = (int)((double)current / total * progressWidth);
            Console.Write("[");
            Console.Write(new string('█', progress));
            Console.Write(new string('░', progressWidth - progress));
            Console.Write("]");

            Console.ResetColor();
        }

        public void ClearProgressLine()
        {
            Console.Write("\r" + new string(' ', 80) + "\r");
        }
    }
}