namespace CmCapitalDevSrProject.Models;

public class InteresseProdutoFuturo
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public int? CategoriaId { get; set; } // agora opcional
    public CategoriaProduto? Categoria { get; set; }

    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
}