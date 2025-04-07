namespace CmCapitalDevSrProject.Services.Models.DTOs;

public class RelatorioVendasFiltroDto
{
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }

    public int? ProdutoId { get; set; }
    public int? CategoriaId { get; set; }

    public bool IncluirEstornos { get; set; } = false;

    public string TipoAgrupamento { get; set; } = "mensal"; // ou "anual"
}
