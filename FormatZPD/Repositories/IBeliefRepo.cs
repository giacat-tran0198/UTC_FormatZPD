using System.Collections.Generic;
using System.Threading.Tasks;
using FormatZPD.Models;

namespace FormatZPD.Repositories
{
    public interface IBeliefRepo
    {
        public Task<IEnumerable<Belief>> GetBelievesOfPerson(string personId);
        public Task<Belief> GetBeliefById(string id);
        public Task<IEnumerable<Belief>> GetBeliefOfPersonAndDidactic(string personId, Node node);
        public Task<Belief> GetBeliefFromParent(string personId, string parent1Id, string parent2Id);
        public Task<Belief> CreateBelief(Belief belief);
        public Task<string> DeleteBelief(string id);
    }
}