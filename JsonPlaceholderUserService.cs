using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Practica3
{
    public class JsonPlaceholderUserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private const string API_URL = "https://jsonplaceholder.typicode.com/users";
        private const int MAX_RETRIES = 3;
        private readonly Random _random;
        private List<User> _cachedUsers;

        public JsonPlaceholderUserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _random = new Random();
            _cachedUsers = null;
        }

        public async Task<User> GetRandomUserAsync()
        {
            int retryCount = 0;

            while (retryCount < MAX_RETRIES)
            {
                try
                {
                    // Si no tenemos usuarios en caché, obtenerlos todos
                    if (_cachedUsers == null)
                    {
                        var response = await _httpClient.GetAsync(API_URL);
                        response.EnsureSuccessStatusCode();

                        var jsonContent = await response.Content.ReadAsStringAsync();
                        _cachedUsers = JsonSerializer.Deserialize<List<User>>(jsonContent);

                        if (_cachedUsers == null || _cachedUsers.Count == 0)
                        {
                            throw new Exception("No se recibieron datos válidos de la API");
                        }
                    }

                    // Retornar un usuario aleatorio de la lista
                    int randomIndex = _random.Next(0, _cachedUsers.Count);
                    return _cachedUsers[randomIndex];
                }
                catch (HttpRequestException ex)
                {
                    retryCount++;
                    if (retryCount >= MAX_RETRIES)
                    {
                        throw new Exception($"Error de conexión después de {MAX_RETRIES} intentos: {ex.Message}");
                    }

                    Console.WriteLine($"⚠️  Error en la solicitud (intento {retryCount}/{MAX_RETRIES}). Reintentando...");
                    await Task.Delay(1000 * retryCount); // Espera incremental
                }
                catch (JsonException ex)
                {
                    throw new Exception($"Error al deserializar JSON: {ex.Message}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error inesperado: {ex.Message}");
                }
            }

            throw new Exception("No se pudo obtener el usuario después de varios intentos");
        }
    }
}
