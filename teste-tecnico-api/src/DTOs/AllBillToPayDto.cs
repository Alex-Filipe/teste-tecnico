namespace teste_tecnico_api.src.Dtos
{
    public class AllUserDto
    {
        public required string? Nome { get; set; }
        public required float? ValorOriginal { get; set; }
        public required float? ValorCorrigido { get; set; }
        public required int? QuantidadeDiasAtraso { get; set; }
        public required DateOnly? DataPagamento { get; set; }
    }
}
