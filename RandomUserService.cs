using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Practica3
{
    // Implementación del servicio de usuarios (Principio SOLID: Single Responsibility)
    public class RandomUserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private const string API_URL = "https://jsonplaceholder.typicode.com/users";
        private const int MAX_RETRIES = 3;

        public RandomUserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User> GetRandomUserAsync()
        {
            int retryCount = 0;

            while (retryCount < MAX_RETRIES)
            {
                try
                {
                    var response = await _httpClient.GetAsync(API_URL);
                    response.EnsureSuccessStatusCode();

                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var userResponse = JsonSerializer.Deserialize<RandomUserResponse>(jsonContent);

                    if (userResponse?.results != null && userResponse.results.Count > 0)
                    {
                        return userResponse.results[0];
                    }

                    throw new Exception("No se recibieron datos válidos de la API");
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
                catch (Exception ex)
                {
                    throw new Exception($"Error inesperado: {ex.Message}");
                }
            }

            throw new Exception("No se pudo obtener el usuario después de varios intentos");
        }
    }
}