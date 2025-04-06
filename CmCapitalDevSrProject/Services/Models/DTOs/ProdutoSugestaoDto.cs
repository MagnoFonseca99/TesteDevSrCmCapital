namespace CmCapitalDevSrProject.Models.DTOs;

public class ProdutoSugestaoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public DateTime DataVencimento { get; set; }
    public string Categoria { get; set; } = string.Empty;
}