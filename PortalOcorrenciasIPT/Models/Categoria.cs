using System.ComponentModel.DataAnnotations;

namespace PortalOcorrenciasIPT.Models;

// Representa uma categoria usada para classificar ocorrências,
// por exemplo Limpeza, Segurança, Iluminação ou Equipamentos.
public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
    [StringLength(80, ErrorMessage = "O nome da categoria não pode exceder 80 caracteres.")]
    public string Nome { get; set; } = "";

    [StringLength(250, ErrorMessage = "A descrição não pode exceder 250 caracteres.")]
    public string? Descricao { get; set; }

    // Permite desativar uma categoria sem a remover da base de dados,
    // preservando as ocorrências que já estejam associadas a ela.
    public bool Ativa { get; set; } = true;

    // Relação um-para-muitos: uma categoria pode ter várias ocorrências.
    public List<Ocorrencia> Ocorrencias { get; set; } = new();
}