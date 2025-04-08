namespace CmCapitalDevSrProject.Models.DTOs;

public class VendaResultadoDto
{
    public int VendaId { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Produto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
}