using Azure.Security.KeyVault.Secrets;
using MvcSendEmailAzure.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MvcSendEmailAzure.Services
{
    public class AppService
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApiUsuarios;

        public AppService(SecretClient secretClient )
        {
            this.Header =
                new MediaTypeWithQualityHeaderValue("application/json");

            KeyVaultSecret keyVaultSecret =
                 secretClient.GetSecretAsync("ApiEmailKey").Result.Value;
            this.UrlApiUsuarios =
                keyVaultSecret.Value;
        }

        public async Task SendMailAsync(string email, string asunto, string mensaje)
        {
            string urlEmail = "https://prod-172.westeurope.logic.azure.com:443/workflows/99ae61627fb54ff886d756d5ea9b8a22/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=gMWMq9HvXev3f4yrWAk3G2Z00lJCumUCLfAoFXaq3ww";
            var model = new
            {
                email = email,
                subject = asunto,
                mensaje = mensaje
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string json = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync(urlEmail, content);
            }
        }



        #region METODOS NO RELEVANTES PARA EL ENVIO DEL EMAIL

        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiUsuarios);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data =
                        await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiUsuarios);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };

                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data =
                        await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token =
                        jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<Mascota>> GetMascotas(string token)
        {
            string request = "/api/mascotas";
            List<Mascota> mascotas = await
                this.CallApiAsync<List<Mascota>>(request, token);
            return mascotas;
        }

        public async Task<List<Cita>> GetCitas(string token)
        {
            string request = "/api/usuarios/citas";
            List<Cita> citas = await
                this.CallApiAsync<List<Cita>>(request, token);
            return citas;
        }

        public async Task CreateCita(int idusuario, int idmascota, string tipo, DateTime fecha, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/usuarios/solicitarcita";
                client.BaseAddress = new Uri(this.UrlApiUsuarios);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);

                Cita cita = new Cita();
                cita.IdCita = 0;
                cita.TipoCita = tipo;
                cita.IdMascota = idmascota;
                cita.IdUsuario = idusuario;
                cita.DiaCita = fecha;

                string json = JsonConvert.SerializeObject(cita);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }

        }

        public async Task<Usuario> GetPerfilUsuarioAsync
            (string token)
        {
            string request = "/api/usuarios/perfilusuario";
            Usuario usuario = await
                this.CallApiAsync<Usuario>(request, token);
            return usuario;
        }

        public async Task<Mascota> FindMascotaAsync
            (string token, int id)
        {
            string request = "/api/mascotas/mascota/" + id;
            Mascota mascota = await
                this.CallApiAsync<Mascota>(request, token);
            return mascota;
        }

        #endregion

    }
}
