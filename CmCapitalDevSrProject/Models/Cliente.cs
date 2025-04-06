using CmCapitalDevSrProject.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CmCapitalDevSrProject.Models;

using System.ComponentModel.DataAnnotations;

public class Cliente: EntidadeBase
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;
    
    [Precision(18, 2)]
    [Range(0, double.MaxValue, ErrorMessage = "O saldo não pode ser negativo.")]
    public decimal SaldoDisponivel { get; set; }
}