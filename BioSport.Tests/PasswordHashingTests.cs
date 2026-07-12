using Xunit;

namespace BioSport.Tests
{
    public class PasswordHashingTests
    {
        [Fact]
        public void HashPassword_VerifyConLaMismaContrasena_RetornaTrue()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("MiClave123");

            var esValida = BCrypt.Net.BCrypt.Verify("MiClave123", hash);

            Assert.True(esValida);
        }

        [Fact]
        public void HashPassword_VerifyConContrasenaIncorrecta_RetornaFalse()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("MiClave123");

            var esValida = BCrypt.Net.BCrypt.Verify("OtraClave456", hash);

            Assert.False(esValida);
        }

        [Fact]
        public void HashPassword_MismaContrasenaDosVeces_GeneraHashesDistintos()
        {
            var hash1 = BCrypt.Net.BCrypt.HashPassword("MiClave123");
            var hash2 = BCrypt.Net.BCrypt.HashPassword("MiClave123");

            Assert.NotEqual(hash1, hash2);
        }
    }
}
