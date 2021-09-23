using System.ComponentModel.DataAnnotations;

namespace FormatZPD.Dtos.Person
{
    /// <summary>
    /// BeliefWriteDto indique tous les attributs à spécifier pour créer une personne au travers de la requête de l'API
    /// </summary>
    public class PersonWriteDto
    {
        [Required(ErrorMessage = "Le prénom est obligatoire")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        public string LastName { get; set; }
    }
}