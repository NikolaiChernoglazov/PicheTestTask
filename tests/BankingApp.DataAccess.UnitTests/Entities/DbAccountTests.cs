using BankingApp.Core;
using BankingApp.DataAccess.Entities;

namespace BankingApp.DataAccess.UnitTests.Entities
{
    public class DbAccountTests
    {
        [Fact]
        public void ToCoreEntity_ReturnsCorrectAccount()
        {
            // Arrange
            var dbAccount = new DbAccount
            {
                Id = 1,
                Iban = "TEST_IBAN",
                Currency = "USD",
                Amount = 1000m,
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Act
            var result = dbAccount.ToCoreEntity();

            // Assert
            Assert.Equal(dbAccount.Id, result.Id);
            Assert.Equal(dbAccount.Iban, result.Iban);
            Assert.Equal(dbAccount.Currency, result.Currency);
            Assert.Equal(dbAccount.Amount, result.Amount);
            Assert.Equal(dbAccount.CreatedAt, result.CreatedAt);
        }

        [Fact]
        public void FromCoreEntity_ReturnsCorrectDbAccount()
        {
            // Arrange
            var account = new Account(1, "TEST_IBAN", "USD", 1000m, DateTimeOffset.UtcNow);

            // Act
            var result = DbAccount.FromCoreEntity(account);

            // Assert
            Assert.Equal(account.Id, result.Id);
            Assert.Equal(account.Iban, result.Iban);
            Assert.Equal(account.Currency.ToUpper(), result.Currency);
            Assert.Equal(account.Amount, result.Amount);
            Assert.Equal(account.CreatedAt, result.CreatedAt);
        }
        
        [Theory]
        [InlineData("usd", "USD")]
        [InlineData("USD", "USD")]
        public void FromCoreEntity_UppercasesCurrency(string currency, string expectedCurrency)
        {
            // Arrange
            var account = new Account(1, "TEST_IBAN", currency, 1000m, DateTimeOffset.UtcNow);

            // Act
            var result = DbAccount.FromCoreEntity(account);

            // Assert
            Assert.Equal(expectedCurrency, result.Currency);
        }
    }
}