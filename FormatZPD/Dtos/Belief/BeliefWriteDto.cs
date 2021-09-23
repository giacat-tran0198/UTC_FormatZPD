using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormatZPD.Dtos.Belief
{
    /// <summary>
    /// BeliefWriteDto indique tous les attributs à spécifier pour créer un belief au travers de la requête de l'API
    /// </summary>
    public class BeliefWriteDto
    {
        public float Hability { get; set; }
        
        public float Dishability { get; set; }
        
        public float Ignorance { get; set; }
        
        public float Conflict { get; set; }
        
        public List<InteractionBeliefWriteDto> Interactions { get; set; }

        public BeliefWriteDto()
        {
            Interactions = new List<InteractionBeliefWriteDto>();
        }
    }
}