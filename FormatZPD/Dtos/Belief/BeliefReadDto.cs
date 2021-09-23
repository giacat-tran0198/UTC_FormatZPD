using System.Collections.Generic;

namespace FormatZPD.Dtos.Belief
{
    /// <summary>
    /// BeliefReadDto indique tous les attributs que la requête de l'API va renvoyer
    /// </summary>
    public class BeliefReadDto
    {
        public string Id { get; set; }

        public float Hability { get; set; }

        public float Dishability { get; set; }

        public float Ignorance { get; set; }

        public float Conflict { get; set; }

        public string PersonId { get; set; }
        
        public string ColorBelief {get; set; }

        public List<InteractionBeliefReadDto> Interactions { get; set; }
    }
}