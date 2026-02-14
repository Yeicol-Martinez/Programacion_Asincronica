using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Practica3
{
    // Orquestador principal (Principio SOLID: Single Responsibility)
    public class UserFetchOrchestrator
    {
        private readonly IUserService _userService;
        private readonly UserDisplayService _displayService;

        public UserFetchOrchestrator(IUserService userService, UserDisplayService displayService)
        {
            _userService = userService;
            _displayService = displayService;
        }

        public async Task<List<User>> FetchUsersAsync(int count)
        {
            var users = new List<User>();
            var tasks = new List<Task<User>>();
            var semaphore = new System.Threading.SemaphoreSlim(5); // Limitar a 5 solicitudes concurrentes

            Console.WriteLine($"\n🚀 Iniciando obtención de {count} usuario(s)...\n");

            // Crear todas las tareas
            for (int i = 0; i < count; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        return await _userService.GetRandomUserAsync();
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            // Esperar y mostrar progreso
            int completed = 0;
            var pendingTasks = new List<Task<User>>(tasks);

            while (completed < count && pendingTasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(pendingTasks);
                pendingTasks.Remove(completedTask);

                try
                {
                    var user = await completedTask;
                    users.Add(user);
                    completed++;
                    _displayService.DisplayProgress(completed, count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Error al obtener usuario: {ex.Message}");
                    completed++;
                }
            }

            // Esperar a que todas terminen
            try
            {
                await Task.WhenAll(tasks);
            }
            catch
            {
                // Los errores ya fueron manejados en el loop anterior
            }

            _displayService.ClearProgressLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Se obtuvieron {users.Count} usuario(s) exitosamente!\n");
            Console.ResetColor();

            return users;
        }
    }
}