using FluentAssertions;
using TransactionsLog.Services.TransactionParser;

namespace TransactionsLog.Services.Tests
{
    public class CNABTransactionParserTests
    {

        private readonly CNABTransactionParser _transactionParser;

        public CNABTransactionParserTests()
        {
            _transactionParser = new();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldFailIfLineIsInvalid(string line)
        {
            var expectedErrorMessage = "Registro vazio";

            var parsingResult = _transactionParser.ParseRecord(line);

            parsingResult.Item1.Should().BeNull();
            parsingResult.Item2.Should().Be(expectedErrorMessage);

        }

        [Fact]
        public void ShouldFailIfLineHasLessCharactersThanExpected()
        {
            var line = "rfhujisedfhuiosdjnsdjnibujnisdv";
            var expectedErrorMessage = "Quantidade de caracteres menor que o m�nimo esperado";

            var parsingResult = _transactionParser.ParseRecord(line);

            parsingResult.Item1.Should().BeNull();
            parsingResult.Item2.Should().Be(expectedErrorMessage);
        }

        [Fact]
        public void ShouldFailIfTransactionTypeIdIsNotANumber()
        {
            var line = "c201903010000014200096206760174753****3153153453JO�O MACEDO   BAR DO JO�O";
            var expectedErrorMessage = "Tipo de transa��o em formato inesperado";

            var parsingResult = _transactionParser.ParseRecord(line);

            parsingResult.Item1.Should().BeNull();
            parsingResult.Item2.Should().Be(expectedErrorMessage);
        }

        [Theory]
        [InlineData("20201301")] //m�s inv�lido
        [InlineData("20201235")] //dia inv�lido
        [InlineData("202012cc")] //caracter
        public void ShouldFailIfDateIsNotParseable(string dateAsString)
        {
            var line = $"1{dateAsString}0000014200096206760174753****3153153453JO�O MACEDO   BAR DO JO�O";
            var expectedErrorMessage = "Data informada inv�lida";

            var parsingResult = _transactionParser.ParseRecord(line);

            parsingResult.Item1.Should().BeNull();
            parsingResult.Item2.Should().Be(expectedErrorMessage);
        }

        [Fact]
        public void ShouldFailIfValueIsNotANumber()
        {
            var line = "12019030100000142cc096206760174753****3153153453JO�O MACEDO   BAR DO JO�O";
            var expectedErrorMessage = "Valor em formato inesperado";

            var parsingResult = _transactionParser.ParseRecord(line);

            parsingResult.Item1.Should().BeNull();
            parsingResult.Item2.Should().Be(expectedErrorMessage);
        }

        [Fact]
        public void ShouldFailIfCpfIsInvalid()
        {
            var line = "1201903010000014200cc6206760174753****3153153453JO�O MACEDO   BAR DO JO�O";
            var expectedErrorMessage = "Cpf inv�lido";

            var parsingResult = _transactionParser.ParseRecord(line);

            parsingResult.Item1.Should().BeNull();
            parsingResult.Item2.Should().Be(expectedErrorMessage);
        }

        [Theory]
        [InlineData("251515")] //hor� inv�lida
        [InlineData("236115")] //minuto inv�lido
        [InlineData("231561")] //segundo inv�lido
        [InlineData("2315cc")] //caracter
        public void ShouldFailIfTimeIsNotParseable(string timeAsString)
        {
            var line = $"1201903010000014200096206760174753****3153{timeAsString}JO�O MACEDO   BAR DO JO�O";
            var expectedErrorMessage = "Hora informada inv�lida";

            var parsingResult = _transactionParser.ParseRecord(line);

            parsingResult.Item1.Should().BeNull();
            parsingResult.Item2.Should().Be(expectedErrorMessage);
        }

        [Fact]        
        public void ShouldListAllErrorsWhenApplicable()
        {
            var line = $"1201913010000014200096206760174753****3153251515JO�O MACEDO   BAR DO JO�O";
            var expectedErrorMessage = "Data informada inv�lida; Hora informada inv�lida";

            var parsingResult = _transactionParser.ParseRecord(line);

            parsingResult.Item1.Should().BeNull();
            parsingResult.Item2.Should().Be(expectedErrorMessage);
        }

        [Fact]
        public void ShouldCorrectValueToAccountForDecimals()
        {
            var absoluteValueFormatted = "0000014200";
            var absoluteValueExpected = 142m;
            var line = $"320190301{absoluteValueFormatted}096206760174753****3153153453JO�O MACEDO   BAR DO JO�O";

            var parsingResult = _transactionParser.ParseRecord(line);

            parsingResult.Item1.Should().NotBeNull();
            parsingResult.Item1.AbsoluteValue.Should().Be(absoluteValueExpected);
        }

        [Fact]
        public void ShouldCombineDateAndTimeIntoTimestamp()
        {
            var expectedTimestamp = new DateTime(2019, 03, 01, 15, 34, 53, DateTimeKind.Local);
            var line = $"3201903010000014200096206760174753****3153153453JO�O MACEDO   BAR DO JO�O";

            var parsingResult = _transactionParser.ParseRecord(line);

            parsingResult.Item1.Should().NotBeNull();
            parsingResult.Item1.Timestamp.Should().Be(expectedTimestamp);
        }
    }
}