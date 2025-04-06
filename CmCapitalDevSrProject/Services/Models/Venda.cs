namespace CmCapitalDevSrProject.Models;

public class Venda
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    
    public Cliente Cliente { get; set; } = null!;
    public int ProdutoId { get; set; }
    
    public Produto Produto { get; set; } = null!;
    public int Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataVenda { get; set; } = DateTime.Now;
}