using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace teste_tecnico_api.src.Models
{
    [Table("bills_to_pay")]
    public class BillToPay
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("name")]
        public required string? Nome { get; set; }

        [Column("original_value")]
        public required float? ValorOriginal { get; set; }

        [Column("corrected_value")]
        public required float? ValorCorrigido { get; set; }

        [Column("number_days_late")]
        public required int? QuantidadeDiasAtraso { get; set; }

        [Column("due_date")]
        public required DateOnly? DataVencimento { get; set; }

        [Column("payday")]
        public required DateOnly? DataPagamento { get; set; }
    }
}