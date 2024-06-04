using System.ComponentModel.DataAnnotations;

namespace teste_tecnico_api.src.Dtos
{
    public class CreateBillToPayDto
    {
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public required string? Nome { get; set; }

        [Required(ErrorMessage = "O campo Valor Original é obrigatório.")]
        public required float? ValorOriginal { get; set; }

        [Required(ErrorMessage = "O campo Data Vencimento é obrigatório.")]
        public required DateOnly? DataVencimento { get; set; }

        [Required(ErrorMessage = "O campo Data Pagamento é obrigatório.")]
        public required DateOnly? DataPagamento { get; set; }
    }
}
