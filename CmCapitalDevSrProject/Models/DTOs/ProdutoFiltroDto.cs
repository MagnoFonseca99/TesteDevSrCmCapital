namespace CmCapitalDevSrProject.Models.DTOs;

public class ProdutoFiltroDto
{
    public decimal? PrecoMin { get; set; }
    public decimal? PrecoMax { get; set; }
    public int? QuantidadeMin { get; set; }
    public int? QuantidadeMax { get; set; }
    public DateTime? VencimentoAte { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}