using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FormatZPD.Dtos;
using FormatZPD.Models;
using FormatZPD.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FormatZPD.Controllers
{
    /// <remarks>
    /// La route principale pour la requête est "api/nodes"
    /// </remarks>
    
    [Route("api/nodes")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly INodeRepo _nodeRepo;
        private readonly IMapper _mapper;

        public NodeController(INodeRepo nodeRepo, IMapper mapper)
        {
            _nodeRepo = nodeRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Une liste de NodeReadDto sous format JSON</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NodeReadDto>>> GetAllNodes()
        {
            try
            {
                var nodes = await _nodeRepo.GetNodes();
                return Ok(_mapper.Map<IEnumerable<NodeReadDto>>(nodes));
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de l'obtention la liste des noeuds");
            }
        }

        /// <summary>
        /// Récupère un noeud à partir de son identifiant
        /// </summary>
        /// <param name="id">Identifiant du noeud</param>
        /// <returns>Un NodeReadDto sous format JSON</returns>

        [HttpGet("{id}")]
        public async Task<ActionResult<NodeReadDto>> GetNodeById(string id)
        {
            try
            {
                var node = await _nodeRepo.GetNodeById(id);
                if (node != null)
                    return Ok(_mapper.Map<NodeReadDto>(node));
                return NotFound("Le noeud n'existe pas");
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de l'obtention du nœud");
            }
        }

        /// <summary>
        /// Récupère les enfants directs du noeud donné en paramètre
        /// </summary>
        /// <param name="parentId">Identifiant du noeud</param>
        /// <returns>Une liste des enfants (NodeReadDto) sous format JSON</returns>
        
        [HttpGet("{parentId}/children")]
        public async Task<ActionResult<IEnumerable<NodeReadDto>>> GetChildrenOfNode(string parentId)
        {
            try
            {
                var parent = await _nodeRepo.GetNodeById(parentId);
                if (parent != null) {
                    var children = await _nodeRepo.GetChildrenOfNode(parent);
                    if (children.Any())
                        return Ok(_mapper.Map<IEnumerable<NodeReadDto>>(children));
                    return NotFound("Le noeud ne possède pas d'enfant");
                }
                return NotFound("Le noeud dont on cherche les enfants n'existe pas");
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de l'obtention des enfants du noeud");
            }
        }

        /// <summary>
        /// Crée un noeud dans l'arbre de connaissances
        /// </summary>
        /// <param name="nodeCreateDto">Le noeud obtenu à partir du NodeCreateDto donné lors de la requête (sous format JSON)</param>
        /// <returns>Un NodeReadDto sous format JSON</returns>

        [HttpPost]
        public async Task<ActionResult<NodeReadDto>> CreateNode([FromBody] NodeCreateDto nodeCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var parentNode = await _nodeRepo.GetNodeById(nodeCreateDto.ParentId);
            if (parentNode == null)
                return NotFound("Pas trouvé le parent de ce nœud");
            if (parentNode.Removed)
                return BadRequest("Le parent a été supprimé");

            try
            {
                var newNode = await _nodeRepo.CreateNode(_mapper.Map<Node>(nodeCreateDto));
                return Ok(_mapper.Map<NodeReadDto>(newNode));
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de la création ce nœud");
            }
        }

        /// <summary>
        /// Modifie les attributs d'un noeud
        /// </summary>
        /// <param name="id">Identifiant du noeud</param>
        /// <param name="nodeUpdateDto">Le noeud obtenu à partir du NodeUpdateDto donné lors de la requête (sous format JSON)</param>
        /// <returns>Un NodeReadDto sous format JSON</returns>

        [HttpPut("{id}")]
        public async Task<ActionResult<NodeReadDto>> UpdateNode(string id, [FromBody] NodeUpdateDto nodeUpdateDto)
        {
            var node = await _nodeRepo.GetNodeById(id);
            if (node == null)
                return NotFound("Le noeud à modifier n'existe pas");
            if (node.Removed)
                return BadRequest("Le noeud à modifier a été supprimé");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            Node newNode = _mapper.Map<Node>(nodeUpdateDto);
            newNode.ParentId = node.Id;

            try
            {
                newNode = await _nodeRepo.EditNode(id, newNode);
                return Ok(_mapper.Map<NodeReadDto>(newNode));
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de la mis à jour ce nœud");
            }
        }

        /// <summary>
        /// Supprime un noeud en passant son attribut "removed" à "true".
        /// Le noeud est toujours stocké dans la base de données.
        /// </summary>
        /// <param name="id">Identifiant du noeud</param>
        /// <returns></returns>

        [HttpDelete("{id}")]
        public async Task<ActionResult<NodeReadDto>> DeleteNode(string id)
        {
            var node = await _nodeRepo.GetNodeById(id);
            if (node == null)
                return NotFound("Le noeud n'existe pas");
            if (node.Type == Node.ROOT)
                return BadRequest("Impossible de supprimer le nœud racine");
            if (node.Removed)
                return BadRequest("Impossible de supprimer ce nœud car il a déjà été supprimé");
            
            try
            {
                var removedNode = await _nodeRepo.DeleteNode(id, node);

                if (removedNode != null)
                    return Ok(_mapper.Map<NodeReadDto>(removedNode));
                return Ok("Le noeud a bien été supprimé");
            }
            catch (Exception e)
            {
                return BadRequest("Erreur lors de la suppression de ce nœud");
            }
        }
    }
}