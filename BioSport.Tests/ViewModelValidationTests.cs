using System.ComponentModel.DataAnnotations;
using BioSport.ViewModels;
using Xunit;

namespace BioSport.Tests
{
    public class ViewModelValidationTests
    {
        private static IList<ValidationResult> Validar(object modelo)
        {
            var contexto = new ValidationContext(modelo);
            var resultados = new List<ValidationResult>();
            Validator.TryValidateObject(modelo, contexto, resultados, validateAllProperties: true);
            return resultados;
        }

        [Fact]
        public void OlvidoContrasena_ContrasenasNoCoinciden_FallaValidacion()
        {
            var modelo = new OlvidoContrasenaViewModel
            {
                Email = "cliente@correo.com",
                CodigoAcceso = "1234",
                NuevaContrasena = "clave123",
                ConfirmarContrasena = "otraClave456"
            };

            var resultados = Validar(modelo);

            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(OlvidoContrasenaViewModel.ConfirmarContrasena)));
        }

        [Fact]
        public void OlvidoContrasena_DatosValidos_PasaValidacion()
        {
            var modelo = new OlvidoContrasenaViewModel
            {
                Email = "cliente@correo.com",
                CodigoAcceso = "1234",
                NuevaContrasena = "clave123",
                ConfirmarContrasena = "clave123"
            };

            var resultados = Validar(modelo);

            Assert.Empty(resultados);
        }

        [Fact]
        public void CambiarContrasena_NuevaContrasenaMuyCorta_FallaValidacion()
        {
            var modelo = new CambiarContrasenaViewModel
            {
                ContrasenaActual = "actual123",
                NuevaContrasena = "123",
                ConfirmarContrasena = "123"
            };

            var resultados = Validar(modelo);

            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(CambiarContrasenaViewModel.NuevaContrasena)));
        }

        [Fact]
        public void SetupAdmin_CorreoInvalido_FallaValidacion()
        {
            var modelo = new SetupAdminViewModel
            {
                Nombre = "Admin",
                Email = "no-es-un-correo",
                Password = "clave123",
                ConfirmarPassword = "clave123"
            };

            var resultados = Validar(modelo);

            Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(SetupAdminViewModel.Email)));
        }
    }
}
