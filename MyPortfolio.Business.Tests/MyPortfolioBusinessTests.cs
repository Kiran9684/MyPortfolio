using Microsoft.Extensions.Logging;
using Moq;
using MyPortfolio.Business.Utilities;
using MyPortfolio.DataAccess.EntityModels;
using MyPortfolio.DataAccess.Repositories;
using MyPortfolio.Models.AppModels;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Business.Tests
{
    // NUnit test fixture for testing the business layer class `MyPortfolioBusiness`.
    // Each test constructs the business class with mocked dependencies so tests exercise
    // only business logic and do not touch external resources (DB, crypto, logger).
    [TestFixture]
    public class MyPortfolioBusinessTests
    {
        //business layer dependencies would be mocked here
        // Mock for logging dependency. We don't assert on logs in these tests,
        // but the concrete business implementation requires an ILogger instance.
        private Mock<ILogger<MyPortfolioBusiness>> _loggerMock;

        // Mock for IUserRepository which encapsulates data access operations related to users.
        // We configure this mock in each test to return controlled responses.
        private Mock<IUserRepository> _userRepositoryMock;

        // Mock for IPasswordHasher which handles password hashing and verification.
        // This isolates cryptographic operations from tests and lets us simulate
        // both valid and invalid password verification scenarios.
        private Mock<IPasswordHasher> _passwordHasherMock;

        // Instance of the class under test. Created in SetUp using mocked dependencies.
        private MyPortfolioBusiness _myPortfolioBusiness;

        // SetUp runs before each test method to create fresh mocks and a fresh business instance.
        // Using fresh mocks prevents state leakage between tests and keeps tests independent.
        [SetUp]
        public void SetUp()
        {
            // Create lightweight mocks for each dependency.
            this._loggerMock = new Mock<ILogger<MyPortfolioBusiness>>();
            this._userRepositoryMock = new Mock<IUserRepository>();
            this._passwordHasherMock = new Mock<IPasswordHasher>();

            // Create the business instance with mocked dependencies injected.
            // This follows dependency injection patterns used in production code.
            _myPortfolioBusiness = new MyPortfolioBusiness(_loggerMock.Object, _userRepositoryMock.Object, _passwordHasherMock.Object);
        }

        // Helper method to configure the mocks for validating an existing user during login.
        // - `user`: the user object that the repository should return (or null to simulate not found).
        // - `passwordValid`: whether the password verification should succeed or fail.
        //
        // Typical usage:
        //   MockSetupsFor_ValidateUserAccountAsync(existingUser, true);
        //   MockSetupsFor_ValidateUserAccountAsync(null, false); // user not found
        private void MockSetupsFor_ValidateUserAccountAsync(User? user, bool passwordValid)
        {
            // Configure repository to return the provided `user` when searching by email.
            // The `It.IsAny<string>()` and `It.IsAny<bool>()` allow any arguments in the call;
            // the test controls behavior by providing the `user` parameter.
            _userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
                               .ReturnsAsync(user);

            // Configure password hasher mock to return the desired verification result.
            // The real implementation would accept the stored hashed info and raw password.
            _passwordHasherMock.Setup(hasher => hasher.VerifyPassword(It.IsAny<HashPasswordInfo>(), It.IsAny<string>()))
                               .Returns(passwordValid);

            // Configure the repository update call (update last login) to be a no-op.
            // We don't assert on that call in these tests, but stubbing it prevents unexpected exceptions.
            _userRepositoryMock.Setup(repo => repo.UpdateUserLastLoginAsync(It.IsAny<User>()));
        }

        // Test: valid credentials should result in a successful authentication.
        // This verifies:
        //  - `IsSuccess` is true,
        //  - no errors are returned,
        //  - AdditionalData contains cookie information with correct email and display name.
        [Test]
        public void ValidateUserAccountAsync_ShouldReturn_True_ForValidCredentials()
        {
            // Arrange
            // Create a sample login view model representing user input from UI/form.
            LoginViewModel loginDetails = new LoginViewModel() { EmailAddress = "Kiran.BS@carelon.com", Password = "Test@123" };

            // Prepare repository to return an active user and password verification to succeed.
            // Note: the email in the returned `User` object is the source used by business layer
            // to populate cookie information; the email passed by the test is independent.
            this.MockSetupsFor_ValidateUserAccountAsync(
                new User() { LastName = "BS", FirstName = "kiran", Email = "test@email.com", IsActive = true },
                true);

            // Act
            // Call the asynchronous business method and synchronously wait for completion in test.
            var businessData = _myPortfolioBusiness.ValidateUserAccountAsync(loginDetails).GetAwaiter().GetResult();

            // Assert
            // Verify that the operation was successful and did not generate errors.
            Assert.That(businessData.IsSuccess, Is.True);
            Assert.That(businessData.Errors, Is.Empty);

            // AdditionalData is expected to hold a CookieInformation object on success.
            // We cast using `as` and assert the object is not null before accessing properties.
            var cookie = businessData.AdditionalData as CookieInformation;
            Assert.That(cookie, Is.Not.Null);

            // Verify that cookie information was populated from the returned `User`.
            Assert.That(cookie!.UserEmail, Is.EqualTo("test@email.com"));
            Assert.That(cookie.UserName, Is.EqualTo("kiran BS"));
        }

        // Test: if password verification fails, authentication must fail and return appropriate error.
        [Test]
        public void ValidateUserAccountAsync_ShouldReturn_False_ForInValidCredentials_Passoword()
        {
            // Arrange
            LoginViewModel loginDetails = new LoginViewModel() { EmailAddress = "Kiran.BS@carelon.com", Password = "Test@123" };

            // Configure mocks: user exists but password verification fails.
            this.MockSetupsFor_ValidateUserAccountAsync(
                new User() { LastName = "BS", FirstName = "kiran", Email = "test@email.com", IsActive = true },
                false);

            // Act
            var businessData = _myPortfolioBusiness.ValidateUserAccountAsync(loginDetails).GetAwaiter().GetResult();

            // Assert
            // Operation should fail, AdditionalData should be null, and there should be one specific error message.
            Assert.That(businessData.IsSuccess, Is.False);
            Assert.That(businessData.AdditionalData, Is.Null);
            Assert.That(businessData.Errors.Count, Is.EqualTo(1));
            Assert.That(businessData.Errors[0], Is.EqualTo(AppMessage.ERROR_INVALID_CREDENTIALS));
        }

        // Test: if user is not found, authentication must fail with the invalid credentials message.
        [Test]
        public void ValidateUserAccountAsync_ShouldReturn_False_ForInValidCredentials_InvalidUser()
        {
            // Arrange
            LoginViewModel loginDetails = new LoginViewModel() { EmailAddress = "Kiran.BS@carelon.com", Password = "Test@123" };

            // Configure mocks to simulate user not found (repository returns null).
            this.MockSetupsFor_ValidateUserAccountAsync(null, false);

            // Act
            var businessData = _myPortfolioBusiness.ValidateUserAccountAsync(loginDetails).GetAwaiter().GetResult();

            // Assert
            Assert.That(businessData.IsSuccess, Is.False);
            Assert.That(businessData.AdditionalData, Is.Null);
            Assert.That(businessData.Errors.Count, Is.EqualTo(1));
            Assert.That(businessData.Errors[0], Is.EqualTo(AppMessage.ERROR_INVALID_CREDENTIALS));
        }

        // Helper for configuring mocks used during user registration tests.
        // When `user` is non-null the repository is simulating that a user already exists;
        // when `user` is null the repository simulates "no existing user" and the business
        // method should attempt to create a new user.
        private void MockSetupsFor_CreateUserAccountAsync(User? user)
        {
            // Configure repository find call to return `user` (or null).
            _userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
                               .ReturnsAsync(user);

            // Configure password hasher to return a HashPasswordInfo object when hashing.
            // The business method reads `HashedPassword` and `Salt` from this object.
            _passwordHasherMock.Setup(hasher => hasher.HashPassword(It.IsAny<string>()))
                               .Returns(new HashPasswordInfo() {HashedPassword = "testing", Salt = "test" });

            // Configure repository to return true when creating a user (success).
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
                               .ReturnsAsync(true);
        }

        // Test: creating a user account with valid registration details should succeed.
        // Important: to exercise the creation branch of the business method we must ensure
        // the repository returns `null` for an existing user (no conflict).
        [Test]
        public void CreateUserAccountAsync_ShouldReturn_True_ForValidCredentials()
        {
            // Arrange
            // The registration details that would come from the UI/form.
            UserRegistrationViewModel UserRegistrationDetails = new UserRegistrationViewModel()
            {
                FullName = "Ayush Raj",
                EmailAddress = "ayushrj1516@gmail.com",
                Password = "Test@123"
            };    

            // Simulate "no existing user" so the business layer creates a new user.
            // If this were a non-null `User`, the business method would mark the registration
            // as failed due to an existing user and `IsSuccess` would be false.
            this.MockSetupsFor_CreateUserAccountAsync(null);

            // Act
            var businessData = _myPortfolioBusiness.CreateUserAccountAsync(UserRegistrationDetails).GetAwaiter().GetResult();

            // Assert
            // On successful creation we expect IsSuccess = true and no errors.
            Assert.That(businessData.IsSuccess, Is.True);
            Assert.That(businessData.Errors, Is.Empty);
        }

        // Test: creating a user account when the email is already in use should fail.
        // This verifies that the business method prevents duplicate user registrations
        // and returns the correct error message.
        [Test]
        public void CreateUserAccountAsync_ShouldReturn_False_WhenUserAlreadyExists()
        {
            // Arrange
            var registration = new UserRegistrationViewModel
            {
                FullName = "Ayush Raj",
                EmailAddress = "exists@example.com",
                Password = "Test@123"
            };

            // Simulate repository returning an existing active user so SUT will return failure
            this.MockSetupsFor_CreateUserAccountAsync(new User
            {
                FirstName = "kiran",
                LastName = "BS",
                Email = "exists@example.com",
                IsActive = true
            });

            // Act
            var businessData = _myPortfolioBusiness.CreateUserAccountAsync(registration).GetAwaiter().GetResult();

            // Assert
            Assert.That(businessData.IsSuccess, Is.False);
            Assert.That(businessData.AdditionalData, Is.Null);
            Assert.That(businessData.Errors.Count, Is.EqualTo(1));
            Assert.That(businessData.Errors[0], Is.EqualTo(AppMessage.ERROR_USER_ALREADY_EXISTS));

            // Verify that CreateUserAsync was not called because user already exists
            _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<User>()), Times.Never);
        }

        // Additional unit test cases for CreateUserAccountAsync (false / edge / mapping / exception scenarios)
        [Test]
        public void CreateUserAccountAsync_ShouldReturn_False_WhenUserExists_Inactive()
        {
            // Arrange
            var registration = new UserRegistrationViewModel
            {
                FullName = "Ayush Raj",
                EmailAddress = "exists@example.com",
                Password = "Test@123"
            };

            // Simulate repository returning an existing but inactive user
            this.MockSetupsFor_CreateUserAccountAsync(new User
            {
                FirstName = "kiran",
                LastName = "BS",
                Email = "exists@example.com",
                IsActive = false
            });

            // Act
            var businessData = _myPortfolioBusiness.CreateUserAccountAsync(registration).GetAwaiter().GetResult();

            // Assert
            Assert.That(businessData.IsSuccess, Is.False);
            Assert.That(businessData.AdditionalData, Is.Null);
            Assert.That(businessData.Errors.Count, Is.EqualTo(1));
            Assert.That(businessData.Errors[0], Is.EqualTo(AppMessage.ERROR_USER_EXISTS_INACTIVE));

            // Ensure CreateUserAsync was not called
            _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void CreateUserAccountAsync_ShouldReturn_False_WhenCreateUserFails()
        {
            // Arrange
            var registration = new UserRegistrationViewModel
            {
                FullName = "Ayush Raj",
                EmailAddress = "newuser@example.com",
                Password = "Test@123"
            };

            // Simulate "no existing user"
            this.MockSetupsFor_CreateUserAccountAsync(null);

            // Override CreateUserAsync to fail
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
                               .ReturnsAsync(false);

            // Act
            var businessData = _myPortfolioBusiness.CreateUserAccountAsync(registration).GetAwaiter().GetResult();

            // Assert
            Assert.That(businessData.IsSuccess, Is.False);
            Assert.That(businessData.Errors.Count, Is.EqualTo(1));
            Assert.That(businessData.Errors[0], Is.EqualTo(AppMessage.ERROR_SOMETHING_WENT_WRONG));
        }

        [Test]
        public void CreateUserAccountAsync_ShouldThrow_When_HashPasswordThrows()
        {
            // Arrange
            var registration = new UserRegistrationViewModel
            {
                FullName = "Ayush Raj",
                EmailAddress = "newuser@example.com",
                Password = "Test@123"
            };

            // Simulate "no existing user"
            _userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
                               .ReturnsAsync((User?)null);

            // Simulate hash failure (exception)
            _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>()))
                               .Throws(new InvalidOperationException("hash failed"));

            // Act & Assert: business method should propagate the exception
            Assert.Throws<InvalidOperationException>(() =>
                _myPortfolioBusiness.CreateUserAccountAsync(registration).GetAwaiter().GetResult());
        }

        [Test]
        public void CreateUserAccountAsync_ShouldThrow_When_FindUserThrows()
        {
            // Arrange
            var registration = new UserRegistrationViewModel
            {
                FullName = "Ayush Raj",
                EmailAddress = "newuser@example.com",
                Password = "Test@123"
            };

            // Simulate repository find throwing
            _userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
                               .ThrowsAsync(new Exception("db error"));

            // Act & Assert: exception propagates
            Assert.Throws<Exception>(() =>
                _myPortfolioBusiness.CreateUserAccountAsync(registration).GetAwaiter().GetResult());
        }

        [Test]
        public void CreateUserAccountAsync_ShouldPass_PasswordHashAndSalt_ToCreateUser()
        {
            // Arrange
            var registration = new UserRegistrationViewModel
            {
                FullName = "Ayush Raj",
                EmailAddress = "newuser@example.com",
                Password = "Test@123"
            };

            // Simulate "no existing user"
            _userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
                               .ReturnsAsync((User?)null);

            // Provide deterministic hash info
            _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>()))
                               .Returns(new HashPasswordInfo { HashedPassword = "h123", Salt = "s123" });

            // Ensure CreateUserAsync succeeds
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
                               .ReturnsAsync(true);

            // Act
            var businessData = _myPortfolioBusiness.CreateUserAccountAsync(registration).GetAwaiter().GetResult();

            // Assert success
            Assert.That(businessData.IsSuccess, Is.True);

            // Verify the user passed to CreateUserAsync contains the hashed password and salt
            _userRepositoryMock.Verify(repo =>
                repo.CreateUserAsync(It.Is<User>(u => u.PasswordHash == "h123" && u.PasswordSalt == "s123")),
                Times.Once);
        }

        [Test]
        public void CreateUserAccountAsync_ShouldSplitFullName_WhenSingleNameProvided_LastNameEmpty()
        {
            // Arrange
            var registration = new UserRegistrationViewModel
            {
                FullName = "Ayush",
                EmailAddress = "single@example.com",
                Password = "Test@123"
            };

            // Simulate "no existing user" and successful create
            this.MockSetupsFor_CreateUserAccountAsync(null);

            // Act
            var businessData = _myPortfolioBusiness.CreateUserAccountAsync(registration).GetAwaiter().GetResult();

            // Assert success
            Assert.That(businessData.IsSuccess, Is.True);

            // Verify CreateUserAsync received FirstName = "Ayush" and LastName = string.Empty
            _userRepositoryMock.Verify(repo =>
                repo.CreateUserAsync(It.Is<User>(u => u.FirstName == "Ayush" && u.LastName == string.Empty)),
                Times.Once);
        }

        [Test]
        public void CreateUserAccountAsync_ShouldPickSecondTokenAsLastName_WhenFullNameHasMultipleTokens()
        {
            // Arrange
            var registration = new UserRegistrationViewModel
            {
                FullName = "Ayush Raj Kumar", // business uses index 0 and 1 only
                EmailAddress = "multi@example.com",
                Password = "Test@123"
            };

            // Simulate "no existing user" and successful create
            this.MockSetupsFor_CreateUserAccountAsync(null);

            // Act
            var businessData = _myPortfolioBusiness.CreateUserAccountAsync(registration).GetAwaiter().GetResult();

            // Assert success
            Assert.That(businessData.IsSuccess, Is.True);

            // Verify CreateUserAsync received FirstName = "Ayush" and LastName = "Raj"
            _userRepositoryMock.Verify(repo =>
                repo.CreateUserAsync(It.Is<User>(u => u.FirstName == "Ayush" && u.LastName == "Raj")),
                Times.Once);
        }

        [Test]
        public void CreateUserAccountAsync_ShouldThrow_When_UserRegistrationIsNull()
        {
            // Arrange
            UserRegistrationViewModel? registration = null;

            // Act & Assert: calling with null should throw (current implementation does not guard against null)
            Assert.Throws<NullReferenceException>(() =>
                _myPortfolioBusiness.CreateUserAccountAsync(registration!).GetAwaiter().GetResult());
        }
    }
}