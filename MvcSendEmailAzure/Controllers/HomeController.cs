using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MvcSendEmailAzure.Filters;
using MvcSendEmailAzure.Helpers;
using MvcSendEmailAzure.Models;
using MvcSendEmailAzure.Services;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;

namespace MvcSendEmailAzure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private AppService service;

        public HomeController(ILogger<HomeController> logger, AppService service)
        {
            _logger = logger;
            this.service = service;
        }

        public IActionResult Index()
        {
            return View();
        }


        [AuthorizeUsuarios]
        public async Task<IActionResult> PedirCita(int idusuario)
        {
            string token =
                HttpContext.Session.GetString("TOKEN");

            List<Mascota> mascotas = await this.service.GetMascotas(token);
            ViewData["MASCOTAS"] = new List<Mascota>(mascotas);

            List<Cita> citas = await this.service.GetCitas(token);
            ViewData["CITAS"] = HelperJson.SerializeObject<List<Cita>>(citas);

            return View();
        }

        [AuthorizeUsuarios]
        [HttpPost]
        public async Task<IActionResult> PedirCita(int idmascota, string tipo, string fecha, string hora)
        {
            string token =
                HttpContext.Session.GetString("TOKEN");
            Usuario usuario = await
                this.service.GetPerfilUsuarioAsync(token);

            string dateTimeString = fecha + " " + hora + ":00.00";
            DateTime citaDateTime = DateTime.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss.ff", CultureInfo.InvariantCulture);

            await this.service.CreateCita(usuario.IdUsuario, idmascota, tipo, citaDateTime, token);

            Mascota mascota = await this.service.FindMascotaAsync(token, idmascota);

            List<Mascota> mascotas = await this.service.GetMascotas(token);
            ViewData["MASCOTAS"] = new List<Mascota>(mascotas);

            List<Cita> citas = await this.service.GetCitas(token);
            ViewData["CITAS"] = HelperJson.SerializeObject<List<Cita>>(citas);

            ViewData["MENSAJE"] = "Cita solicitada Correctamente";
            ViewData["FECHA"] = citaDateTime;

            DateTime fechaFormateada = DateTime.ParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            //CODIGO PARA EL EMAIL
            string email = usuario.Email;
            string asunto = "Cita Solicita Correctamente";
            string mensaje = "Cita solicitada para mascota:" + mascota.Nombre + " en el día " + fechaFormateada.ToString("dd-MM-yyyy") + " a las " + hora;

            await this.service.SendMailAsync(email, asunto, mensaje);


            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        #region LOGIN/LOGOUT

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            string token = await this.service.GetTokenAsync(username, password);

            if (token == null)
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
            }
            else
            {
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity =
                    new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name, ClaimTypes.Role);

                identity.AddClaim(new Claim(ClaimTypes.Name, username));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, password));
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal, new AuthenticationProperties
                        {
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                        });
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("TOKEN");
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}