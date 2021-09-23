using AutoMapper;
using FormatZPD.Dtos;
using FormatZPD.Models;

namespace FormatZPD.Profiles
{
    /// <summary>
    /// Les profiles permettent de faire du mapping simple rapide et efficace grâce au package Automapper.
    /// Il est nécessaire de déclarer tous les mappings de Node ici.
    /// </summary>
    public class NodesProfile : Profile
    {
        public NodesProfile()
        {
            //NodesProfile déclare tous les types de mapping automatiques possibles
            CreateMap<Node, NodeReadDto>();
            CreateMap<NodeCreateDto, Node>();
            CreateMap<NodeUpdateDto, Node>();
        }
    }
}
