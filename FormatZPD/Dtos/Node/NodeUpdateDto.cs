using System.ComponentModel.DataAnnotations;
using FormatZPD.Models;
using FormatZPD.Validations;

namespace FormatZPD.Dtos
{
    /// <summary>
    /// NodeUpdateDto indique tous les attributs à spécifier pour modifier un noeud au travers de la requête de l'API
    /// </summary>
    public class NodeUpdateDto
    {
        [Required(ErrorMessage="Le titre est obligatoire")] public string Title { get; set; }

        [Required(ErrorMessage="Le type est obligatoire"), MatchInArrayString(new[]
        {
            Node.KNOWLEDGE, Node.DIDACTIC
        })]
        public string Type { get; set; }
    }
}