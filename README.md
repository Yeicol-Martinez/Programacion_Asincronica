# 🎲 Generador de Usuarios Aleatorios - JSONPlaceholder API

Aplicación de consola en C# que demuestra el uso de **programación asíncrona** con `async/await` y **concurrencia** para obtener usuarios aleatorios de la API de JSONPlaceholder.

## 📋 Características

✅ **Programación Asíncrona**: Utiliza `async/await` para operaciones no bloqueantes  
✅ **Concurrencia Controlada**: Múltiples solicitudes simultáneas con límite de 5 concurrentes  
✅ **Manejo de Errores**: Reintentos automáticos (hasta 3 intentos) con espera incremental  
✅ **Sistema de Caché**: Optimización para APIs con datos fijos  
✅ **Indicador de Progreso**: Barra de progreso visual en tiempo real  
✅ **Interfaz Amigable**: Colores y emojis para mejor experiencia de usuario  
✅ **Principios SOLID**: Implementa SRP y DIP  

## 🔄 Sobre JSONPlaceholder API

**JSONPlaceholder** (https://jsonplaceholder.typicode.com) es una API REST gratuita para testing y prototipado que proporciona:
- **10 usuarios únicos y fijos**
- Datos realistas (nombres, emails, direcciones, teléfonos, compañías)
- Sin necesidad de autenticación
- Respuestas rápidas y confiables

### ⚠️ Diferencia con Random User API
A diferencia de Random User API que genera usuarios nuevos cada vez, JSONPlaceholder tiene un conjunto fijo de 10 usuarios. Esta aplicación:
1. Descarga todos los usuarios **una sola vez** (caché)
2. Selecciona **aleatoriamente** de ese conjunto
3. Permite solicitar más de 10 usuarios (se repetirán aleatoriamente)

## 🏗️ Arquitectura y Principios SOLID

### 1️⃣ **Single Responsibility Principle (SRP)**
Cada clase tiene una única responsabilidad:

- **`JsonPlaceholderUserService`**: Responsable únicamente de comunicarse con la API de JSONPlaceholder
- **`UserDisplayService`**: Responsable solo de mostrar información en consola
- **`UserFetchOrchestrator`**: Responsable de orquestar el proceso de obtención concurrente
- **Modelos de datos**: Representan únicamente la estructura de datos de la API

### 2️⃣ **Dependency Inversion Principle (DIP)**
- Se define la interfaz `IUserService` que abstrae la implementación
- El `UserFetchOrchestrator` depende de la abstracción, no de la implementación concreta
- Facilita pruebas unitarias y permite cambiar fácilmente entre diferentes APIs

## 🚀 Requisitos

- **.NET 6.0 SDK** o superior
- Conexión a Internet (para acceder a la API)

## 📦 Instalación y Ejecución

### Paso 1: Renombrar el archivo
```bash
# Renombra el archivo para que coincida con el proyecto
mv RandomUserApp_JSONPlaceholder.cs RandomUserApp.cs
```

### Paso 2: Compilar y ejecutar
```bash
# Compilar el proyecto
dotnet build RandomUserApp.csproj

# Ejecutar la aplicación
dotnet run --project RandomUserApp.csproj
```

### Opción alternativa:
```bash
# Compilar
dotnet build

# Ejecutar el ejecutable generado (Windows)
./bin/Debug/net6.0/RandomUserApp.exe

# O ejecutar el .dll (multiplataforma)
dotnet ./bin/Debug/net6.0/RandomUserApp.dll
```

## 🎯 Funcionalidades Implementadas

### 1. Solicitud de cantidad de usuarios ✨
Al iniciar, la aplicación pregunta cuántos usuarios deseas obtener (pueden ser más de 10, se repetirán aleatoriamente).

### 2. Solicitudes asíncronas con sistema de caché 🚀
- **Primera solicitud**: Descarga todos los usuarios de la API
- **Solicitudes posteriores**: Usa caché local (más rápido)
- **Selección aleatoria**: Cada "usuario" se selecciona aleatoriamente del conjunto
- **Concurrencia controlada**: Límite de 5 operaciones simultáneas
- **Uso de `SemaphoreSlim`**: Control de concurrencia thread-safe

### 3. Visualización completa de datos 📊
Cada usuario muestra:
- 👤 **Nombre completo** (ej. "Leanne Graham")
- 🏷️ **Username** (ej. "Bret")
- 📧 **Email** (ej. "Sincere@april.biz")
- 🏢 **Compañía** (ej. "Romaguera-Crona")
- 🌍 **Ciudad** (ej. "Gwenborough")
- 📍 **Dirección completa** (calle y suite)
- 📞 **Teléfono** (ej. "1-770-736-8031 x56442")
- 🌐 **Website** (ej. "hildegard.org")

### 4. Indicador de progreso en tiempo real ⏱️
Muestra:
- Cantidad de usuarios obtenidos vs total solicitado
- Barra de progreso visual animada
- Actualización dinámica mientras se procesan las tareas asíncronas

### 5. Manejo robusto de errores 🛡️
- **Reintentos automáticos**: Hasta 3 intentos por solicitud fallida
- **Espera incremental**: 1s, 2s, 3s entre reintentos (backoff exponencial)
- **Mensajes descriptivos**: Indica claramente qué salió mal
- **Manejo de JSON inválido**: Captura errores de deserialización
- **Recuperación graceful**: La aplicación nunca se cierra inesperadamente

### 6. Menú interactivo 🔄
Después de cada búsqueda:
- Opción para buscar más usuarios (reutiliza caché)
- Opción para salir de la aplicación
- Limpieza de pantalla entre búsquedas para mejor UX

## 🔧 Características Técnicas

### Programación Asíncrona
```csharp
// Ejemplo de uso de async/await con manejo de errores
public async Task<User> GetRandomUserAsync()
{
    if (_cachedUsers == null)
    {
        var response = await _httpClient.GetAsync(API_URL);
        response.EnsureSuccessStatusCode();
        
        var jsonContent = await response.Content.ReadAsStringAsync();
        _cachedUsers = JsonSerializer.Deserialize<List<User>>(jsonContent);
    }
    
    // Selección aleatoria del caché
    int randomIndex = _random.Next(0, _cachedUsers.Count);
    return _cachedUsers[randomIndex];
}
```

### Sistema de Caché Inteligente
```csharp
private List<User> _cachedUsers; // Caché de usuarios

// Primera vez: descarga de API
if (_cachedUsers == null) {
    _cachedUsers = await DescargarTodosLosUsuarios();
}

// Siguientes veces: usa caché (instantáneo)
return _cachedUsers[_random.Next(0, _cachedUsers.Count)];
```

### Concurrencia con Semáforos
```csharp
var semaphore = new SemaphoreSlim(5); // Máximo 5 operaciones concurrentes

await semaphore.WaitAsync();
try
{
    return await _userService.GetRandomUserAsync();
}
finally
{
    semaphore.Release();
}
```

### Manejo de Tareas Concurrentes con Progreso
```csharp
// Crear múltiples tareas asíncronas
var tasks = new List<Task<User>>();
for (int i = 0; i < count; i++)
{
    tasks.Add(GetUserTask());
}

// Procesar conforme se completan (no esperar a todas)
while (completed < count && pendingTasks.Count > 0)
{
    var completedTask = await Task.WhenAny(pendingTasks);
    pendingTasks.Remove(completedTask);
    
    var user = await completedTask;
    users.Add(user);
    
    // Actualizar progreso en tiempo real
    _displayService.DisplayProgress(++completed, count);
}
```

## 📊 Ejemplo de Uso Completo

```
╔════════════════════════════════════════════════════════╗
║     GENERADOR DE USUARIOS ALEATORIOS - API CLIENT     ║
║              JSONPlaceholder API Version               ║
╔════════════════════════════════════════════════════════╗

📊 ¿Cuántos usuarios aleatorios deseas obtener? 5

🚀 Iniciando obtención de 5 usuario(s)...

⏳ Obteniendo usuarios... [5/5] [██████████████████████████████]

✅ Se obtuvieron 5 usuario(s) exitosamente!

┌─── Usuario #1 ───────────────────────────────────
│ 👤 Nombre completo: Leanne Graham
│ 🏷️  Username: Bret
│ 📧 Email: Sincere@april.biz
│ 🏢 Compañía: Romaguera-Crona
│ 🌍 Ciudad: Gwenborough
│ 📍 Dirección: Kulas Light, Apt. 556
│ 📞 Teléfono: 1-770-736-8031 x56442
│ 🌐 Website: hildegard.org
└────────────────────────────────────────────────────────

┌─── Usuario #2 ───────────────────────────────────
│ 👤 Nombre completo: Ervin Howell
│ 🏷️  Username: Antonette
│ 📧 Email: Shanna@melissa.tv
│ 🏢 Compañía: Deckow-Crist
│ 🌍 Ciudad: Wisokyburgh
│ 📍 Dirección: Victor Plains, Suite 879
│ 📞 Teléfono: 010-692-6593 x09125
│ 🌐 Website: anastasia.net
└────────────────────────────────────────────────────────

[... más usuarios ...]

🔄 ¿Deseas buscar más usuarios? (S/N): s

📊 ¿Cuántos usuarios aleatorios deseas obtener? 3

🚀 Iniciando obtención de 3 usuario(s)...

✅ Se obtuvieron 3 usuario(s) exitosamente!

[... resultados ...]

🔄 ¿Deseas buscar más usuarios? (S/N): n

👋 ¡Gracias por usar el generador de usuarios! Hasta pronto.
```

## 🛡️ Manejo de Errores

La aplicación maneja múltiples tipos de errores:

### 1. Errores de Conexión HTTP
```
⚠️  Error en la solicitud (intento 1/3). Reintentando...
⚠️  Error en la solicitud (intento 2/3). Reintentando...
```

### 2. Errores de Deserialización JSON
```
❌ Error al obtener usuario: Error al deserializar JSON: ...
```

### 3. Timeouts
- Timeout configurado: 10 segundos por solicitud
- Reintentos automáticos con espera incremental

### 4. Entrada de Usuario Inválida
```
❌ Por favor, ingresa un número válido mayor a 0.
```

### 5. Errores Críticos
```
💥 Error crítico: [mensaje de error]
¿Deseas intentar nuevamente? (S/N):
```

## 🎨 Características de UI

- **🎨 Colores temáticos**: 
  - 🟣 Magenta: Títulos y encabezados
  - 🔵 Cyan: Información de usuarios
  - 🟡 Amarillo: Progreso y advertencias
  - 🟢 Verde: Mensajes de éxito
  - 🔴 Rojo: Errores

- **😀 Emojis descriptivos**: Mejoran la comprensión visual
- **📊 Barra de progreso**: Feedback visual en tiempo real
- **📦 Formato estructurado**: Presentación clara y organizada de datos

## 🔒 Buenas Prácticas Implementadas

✅ **HttpClient reutilizable** (evita agotamiento de sockets)  
✅ **Timeouts apropiados** (10 segundos)  
✅ **Liberación de recursos** con `Dispose()`  
✅ **Separación de responsabilidades** (SOLID)  
✅ **Manejo defensivo de errores**  
✅ **Concurrencia controlada** (máximo 5 operaciones simultáneas)  
✅ **Sistema de caché** para optimizar rendimiento  
✅ **Código limpio y autodocumentado**  
✅ **Reintentos con backoff exponencial**  
✅ **Validación de entrada de usuario**  

## 📝 Estructura del Código

```
RandomUserApp_JSONPlaceholder.cs
│
├── Modelos de Datos
│   ├── User (usuario principal)
│   ├── Address (dirección)
│   ├── Geo (coordenadas geográficas)
│   └── Company (información de compañía)
│
├── Interfaces
│   └── IUserService (abstracción del servicio)
│
├── Servicios
│   ├── JsonPlaceholderUserService (implementación de API)
│   ├── UserDisplayService (visualización)
│   └── UserFetchOrchestrator (orquestación)
│
└── Programa Principal
    └── Main() (punto de entrada)
```

## 🔄 Diferencias con Random User API

| Característica | JSONPlaceholder | Random User |
|----------------|----------------|-------------|
| **Usuarios** | 10 fijos | Infinitos generados |
| **Formato JSON** | Array directo `[...]` | Objeto wrapper `{"results": [...]}` |
| **Datos** | Nombre, username, compañía, dirección | Nombre separado, género, país |
| **Estrategia** | Caché + selección aleatoria | Solicitud individual por usuario |
| **Velocidad** | Muy rápido (caché) | Depende de la red |
| **Repetición** | Sí (10 únicos) | No |

## 💡 Notas Adicionales

- **Límite de usuarios**: JSONPlaceholder tiene solo 10 usuarios únicos
- **Repetición**: Si solicitas más de 10, se seleccionarán aleatoriamente y se repetirán
- **Advertencia visual**: La app avisa cuando solicitas más de 50 usuarios
- **Optimización**: Primera solicitud descarga todos los usuarios (caché), siguientes son instantáneas
- **Concurrencia**: Máximo 5 operaciones simultáneas para no saturar recursos
- **Timeout**: 10 segundos por operación HTTP
- **Reintentos**: Máximo 3 intentos con espera de 1s, 2s, 3s

## 🌐 API Utilizada

**JSONPlaceholder - Free Fake API**  
URL: https://jsonplaceholder.typicode.com/users  
Documentación: https://jsonplaceholder.typicode.com/guide/

### Ejemplo de respuesta de la API:
```json
[
  {
    "id": 1,
    "name": "Leanne Graham",
    "username": "Bret",
    "email": "Sincere@april.biz",
    "address": {
      "street": "Kulas Light",
      "suite": "Apt. 556",
      "city": "Gwenborough",
      "zipcode": "92998-3874",
      "geo": {
        "lat": "-37.3159",
        "lng": "81.1496"
      }
    },
    "phone": "1-770-736-8031 x56442",
    "website": "hildegard.org",
    "company": {
      "name": "Romaguera-Crona",
      "catchPhrase": "Multi-layered client-server neural-net",
      "bs": "harness real-time e-markets"
    }
  }
]
```

## 🎓 Conceptos Demostrados

### Programación Asíncrona
- ✅ `async` / `await`
- ✅ `Task<T>` y `Task`
- ✅ `Task.WhenAny()` para procesar tareas conforme se completan
- ✅ `Task.WhenAll()` para esperar múltiples tareas

### Concurrencia
- ✅ `SemaphoreSlim` para limitar operaciones concurrentes
- ✅ Ejecución paralela de múltiples tareas
- ✅ Manejo seguro de recursos compartidos

### Manejo de Errores
- ✅ Bloques `try-catch` apropiados
- ✅ Reintentos con backoff exponencial
- ✅ Manejo de diferentes tipos de excepciones
- ✅ Recuperación graceful de errores

### Principios SOLID
- ✅ Single Responsibility Principle
- ✅ Dependency Inversion Principle
- ✅ Separación de concerns

## 📚 Recursos de Aprendizaje

- [Documentación oficial de async/await en C#](https://docs.microsoft.com/en-us/dotnet/csharp/async)
- [JSONPlaceholder API Docs](https://jsonplaceholder.typicode.com/)
- [Principios SOLID explicados](https://www.digitalocean.com/community/conceptual_articles/s-o-l-i-d-the-first-five-principles-of-object-oriented-design)

---

**Desarrollado con ❤️ usando C# y .NET 8**  
**Versión adaptada para JSONPlaceholder API**

**Yeicol Martinez**
