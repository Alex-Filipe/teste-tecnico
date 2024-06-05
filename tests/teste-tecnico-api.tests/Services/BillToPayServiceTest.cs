using Xunit;
using Moq; // Se você estiver usando Mocks
using teste_tecnico_api.src.Dtos;
using teste_tecnico_api.src.Interfaces;
using teste_tecnico_api.src.Services;
using teste_tecnico_api.src.Models;

namespace teste_tecnico_api.tests.Services
{
    public class BillToPayServiceTest
    {
        private readonly BillToPayService _service;
        private readonly Mock<IBillToPayRepository> _mockRepository;
        private readonly List<AllBillToPayDto> _allBillToPayDto;

        public BillToPayServiceTest()
        {
            _mockRepository = new Mock<IBillToPayRepository>();
            _service = new BillToPayService(_mockRepository.Object);
            _allBillToPayDto = [];
        }

        [Fact]
        public void TestGetAllBillsToPayReturnsListOfAllBillToPayDto()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllBillsToPay()).Returns(_allBillToPayDto);

            // Act
            var result = _service.GetAllBillsToPay();

            // Assert
            Assert.Equal(_allBillToPayDto, result);
        }

        [Fact]
        public void TestCreateNewBillToPay()
        {
            // Arrange
            var billToPayDto = new CreateBillToPayDto
            {
                Nome = "Conta de Luz",
                ValorOriginal = 100.00f,
                DataVencimento = new DateOnly(2024, 6, 10),
                DataPagamento = new DateOnly(2024, 6, 12)
            };

            var newBillToPay = new BillToPay
            {
                Nome = billToPayDto.Nome,
                ValorOriginal = billToPayDto.ValorOriginal,
                ValorCorrigido = 102.20,
                QuantidadeDiasAtraso = 2,
                DataVencimento = billToPayDto.DataVencimento,
                DataPagamento = billToPayDto.DataPagamento
            };

            _mockRepository.Setup(repo => repo.CreateBillToPay(It.IsAny<BillToPay>())).Verifiable();

            // Act
            _service.CreateBillToPay(billToPayDto);

            // Assert
            _mockRepository.Verify(repo => repo.CreateBillToPay(It.IsAny<BillToPay>()), Times.AtLeastOnce);
        }

        [Fact]
        public void TestWithValidData_ShouldNotThrowException()
        {
            // Arrange
            var billToPayDto = new CreateBillToPayDto
            {
                Nome = "Conta de Luz",
                ValorOriginal = 100.00f,
                DataVencimento = new DateOnly(2024, 6, 10),
                DataPagamento = new DateOnly(2024, 6, 12)
            };

            // Act & Assert
            _service.ValidateBillToPay(billToPayDto);
        }

        [Fact]
        public void TestWithNullPaymentDateShouldThrowArgumentException()
        {
            // Arrange
            var billToPayDto = new CreateBillToPayDto
            {
                Nome = "Conta de Luz",
                ValorOriginal = 100.00f,
                DataVencimento = new DateOnly(2024, 6, 10),
                DataPagamento = null
            };

            // Act & Assert
            Assert.Throws<Exception>(() => _service.ValidateBillToPay(billToPayDto));
        }

        [Fact]
        public void TestValidateBillToPayShouldThrowExceptionWhenPaymentDateExists()
        {
            // Arrange
            var createBillToPayDto = new CreateBillToPayDto
            {
                Nome = "Exemplo 1",
                ValorOriginal = 100,
                DataVencimento = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-5),
                DataPagamento = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            };

            _mockRepository.Setup(repo => repo.GetBillToPayByPaymentDate(new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day))).Returns(new BillToPay
            {
                Nome = "Exemplo 2",
                ValorOriginal = 100.0f,
                ValorCorrigido = 105.0f,
                QuantidadeDiasAtraso = 5,
                DataVencimento = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                DataPagamento = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            });

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.ValidateBillToPay(createBillToPayDto));
            Assert.Equal("Já existe uma fatura com a mesma data de pagamento.", exception.Message);
        }

        [Fact]
        public void TestCalculateDaysLateShouldReturnCorrectDays()
        {
            // Arrange
            var dataPagamento = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var dataVencimento = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-5);

            // Act
            var result = BillToPayService.CalculateDaysLate(dataPagamento.ToDateTime(TimeOnly.MinValue), dataVencimento.ToDateTime(TimeOnly.MinValue));

            // Assert
            Assert.Equal(5, result);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(3, 0.02f, 0.003f)]
        [InlineData(5, 0.03f, 0.01f)]
        [InlineData(11, 0.05f, 0.033f)]
        public void TestCalculatePenaltyAndInterestShouldReturnCorrectValues(int diasAtraso, float expectedMulta, float expectedJuros)
        {
            // Act
            var (multa, juros) = BillToPayService.CalculatePenaltyAndInterest(diasAtraso);

            // Assert
            const float tolerance = 0.0001f;
            Assert.InRange(multa, expectedMulta - tolerance, expectedMulta + tolerance);
            Assert.InRange(juros, expectedJuros - tolerance, expectedJuros + tolerance);
        }


    }
}
