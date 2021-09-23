using System.ComponentModel.DataAnnotations;
using FormatZPD.Models;
using FormatZPD.Validations;

namespace FormatZPD.Dtos.Belief
{
    public class InteractionBeliefWriteDto
    {
        [Required(ErrorMessage = "NodeId est obligatoire")]
        public string NodeId { get; set; }

        [Required(ErrorMessage = "Level est obligatoire"), MatchInArrayString(new[]
        {
            InteractionBelief.WEAK, InteractionBelief.MEDIUM, InteractionBelief.STRONG
        })]
        public string Level { get; set; }
    }
}