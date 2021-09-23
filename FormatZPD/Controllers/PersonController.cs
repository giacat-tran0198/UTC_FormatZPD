using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FormatZPD.Dtos.Person;
using FormatZPD.Models;
using FormatZPD.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FormatZPD.Controllers
{
    /// <remarks>
    /// La route principale pour la requête est "api/people"
    /// </remarks>
    [Route("api/people")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonRepo _personRepo;
        private readonly IMapper _mapper;

        public PersonController(IPersonRepo personRepo, IMapper mapper)
        {
            _personRepo = personRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Récupère toutes les personnes existantes
        /// </summary>
        /// <returns>Une liste de PersonReadDto sous format JSON</returns>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonReadDto>>> GetPeople()
        {
            try
            {
                var people = await _personRepo.GetPeople();
                return Ok(_mapper.Map<IEnumerable<PersonReadDto>>(people));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Récupère une personne à partir de son identifiant
        /// </summary>
        /// <param name="id">Identifiant de la personne</param>
        /// <returns>Un PersonReadDto sous format JSON</returns>

        [HttpGet("{id}")]
        public async Task<ActionResult<PersonReadDto>> GetPersonById(string id)
        {
            try
            {
                var person = await _personRepo.GetPersonById(id);
                if (person != null)
                    return Ok(_mapper.Map<PersonReadDto>(person));
                return NotFound("La personne n'existe pas");
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de l'obtention de la personne");
            }
        }

        /// <summary>
        /// Crée une personne
        /// </summary>
        /// <param name="personWriteDto">La personne obtenue à partir du PersonWriteDto donné lors de la requête</param>
        /// <returns>Un PersonReadDto sous format JSON</returns>

        [HttpPost]
        public async Task<ActionResult<PersonReadDto>> CreatePerson([FromBody] PersonWriteDto personWriteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newPerson = await _personRepo.CreatePerson(_mapper.Map<Person>(personWriteDto));
                return Ok(_mapper.Map<PersonReadDto>(newPerson));
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de la création cette personne");
            }
        }

        /// <summary>
        /// Modifie les attributs d'une personne
        /// </summary>
        /// <param name="id">Identifiant de la personne</param>
        /// <param name="personWriteDto">La personne obtenue à partir du PersonWriteDto donné lors de la requête</param>
        /// <returns>Un PersonReadDto sous format JSON</returns>

        [HttpPut("{id}")]
        public async Task<ActionResult<PersonReadDto>> UpdatePerson(string id, [FromBody] PersonWriteDto personWriteDto)
        {
            var person = await _personRepo.GetPersonById(id);
            if (person == null)
                return NotFound("La personne à modifier n'existe pas");
            if (person.Removed)
                return BadRequest("La personne à modifier a été supprimé");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            Person newPerson = _mapper.Map<Person>(personWriteDto);

            try
            {
                newPerson = await _personRepo.EditPerson(id, newPerson);
                return Ok(_mapper.Map<PersonReadDto>(newPerson));
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de la mis à jour cette personne");
            }
        }

        /// <summary>
        /// Supprime une personne en faisant passer son attribut "removed" à "true"
        /// </summary>
        /// <param name="id">Identifiant de la personne</param>
        /// <returns>Un PersonReadDto sous format JSON</returns>

        [HttpDelete("{id}")]
        public async Task<ActionResult<PersonReadDto>> DeletePerson(string id)
        {
            var person = await _personRepo.GetPersonById(id);
            if (person == null)
                return NotFound("La personne n'existe pas");
            if (person.Removed)
                return BadRequest("Impossible de supprimer cette personne car elle a déjà été supprimé");

            try
            {
                var removedPerson = await _personRepo.DeletePerson(id, person);

                if (removedPerson != null)
                    return Ok(_mapper.Map<PersonReadDto>(removedPerson));
                return Ok("La personne a bien été supprimé");
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de la suppression de cette personne");
            }
        }
    }
}