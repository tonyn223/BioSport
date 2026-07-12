using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BioSport.Tests
{
    // Pruebas de humo: levantan la aplicación real y verifican rutas que no
    // dependen de una base de datos disponible, para detectar que el build
    // y el arranque de la app siguen funcionando.
    public class SmokeTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public SmokeTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Login_Get_RespondeOkYMuestraElFormulario()
        {
            var cliente = _factory.CreateClient();

            var respuesta = await cliente.GetAsync("/Account/Login");
            var contenido = await respuesta.Content.ReadAsStringAsync();

            respuesta.EnsureSuccessStatusCode();
            Assert.Contains("Iniciar Sesión", contenido);
        }

        [Fact]
        public async Task AccessDenied_Get_RespondeOk()
        {
            var cliente = _factory.CreateClient();

            var respuesta = await cliente.GetAsync("/Account/AccessDenied");

            respuesta.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task RutaProtegida_SinSesion_RedirigeALogin()
        {
            var cliente = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var respuesta = await cliente.GetAsync("/Usuarios");

            Assert.Equal(System.Net.HttpStatusCode.Redirect, respuesta.StatusCode);
            Assert.Contains("/Account/Login", respuesta.Headers.Location!.ToString());
        }
    }
}
