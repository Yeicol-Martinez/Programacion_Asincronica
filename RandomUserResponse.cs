using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Practica3
{
        // Modelos para deserializar la respuesta de la API
        public class RandomUserResponse
        {
            public List<User> results { get; set; }
        }
}
