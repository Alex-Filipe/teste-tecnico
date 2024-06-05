using Xunit;
using Moq;
using teste_tecnico_api.src.Dtos;
using teste_tecnico_api.src.Interfaces;
using teste_tecnico_api.src.Services;
using teste_tecnico_api.src.Models;
using teste_tecnico_api.src.Services.Validations;

namespace teste_tecnico_api.tests.Services
{
    public class BillToPayServiceTest
    {
        private readonly Mock<IBillToPayRepository> _mockRepository;
        private readonly Mock<IBillToPayValidator> _mockValidator;
        private readonly BillToPayService _service;

        public BillToPayServiceTest()
        {
            _mockRepository = new Mock<IBillToPayRepository>();
            _mockValidator = new Mock<IBillToPayValidator>();
            _service = new BillToPayService(_mockRepository.Object, _mockValidator.Object);
        }

        [Fact]
        public void GetAllBillsToPayShouldReturnAllBillsToPay()
        {
            // Arrange
            var bills = new List<AllBillToPayDto>
        {
            new() {
                Nome = "Conta de Luz",
                ValorOriginal = 100.0,
                ValorCorrigido = 105.0,
                QuantidadeDiasAtraso = 5,
                DataPagamento = new DateOnly(2024, 5, 30)
            },
            new() {
                Nome = "Conta de Água",
                ValorOriginal = 50.0,
                ValorCorrigido = 52.0,
                QuantidadeDiasAtraso = 2,
                DataPagamento = new DateOnly(2024, 6, 1)
            }
        };

            _mockRepository.Setup(repo => repo.GetAllBillsToPay()).Returns(bills);

            // Act
            var result = _service.GetAllBillsToPay();

            // Assert
            Assert.Equal(bills, result);
        }

        [Fact]
        public void CreateBillToPayShouldCallRepositoryCreateWhenValid()
        {
            // Arrange
            var dto = new CreateBillToPayDto
            {
                Nome = "Conta de Internet",
                ValorOriginal = 75.0,
                DataVencimento = new DateOnly(2024, 5, 25),
                DataPagamento = new DateOnly(2024, 5, 30)
            };

            _mockValidator.Setup(val => val.ValidatePaymentDateAndDueDate(dto));

            // Act
            _service.CreateBillToPay(dto);

            // Assert
            _mockRepository.Verify(repo => repo.CreateBillToPay(It.IsAny<BillToPay>()), Times.Once);
        }

        [Theory]
        [InlineData("2024-05-30", "2024-05-30", 0)]
        [InlineData("2024-05-29", "2024-05-30", -1)]
        [InlineData("2024-05-31", "2024-05-30", 1)]
        [InlineData("2024-06-01", "2024-05-30", 2)]
        public void CalculateDaysLateShouldReturnCorrectDaysLate(string dataPagamentoStr, string dataVencimentoStr, int expectedDaysLate)
        {
            // Arrange
            var dataPagamento = DateTime.Parse(dataPagamentoStr);
            var dataVencimento = DateTime.Parse(dataVencimentoStr);

            // Act
            var result = _service.CalculateDaysLate(dataPagamento, dataVencimento);

            // Assert
            Assert.Equal(expectedDaysLate, result);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0.02f, 0.001f)]
        [InlineData(3, 0.02f, 0.003f)]
        [InlineData(4, 0.03f, 0.008f)]
        [InlineData(10, 0.03f, 0.02f)]
        [InlineData(11, 0.05f, 0.033f)]
        public void CalculatePenaltyAndInterestShouldReturnCorrectValues(int diasAtraso, float expectedMulta, float expectedJuros)
        {
            // Act
            var (multa, juros) = _service.CalculatePenaltyAndInterest(diasAtraso);

            // Assert
            const float tolerance = 0.0001f;
            Assert.Equal(expectedMulta, multa, tolerance);
            Assert.Equal(expectedJuros, juros, tolerance);
        }
    }
}
