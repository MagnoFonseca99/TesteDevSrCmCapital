namespace CmCapitalDevSrProject.Repositories;

public class EntidadeBase
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}