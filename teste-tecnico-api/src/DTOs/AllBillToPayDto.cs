namespace teste_tecnico_api.src.Dtos
{
    public class AllBillToPayDto
    {
        public required string? Nome { get; set; }
        public required double? ValorOriginal { get; set; }
        public required double? ValorCorrigido { get; set; }
        public required int? QuantidadeDiasAtraso { get; set; }
        public required DateOnly? DataPagamento { get; set; }
    }
}
