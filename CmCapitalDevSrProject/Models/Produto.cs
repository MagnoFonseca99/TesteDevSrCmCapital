using System.ComponentModel.DataAnnotations;
using CmCapitalDevSrProject.Repositories;

namespace CmCapitalDevSrProject.Models;

public class Produto:EntidadeBase
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;
    
    public int CategoriaId { get; set; }
    public CategoriaProduto Categoria { get; set; } = null!;


    [Range(0, double.MaxValue)]
    public decimal Preco { get; set; }

    [Range(0, int.MaxValue)]
    public int QuantidadeEstoque { get; set; }

    public DateTime DataVencimento { get; set; }
}