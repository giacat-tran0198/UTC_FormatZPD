using AutoMapper;
using FormatZPD.Dtos.Person;
using FormatZPD.Models;

namespace FormatZPD.Profiles
{
    /// <summary>
    /// Les profiles permettent de faire du mapping simple rapide et efficace grâce au package Automapper.
    /// Il est nécessaire de déclarer tous les mappings de Person ici.
    /// </summary>
    public class PeopleProfile : Profile
    {
        public PeopleProfile()
        {
            CreateMap<Person, PersonReadDto>();
            CreateMap<PersonWriteDto, Person>();
        }
    }
}