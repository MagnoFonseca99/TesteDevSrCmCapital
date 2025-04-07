using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services.Models.DTOs;

namespace CmCapitalDevSrProject.Services.Interfaces;

public interface IInteresseProdutoService
{
    Task RegistrarInteresseAsync(InteresseProdutoFuturoDto dto);
}