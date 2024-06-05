

using teste_tecnico_api.src.Database;
using teste_tecnico_api.src.Dtos;
using teste_tecnico_api.src.Interfaces;
using teste_tecnico_api.src.Models;

namespace teste_tecnico_api.src.Repositories
{
    public class BillToPayRepository(ApplicationDbContext context) : IBillToPayRepository
    {
        private readonly ApplicationDbContext _context = context;

        public List<AllBillToPayDto> GetAllBillsToPay()
        {
            return [.. _context.BillsToPay
                           .Select(
                                 (billToPay) => new AllBillToPayDto
                                 {
                                     Nome = billToPay.Nome,
                                     ValorOriginal = billToPay.ValorOriginal,
                                     ValorCorrigido = billToPay.ValorCorrigido,
                                     QuantidadeDiasAtraso = billToPay.QuantidadeDiasAtraso,
                                     DataPagamento = billToPay.DataPagamento
                                 })];
        }

        public void CreateBillToPay(BillToPay newBillToPay)
        {
            _context.BillsToPay.Add(newBillToPay);
            _context.SaveChanges();
        }

        public BillToPay? GetBillToPayByPaymentDate(DateOnly paymentDate)
        {
            return _context.BillsToPay.SingleOrDefault(billsToPay => billsToPay.DataPagamento == paymentDate);
        }
    }
}