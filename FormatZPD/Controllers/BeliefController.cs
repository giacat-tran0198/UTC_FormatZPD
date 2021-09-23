using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FormatZPD.Dtos.Belief;
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
    public class BeliefController : ControllerBase
    {
        private readonly IPersonRepo _personRepo;
        private readonly INodeRepo _nodeRepo;
        private readonly IBeliefRepo _beliefRepo;
        private readonly IMapper _mapper;

        public BeliefController(IPersonRepo personRepo, INodeRepo nodeRepo, IBeliefRepo beliefRepo, IMapper mapper)
        {
            _personRepo = personRepo;
            _nodeRepo = nodeRepo;
            _beliefRepo = beliefRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Récupère tous les Belief d'une personne
        /// </summary>
        /// <param name="personId">Identifiant de la personne possédant le belief</param>
        /// <returns>Un BeliefReadDto sous format JSON</returns>

        [HttpGet("{personId}/believes")]
        public async Task<ActionResult<BeliefReadDto>> GetBelievesOfPerson(string personId)
        {
            try
            {
                var person = await _personRepo.GetPersonById(personId);
                if (person == null)
                    return NotFound("La personne n'existe pas");
                var believes = await _beliefRepo.GetBelievesOfPerson(personId);
                return Ok(_mapper.Map<IEnumerable<BeliefReadDto>>(believes));
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de l'obtention des believes de la personne");
            }
        }

        /// <summary>
        /// Récupère un Belief résultant de la fusion de tous les Belief ayant pour parent direct ou lointain les parents spécifiés en paramètre
        /// </summary>
        /// <param name="personId">Identifiant de la personne</param>
        /// <param name="parent1Id">Identifiant du premier parent</param>
        /// <param name="parent2Id">Identifiant du deuxième parent</param>
        /// <returns>Un BeliefReadDto sous format JSON</returns>


        [HttpGet("{personId}/believes/parent/{parent1Id}/{parent2Id}")]
        public async Task<ActionResult<BeliefReadDto>> GetParentBeliefOfPerson(string personId, string parent1Id, string parent2Id)
        {
            try
            {
                var person = await _personRepo.GetPersonById(personId);
                if (person == null)
                    return NotFound("La personne n'existe pas");

                var belief = await _beliefRepo.GetBeliefFromParent(personId, parent1Id, parent2Id);
                return Ok(_mapper.Map<BeliefReadDto>(belief));
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de l'obtention des believes de la personne avec ces noeuds parents");
            }
        }

        /// <summary>
        /// Crée un Belief pour une personne donnée
        /// </summary>
        /// <param name="personId">Identifiant de la personne</param>
        /// <param name="belief">Le Belief obtenu à partir du BeliefWriteDto donné lors de la requête (sous format JSON)</param>
        /// <returns></returns>

        [HttpPost("{personId}/believes")]
        public async Task<ActionResult<BeliefReadDto>> CreateBelievesOfPerson(string personId,
            [FromBody] BeliefWriteDto belief)
        {
            if (Math.Abs(belief.Hability + belief.Dishability + belief.Conflict + belief.Ignorance - 1) != 0)
            {
                ModelState.AddModelError("Somme", "La somme de toutes les variables doit faire 1");
            }

            if (belief.Interactions.Count != 2)
            {
                ModelState.AddModelError("Interactions", "La liste d'interaction doit avoir 2 interactions ");
            }

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            Belief newBelief = _mapper.Map<Belief>(belief);
            newBelief.PersonId = personId;

            try
            {
                var person = await _personRepo.GetPersonById(newBelief.PersonId);
                if (person == null)
                    return NotFound("La personne avec ID=" + newBelief.PersonId + " n'existe pas");
                if (person.Removed)
                    return BadRequest("La personne avec ID=" + newBelief.PersonId + " a été supprimé");

                var node = await _nodeRepo.GetNodeById(newBelief.Interactions[0].NodeId);
                if (node == null)
                    return NotFound("Le noeud avec ID='" + belief.Interactions[0].NodeId + "' n'existe pas");
                if (node.Removed)
                    return BadRequest("Le noeud avec ID='" + belief.Interactions[0].NodeId + " a été supprimé");

                node = await _nodeRepo.GetNodeById(newBelief.Interactions[1].NodeId);
                if (node == null)
                    return NotFound("Le noeud avec ID='" + belief.Interactions[1].NodeId + "' n'existe pas");
                if (node.Removed)
                    return BadRequest("Le noeud avec ID='" + belief.Interactions[1].NodeId + " a été supprimé");

                newBelief = await _beliefRepo.CreateBelief(newBelief);

                return Ok(_mapper.Map<BeliefReadDto>(newBelief));
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de la création des believes de la personne");
            }
        }

        /// <summary>
        /// Récupère le belief à partir de l'identifiant procuré
        /// </summary>
        /// <param name="beliefId">Identifiant du Belief</param>
        /// <returns></returns>

        [HttpGet("believes/{beliefId}")]
        public async Task<ActionResult<BeliefReadDto>> GetBeliefById(string beliefId)
        {
            try
            {
                var belief = await _beliefRepo.GetBeliefById(beliefId);
                return Ok(_mapper.Map<BeliefReadDto>(belief));
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de l'obtention de la croyance");
            }
        }

        /// <summary>
        /// Supprime un Belief et toute interaction entre lui et ses parents
        /// </summary>
        /// <param name="beliefId">Identifiant du Belief</param>
        /// <returns></returns>
        
        [HttpDelete("believes/{beliefId}")]
        public async Task<ActionResult> DeleteBelief(string beliefId)
        {
            try
            {
                var belief = await _beliefRepo.GetBeliefById(beliefId);
                if (belief == null)
                    return NotFound("Le belief n'existe pas");

                var id = await _beliefRepo.DeleteBelief(beliefId);
                if (id != null)
                {
                    return Ok("Le belief a bien été supprimée");    
                }

                return BadRequest("Le belief n'a pas été supprimée");

            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de la suppression du belief");
            }
        }
    }
}