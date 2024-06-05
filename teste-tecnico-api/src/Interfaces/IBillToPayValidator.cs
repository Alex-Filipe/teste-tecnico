using teste_tecnico_api.src.Dtos;

namespace teste_tecnico_api.src.Interfaces
{
    public interface IBillToPayValidator
    {
        void ValidatePaymentDateAndDueDate(CreateBillToPayDto billToPay);
    }
}
