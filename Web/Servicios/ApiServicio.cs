using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Web.Servicios
{

    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class ApiServicio
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _ctx;

        public ApiServicio(IHttpClientFactory httpClientFactory,
                           IHttpContextAccessor ctxAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _ctx = ctxAccessor;
        }

        private HttpClient CrearClienteConToken()
        {
            HttpClient http = _httpClientFactory.CreateClient("api");

            HttpContext? httpContext = _ctx.HttpContext;
            if (httpContext != null)
            {
                // 1) Intentar primero desde Session
                string? token = httpContext.Session.GetString("token");

                // 2) Si no hay en Session, intentar desde los claims del usuario autenticado
                if (string.IsNullOrWhiteSpace(token) &&
                    httpContext.User?.Identity != null &&
                    httpContext.User.Identity.IsAuthenticated)
                {
                    token = httpContext.User.Claims
                        .FirstOrDefault(c => c.Type == "Token")?.Value;
                }

                // 3) Si tenemos token, se envía como Bearer
                if (!string.IsNullOrWhiteSpace(token))
                {
                    http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return http;
        }


        private static readonly JsonSerializerOptions JsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        // GET “seguro”
        public async Task<T?> GetSafeAsync<T>(string url)
        {
            using HttpClient http = CrearClienteConToken();
            using HttpResponseMessage resp = await http.GetAsync(url);

            string str = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                // Aquí ya no “ocultamos” el error.
                // Lanzamos una excepción con el código y el cuerpo que devuelve el API.
                throw new HttpRequestException(
                    $"GET {url} -> {(int)resp.StatusCode}: {str}");
            }

            T? dato = JsonSerializer.Deserialize<T>(str, JsonOptions);
            return dato;
        }


        // POST sin respuesta
        public async Task PostAsync<TReq>(string url, TReq body)
        {
            using HttpClient http = CrearClienteConToken();

            string json = JsonSerializer.Serialize(body);
            using StringContent contenido = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage resp = await http.PostAsync(url, contenido);
            string str = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"POST {url} -> {(int)resp.StatusCode}: {str}");
            }
        }

        // POST con respuesta tipada
        public async Task<TResp?> PostAsync<TReq, TResp>(string url, TReq body)
        {
            using HttpClient http = CrearClienteConToken();

            string json = JsonSerializer.Serialize(body);
            using StringContent contenido = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage resp = await http.PostAsync(url, contenido);
            string str = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"POST {url} -> {(int)resp.StatusCode}: {str}");
            }

            TResp? dato = JsonSerializer.Deserialize<TResp>(str, JsonOptions);
            return dato;
        }

        // PUT
        public async Task PutAsync<TReq>(string url, TReq body)
        {
            using HttpClient http = CrearClienteConToken();

            string json = JsonSerializer.Serialize(body);
            using StringContent contenido = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage resp = await http.PutAsync(url, contenido);
            string str = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"PUT {url} -> {(int)resp.StatusCode}: {str}");
            }
        }

        // DELETE
        public async Task DeleteAsync(string url)
        {
            using HttpClient http = CrearClienteConToken();
            using HttpResponseMessage resp = await http.DeleteAsync(url);
            string str = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"DELETE {url} -> {(int)resp.StatusCode}: {str}");
            }
        }
    }
}
