using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FormatZPD.Models;
using Neo4j.Driver;

namespace FormatZPD.Repositories
{
    public class NodeRepo : INodeRepo
    {
        private readonly IDriver _driver;

        public NodeRepo(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<IEnumerable<Node>> GetNodes()
        {
            IResultCursor cursor;
            IAsyncSession session = _driver.AsyncSession();
            List<Node> nodes = new List<Node>();
            try
            {
                cursor = await session.RunAsync(@"
MATCH (n:Node) 
OPTIONAL MATCH (parent:Node)-[:contains]->(n) 
RETURN n.id AS id, n.title AS title, n.type AS type, n.removed AS removed, parent.id AS parentId
");
                nodes = await cursor.ToListAsync(record =>
                {
                    Node node = new Node()
                    {
                        Id = record["id"].As<string>(),
                        ParentId = record["parentId"]?.As<string>() ?? "-1",
                        Title = record["title"].As<string>(),
                        Type = record["type"].As<string>(),
                        Removed = record["removed"].As<bool>(),
                    };

                    return node;
                });
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await session.CloseAsync();
            }

            return nodes;
        }

        public async Task<Node> GetNodeById(string id)
        {
            IResultCursor cursor;
            IAsyncSession session = _driver.AsyncSession();
            Node node = null;
            try
            {
                cursor = await session.RunAsync(@"
MATCH (n:Node {id: $id}) 
OPTIONAL MATCH (parent:Node)-[:contains]->(n) 
RETURN n.id AS id, n.title AS title, n.type AS type, n.removed AS removed, parent.id AS parentId
", new {id});
                IRecord record = await cursor.SingleAsync();
                if (record != null)
                {
                    node = new Node()
                    {
                        Id = record["id"].As<string>(),
                        ParentId = record["parentId"]?.As<string>() ?? "-1",
                        Title = record["title"].As<string>(),
                        Type = record["type"].As<string>(),
                        Removed = record["removed"].As<bool>(),
                    };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await session.CloseAsync();
            }

            return node;
        }

        public async Task<IEnumerable<Node>> GetChildrenOfNode(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            IResultCursor cursor;
            IAsyncSession session = _driver.AsyncSession();
            List<Node> children = new List<Node>();
            try
            {
                var parameters = new
                {
                    newNode = node.AsDictionary()
                };

                cursor = await session.RunAsync(@"
MATCH (n:Node {id: $newNode.id})-[:contains]->(children)
WHERE NOT children.removed = true
RETURN children.id AS childrenId, children.title AS title, children.type AS type, children.removed AS removed
", parameters);
                children = await cursor.ToListAsync(record =>
                {
                    Node child = new Node()
                    {
                        Id = record["childrenId"].As<string>(),
                        ParentId = node.Id,
                        Title = record["title"].As<string>(),
                        Type = record["type"].As<string>(),
                        Removed = record["removed"].As<bool>(),
                    };

                    return child;
                });
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await session.CloseAsync();
            }

            return children;
        }

        public async Task<Node> CreateNode(Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            IResultCursor cursor;
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                var parameters = new
                {
                    newNode = node.AsDictionary()
                };
                cursor = await session.RunAsync(@"
MATCH (parent: Node {id: $newNode.parentId})
SET parent.type = 'knowledge' 
WITH parent
WHERE parent.type <> 'root'
CREATE (child: Node {title: $newNode.title, type: $newNode.type, removed: false, id: apoc.create.uuid()})
CREATE (parent)-[:contains]->(child)
RETURN child.id AS id
", parameters);
                IRecord record = await cursor.SingleAsync();
                node.Id = record["id"].As<string>();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await session.CloseAsync();
            }

            return node;
        }

        public async Task<Node> EditNode(string id, Node node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            IAsyncSession session = _driver.AsyncSession();
            try
            {
                node.Id = id;
                var parameters = new
                {
                    newNode = node.AsDictionary()
                };
                await session.RunAsync(@"
MATCH (node: Node {id: $newNode.id})
SET node.title = $newNode.title, node.type = $newNode.type
", parameters);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await session.CloseAsync();
            }

            return node;
        }

        public async Task<Node> DeleteNode(string id, Node node)
        {
            IAsyncSession session = _driver.AsyncSession();
            IResultCursor cursor;
            try
            {
                cursor = await session.RunAsync(@"
MATCH (n: Node {id: $id})
OPTIONAL MATCH (n)-[:contains]->(children : Node {removed: false})

WITH n, COUNT(children) as nbChildren
SET n.removed = CASE WHEN nbChildren = 0 THEN true ELSE n.removed END

WITH n
OPTIONAL MATCH (b:Belief)<-[:interaction]-(n)
DETACH DELETE b

WITH n
MATCH (parent : Node)-[:contains]->(n)
OPTIONAL MATCH (parent)-[:contains]->(siblings : Node{removed: false}) WHERE siblings.id <> n.id

WITH parent, COUNT(siblings) AS nbSiblings
SET parent.type = CASE WHEN parent.type <> 'root' AND nbSiblings = 0 THEN 'didactic' ELSE  parent.type END

RETURN count(*) AS count
", new {id});

                IRecord record = await cursor.SingleAsync();

                if (record["count"].As<long>() == 0)
                    node.Removed = true;
                else
                    node = null;
            }

            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await session.CloseAsync();
            }

            return node;
        }
    }
}