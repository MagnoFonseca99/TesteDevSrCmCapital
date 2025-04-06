namespace CmCapitalDevSrProject.Models.DTOs;

public class ProdutoDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    
    public int CategoriaId { get; set; }
    public int QuantidadeEstoque { get; set; }
    public DateTime DataVencimento { get; set; }
}