using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using teste_tecnico_api.src.Dtos;
using teste_tecnico_api.src.Interfaces;
using teste_tecnico_api.src.Services.Validations;

namespace teste_tecnico_api.src.Services
{
    public class BillToPayService(IBillToPayRepository billToPayRepository, IBillToPayValidator billToPayValidator)
    {
        private readonly IBillToPayRepository _billToPayRepository = billToPayRepository;

        private readonly IBillToPayValidator _billToPayValidator = billToPayValidator;

        public List<AllBillToPayDto> GetAllBillsToPay()
        {
            return _billToPayRepository.GetAllBillsToPay();
        }

        public void CreateBillToPay(CreateBillToPayDto billToPay)
        {
            try
            {
                _billToPayValidator.ValidatePaymentDateAndDueDate(billToPay);

                var diasAtraso = CalculateDaysLate(billToPay.DataPagamento?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue, billToPay.DataVencimento?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue);

                var (multa, juros) = CalculatePenaltyAndInterest(diasAtraso);

                var valorCorrigido = billToPay.ValorOriginal + (billToPay.ValorOriginal * multa) + (billToPay.ValorOriginal * juros);

                var newBillToPay = new Models.BillToPay
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
            catch (ValidationException ex)
            {
                throw new Exception("Erro de validação ao criar a conta a pagar: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao criar a conta a pagar: " + ex.Message);
            }
        }


        public int CalculateDaysLate(DateTime dataPagamento, DateTime dataVencimento)
        {
            return (dataPagamento - dataVencimento).Days;
        }

        public (float multa, float juros) CalculatePenaltyAndInterest(int diasAtraso)
        {
            const float MULTA_ATRASO_ATE_3_DIAS = 0.02f;
            const float MULTA_ATRASO_ATE_10_DIAS = 0.03f;
            const float MULTA_ATRASO_ACIMA_10_DIAS = 0.05f;

            const float JUROS_ATRASO_ATE_3_DIAS = 0.001f;
            const float JUROS_ATRASO_ATE_10_DIAS = 0.002f;
            const float JUROS_ATRASO_ACIMA_10_DIAS = 0.003f;

            if (diasAtraso <= 0) return (0, 0);

            var (multa, jurosDiario) = diasAtraso switch
            {
                <= 3 => (MULTA_ATRASO_ATE_3_DIAS, JUROS_ATRASO_ATE_3_DIAS),
                <= 10 => (MULTA_ATRASO_ATE_10_DIAS, JUROS_ATRASO_ATE_10_DIAS),
                _ => (MULTA_ATRASO_ACIMA_10_DIAS, JUROS_ATRASO_ACIMA_10_DIAS)
            };

            float juros = jurosDiario * diasAtraso;
            return (multa, juros);
        }
    }
}
