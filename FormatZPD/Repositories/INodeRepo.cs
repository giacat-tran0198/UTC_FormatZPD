using System.Collections.Generic;
using System.Threading.Tasks;
using FormatZPD.Models;

namespace FormatZPD.Repositories
{
    public interface INodeRepo
    {
        public Task<IEnumerable<Node>> GetNodes();
        public Task<Node> GetNodeById(string id);
        public Task<IEnumerable<Node>> GetChildrenOfNode(Node node);
        public Task<Node> CreateNode(Node node);
        public Task<Node> EditNode(string id, Node node);
        public Task<Node> DeleteNode(string id, Node node);
    }
}