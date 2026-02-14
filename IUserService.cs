using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Practica3
{
    // Interfaz para el servicio de usuarios (Principio SOLID: Dependency Inversion)
    public interface IUserService
    {
        Task<User> GetRandomUserAsync();
    }
}
