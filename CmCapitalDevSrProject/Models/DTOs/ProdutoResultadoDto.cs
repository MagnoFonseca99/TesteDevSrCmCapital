namespace CmCapitalDevSrProject.Models.DTOs;

public class ProdutoResultadoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int QuantidadeEstoque { get; set; }
    public DateTime DataVencimento { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
}
