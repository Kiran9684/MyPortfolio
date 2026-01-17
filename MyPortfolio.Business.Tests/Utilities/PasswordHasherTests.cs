using Microsoft.Extensions.Logging;
using Moq;
using MyPortfolio.Business.Utilities;

namespace MyPortfolio.Business.Tests.Utilities
{
    [TestFixture]
    public class PasswordHasherTests
    {
        private IPasswordHasher _passwordHasher;
        private Mock<ILogger<IPasswordHasher>> _loggerMock = new Mock<ILogger<IPasswordHasher>>();
        private static IEnumerable<string> InvalidPasswords { get
            {
                yield return "dummy";
                yield return string.Empty;
            } 
        }

        [SetUp]
        public void Init()
        {
            //Arrange
            this._passwordHasher = new PasswordHasher(_loggerMock.Object);
        }

        [Test]
        public void VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
        {
            //Arrange 
            string password = "admin123";
            string hashedPassword = "/3UEOkJ1Y7Y8oo7wL1P3afxcawsuOeHEs09MfDtrS3M=";
            string salt = "EqFq8a9P6xdm/+8vavXIJw=="; 

            //Act
            var flag = this._passwordHasher.VerifyPassword(new Models.AppModels.HashPasswordInfo() { HashedPassword = hashedPassword, Salt = salt }, password); 
            Assert.That(flag, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(InvalidPasswords))]
        public void VerifyPassword_ShouldReturnFalse_ForIncorrectPassword(string password)
        {
            //Arrange 
            string hashedPassword = "/3UEOkJ1Y7Y8oo7wL1P3afxcawsuOeHEs09MfDtrS3M=";
            string salt = "EqFq8a9P6xdm/+8vavXIJw==";

            //Act
            var flag = this._passwordHasher.VerifyPassword(new Models.AppModels.HashPasswordInfo() { HashedPassword = hashedPassword, Salt = salt }, password);
            Assert.That(flag, Is.False);
        }

        [Test]
        public void VerifyPassword_ShouldThrow_Exception_ForNullPassword()
        {
            //Arrange 
            string password = null;
            string hashedPassword = "/3UEOkJ1Y7Y8oo7wL1P3afxcawsuOeHEs09MfDtrS3M=";
            string salt = "EqFq8a9P6xdm/+8vavXIJw==";

            //Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => this._passwordHasher.VerifyPassword(new Models.AppModels.HashPasswordInfo() { HashedPassword = hashedPassword, Salt = salt }, password));
        }
    }
}
