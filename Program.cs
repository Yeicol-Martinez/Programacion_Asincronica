using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Practica3
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configuración de servicios (Dependency Injection manual)
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);

            var userService = new JsonPlaceholderUserService(httpClient);
            var displayService = new UserDisplayService();
            var orchestrator = new UserFetchOrchestrator(userService, displayService);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     GENERADOR DE USUARIOS ALEATORIOS - API CLIENT     ║");
            Console.WriteLine("║              JSONPlaceholder API Version               ║");
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.ResetColor();

            bool continuar = true;

            while (continuar)
            {
                try
                {
                    // Paso 1: Solicitar cantidad de usuarios
                    Console.Write("\n📊 ¿Cuántos usuarios aleatorios deseas obtener? ");
                    string input = Console.ReadLine();

                    if (!int.TryParse(input, out int cantidadUsuarios) || cantidadUsuarios <= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Por favor, ingresa un número válido mayor a 0.");
                        Console.ResetColor();
                        continue;
                    }

                    if (cantidadUsuarios > 50)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️  Nota: JSONPlaceholder solo tiene 10 usuarios únicos. Se repetirán aleatoriamente.");
                        Console.ResetColor();
                    }

                    // Paso 2-4: Obtener usuarios de forma asíncrona con progreso
                    var usuarios = await orchestrator.FetchUsersAsync(cantidadUsuarios);

                    // Paso 3: Mostrar resultados
                    for (int i = 0; i < usuarios.Count; i++)
                    {
                        displayService.DisplayUser(usuarios[i], i + 1);
                    }

                    // Paso 6: Preguntar si desea continuar
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("\n🔄 ¿Deseas buscar más usuarios? (S/N): ");
                    Console.ResetColor();

                    string respuesta = Console.ReadLine()?.Trim().ToUpper();
                    continuar = respuesta == "S" || respuesta == "SI" || respuesta == "SÍ";

                    if (continuar)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
                        Console.WriteLine("║     GENERADOR DE USUARIOS ALEATORIOS - API CLIENT     ║");
                        Console.WriteLine("║              JSONPlaceholder API Version               ║");
                        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n💥 Error crítico: {ex.Message}");
                    Console.ResetColor();

                    Console.Write("\n¿Deseas intentar nuevamente? (S/N): ");
                    string respuesta = Console.ReadLine()?.Trim().ToUpper();
                    continuar = respuesta == "S" || respuesta == "SI" || respuesta == "SÍ";
                }
            }

            // Limpieza
            httpClient.Dispose();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n👋 ¡Gracias por usar el generador de usuarios! Hasta pronto.\n");
            Console.ResetColor();
        }
    }
}