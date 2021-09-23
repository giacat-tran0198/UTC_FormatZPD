using System.ComponentModel.DataAnnotations;
using FormatZPD.Models;
using FormatZPD.Validations;

namespace FormatZPD.Dtos

{
    /// <summary>
    /// NodeCreateDto indique tous les attributs à spécifier pour créer un noeud au travers de la requête de l'API
    /// </summary>
    public class NodeCreateDto
    {
        [Required(ErrorMessage = "Le titre est obligatoire")] public string Title { get; set; }

        [Required(ErrorMessage= "Le type est obligatoire"), MatchInArrayString(new[]
        {
            Node.KNOWLEDGE, Node.DIDACTIC
        })]
        public string Type { get; set; }

        [Required(ErrorMessage = "Son parent est obligatoire")] public string ParentId { get; set; }
    }
}