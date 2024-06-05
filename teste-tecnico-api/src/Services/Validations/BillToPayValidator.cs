using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using teste_tecnico_api.src.Dtos;
using teste_tecnico_api.src.Interfaces;

namespace teste_tecnico_api.src.Services.Validations
{
    public class BillToPayValidator(IBillToPayRepository billToPayRepository) : IBillToPayValidator
    {
        private readonly IBillToPayRepository _billToPayRepository = billToPayRepository;
        public void ValidatePaymentDateAndDueDate(CreateBillToPayDto billToPay)
        {
            if (!billToPay.DataPagamento.HasValue || !billToPay.DataVencimento.HasValue)
            {
                throw new ValidationException("A data de pagamento e/ou vencimento não pode ser nula.");
            }

            var existingBill = _billToPayRepository.GetBillToPayByPaymentDate(billToPay.DataPagamento.Value);
            if (existingBill != null)
            {
                throw new ValidationException("Já existe uma fatura com a mesma data de pagamento.");
            }
        }
    }
}