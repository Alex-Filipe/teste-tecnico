using teste_tecnico_api.src.Dtos;
using teste_tecnico_api.src.Interfaces;
using teste_tecnico_api.src.Models;


namespace teste_tecnico_api.src.Services
{
    public class BillToPayService(IBillToPayRepository billToPayRepository)
    {
        private readonly IBillToPayRepository _billToPayRepository = billToPayRepository;

        public List<AllUserDto> GetAllBillsToPay()
        {
            try
            {
                return _billToPayRepository.GetAllBillsToPay();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void CreateBillToPay(CreateBillToPayDto billToPay)
        {
            try
            {
                ValidateBillToPay(billToPay);

                var diasAtraso = CalculateDaysLate(billToPay.DataPagamento?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue, billToPay.DataVencimento?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue);
                
                var (multa, juros) = CalculatePenaltyAndInterest(diasAtraso);

                var valorCorrigido = billToPay.ValorOriginal + (billToPay.ValorOriginal * multa) + (billToPay.ValorOriginal * juros);

                var newBillToPay = new BillToPay
                {
                    Nome = billToPay.Nome,
                    ValorOriginal = billToPay.ValorOriginal,
                    ValorCorrigido = valorCorrigido,
                    QuantidadeDiasAtraso = diasAtraso,
                    DataVencimento = billToPay.DataVencimento,
                    DataPagamento = billToPay.DataPagamento
                };

                _billToPayRepository.CreateBillToPay(newBillToPay);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao criar a conta a pagar: " + ex.Message);
            }
        }

        private void ValidateBillToPay(CreateBillToPayDto billToPay)
        {
            if (!billToPay.DataPagamento.HasValue || !billToPay.DataVencimento.HasValue)
            {
                throw new ArgumentException("A data de pagamento e/ou vencimento não pode ser nula.");
            }

            var existingBill = _billToPayRepository.GetBillToPayByPaymentDate(billToPay.DataPagamento.Value);
            if (existingBill != null)
            {
                throw new InvalidOperationException("Já existe uma fatura com a mesma data de pagamento.");
            }
        }

        private static int CalculateDaysLate(DateTime dataPagamento, DateTime dataVencimento)
        {
            return (dataPagamento - dataVencimento).Days;
        }

        private static (float multa, float juros) CalculatePenaltyAndInterest(int diasAtraso)
        {
            const float MULTA_ATRASO_ATE_3_DIAS = 0.02f;
            const float MULTA_ATRASO_ATE_10_DIAS = 0.03f;
            const float MULTA_ATRASO_ACIMA_10_DIAS = 0.05f;

            const float JUROS_ATRASO_ATE_3_DIAS = 0.001f;
            const float JUROS_ATRASO_ATE_10_DIAS = 0.002f;
            const float JUROS_ATRASO_ACIMA_10_DIAS = 0.003f;

            if (diasAtraso <= 0) return (0, 0);

            if (diasAtraso <= 3)
            {
                return (MULTA_ATRASO_ATE_3_DIAS, JUROS_ATRASO_ATE_3_DIAS * diasAtraso);
            }
            else if (diasAtraso <= 10)
            {
                return (MULTA_ATRASO_ATE_10_DIAS, JUROS_ATRASO_ATE_10_DIAS * diasAtraso);
            }
            else
            {
                return (MULTA_ATRASO_ACIMA_10_DIAS, JUROS_ATRASO_ACIMA_10_DIAS * diasAtraso);
            }
        }
    }
}