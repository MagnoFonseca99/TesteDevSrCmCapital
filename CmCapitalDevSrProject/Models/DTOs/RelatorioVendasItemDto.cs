namespace CmCapitalDevSrProject.Services.Models.DTOs;

public class RelatorioVendasItemDto
{
    public string Produto { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public int QuantidadeTotal { get; set; }
    public decimal ValorTotal { get; set; }
}