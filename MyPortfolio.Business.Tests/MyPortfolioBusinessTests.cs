using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using MyPortfolio.Business.Utilities;
using MyPortfolio.DataAccess.EntityModels;
using MyPortfolio.DataAccess.Repositories;
using MyPortfolio.Models.AppModels;
using MyPortfolio.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.Business.Tests
{
    [TestFixture]
    public class MyPortfolioBusinessTests
    {
        //business layer dependencies would be mocked here
        private Mock<ILogger<MyPortfolioBusiness>> _loggerMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPasswordHasher> _passwordHasherMock;
        private MyPortfolioBusiness _myPortfolioBusiness;

        [SetUp]
        public void SetUp()
        {
            this._loggerMock = new Mock<ILogger<MyPortfolioBusiness>>();
            this._userRepositoryMock = new Mock<IUserRepository>();
            this._passwordHasherMock = new Mock<IPasswordHasher>();
            _myPortfolioBusiness = new MyPortfolioBusiness(_loggerMock.Object, _userRepositoryMock.Object, _passwordHasherMock.Object);
        }

        private void MockSetupsFor_ValidateUserAccountAsync(User user, bool passwordValid)
        {
            _userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(user);
            _passwordHasherMock.Setup(hasher => hasher.VerifyPassword(It.IsAny<HashPasswordInfo>(), It.IsAny<string>())).Returns(passwordValid);
            _userRepositoryMock.Setup(repo => repo.UpdateUserLastLoginAsync(It.IsAny<User>()));
        }

        [Test]
        public void ValidateUserAccountAsync_ShouldReturn_True_ForValidCredentials()
        {
            //Arrange
            LoginViewModel loginDetails = new LoginViewModel() { EmailAddress ="Kiran.BS@carelon.com", Password="Test@123" };
            this.MockSetupsFor_ValidateUserAccountAsync(new User() { LastName = "BS", FirstName = "kiran", Email = "test@email.com", IsActive = true }, true);

            //Act
            var businessData = _myPortfolioBusiness.ValidateUserAccountAsync(loginDetails).GetAwaiter().GetResult();
            
            //Assert
            Assert.That(businessData.IsSuccess, Is.True);
            Assert.That(businessData.Errors, Is.Empty);
            Assert.That(((CookieInformation)businessData.AdditionalData).UserEmail, Is.EqualTo("test@email.com"));
            Assert.That(((CookieInformation)businessData.AdditionalData).UserName, Is.EqualTo("kiran BS"));
        }

        [Test]
        public void ValidateUserAccountAsync_ShouldReturn_False_ForInValidCredentials_Passoword()
        {
            //Arrange
            LoginViewModel loginDetails = new LoginViewModel() { EmailAddress = "Kiran.BS@carelon.com", Password = "Test@123" };
            this.MockSetupsFor_ValidateUserAccountAsync(new User() { LastName = "BS", FirstName = "kiran", Email = "test@email.com", IsActive = true }, false);

            //Act
            var businessData = _myPortfolioBusiness.ValidateUserAccountAsync(loginDetails).GetAwaiter().GetResult();

            //Assert
            Assert.That(businessData.IsSuccess, Is.False);
            Assert.That(businessData.AdditionalData, Is.Null);
            Assert.That(businessData.Errors.Count, Is.EqualTo(1));
            Assert.That(businessData.Errors[0], Is.EqualTo(AppMessage.ERROR_INVALID_CREDENTIALS));
        }

        [Test]
        public void ValidateUserAccountAsync_ShouldReturn_False_ForInValidCredentials_InvalidUser()
        {
            //Arrange
            LoginViewModel loginDetails = new LoginViewModel() { EmailAddress = "Kiran.BS@carelon.com", Password = "Test@123" };
            this.MockSetupsFor_ValidateUserAccountAsync(null, false);

            //Act
            var businessData = _myPortfolioBusiness.ValidateUserAccountAsync(loginDetails).GetAwaiter().GetResult();

            //Assert
            Assert.That(businessData.IsSuccess, Is.False);
            Assert.That(businessData.AdditionalData, Is.Null);
            Assert.That(businessData.Errors.Count, Is.EqualTo(1));
            Assert.That(businessData.Errors[0], Is.EqualTo(AppMessage.ERROR_INVALID_CREDENTIALS));
        }
    }
}
