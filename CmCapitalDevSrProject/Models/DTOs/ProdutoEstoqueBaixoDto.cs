namespace CmCapitalDevSrProject.Models.DTOs;

public class ProdutoEstoqueBaixoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int QuantidadeEstoque { get; set; }
}
