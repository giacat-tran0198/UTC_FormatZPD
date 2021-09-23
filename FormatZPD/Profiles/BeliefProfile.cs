using AutoMapper;
using FormatZPD.Dtos.Belief;
using FormatZPD.Models;

namespace FormatZPD.Profiles
{
    /// <summary>
    /// Les profiles permettent de faire du mapping simple rapide et efficace grâce au package Automapper.
    /// Il est nécessaire de déclarer tous les mappings de Beliefs et d'InteractionBelief ici.
    /// </summary>
    public class BeliefProfile : Profile
    {
        public BeliefProfile()
        {
            CreateMap<Belief, BeliefReadDto>();
            CreateMap<InteractionBelief, InteractionBeliefReadDto>();

            CreateMap<InteractionBeliefWriteDto, InteractionBelief>();
            CreateMap<BeliefWriteDto, Belief>();
        }
    }
}