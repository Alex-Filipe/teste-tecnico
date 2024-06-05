using teste_tecnico_api.src.Dtos;
using teste_tecnico_api.src.Models;

namespace teste_tecnico_api.src.Interfaces
{
    public interface IBillToPayRepository
    {
        List<AllBillToPayDto> GetAllBillsToPay();

        void CreateBillToPay(BillToPay newBillToPay);

        BillToPay? GetBillToPayByPaymentDate(DateOnly paymentDate);

    }
}