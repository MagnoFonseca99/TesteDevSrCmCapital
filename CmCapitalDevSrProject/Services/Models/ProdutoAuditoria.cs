using CmCapitalDevSrProject.Repositories;

namespace CmCapitalDevSrProject.Models;

public class ProdutoAuditoria : EntidadeBase
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string Operacao { get; set; } = string.Empty; // "CREATE", "UPDATE"
    
    public DateTime DataAlteracao { get; set; } = DateTime.Now;
    public string DadosAnteriores { get; set; } = string.Empty;
    public string DadosNovos { get; set; } = string.Empty;
}