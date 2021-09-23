using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormatZPD.Models;
using Neo4j.Driver;

namespace FormatZPD.Repositories
{
    public class BeliefRepo : IBeliefRepo
    {
        private readonly IDriver _driver;
        private readonly INodeRepo _nodeRepo;

        public BeliefRepo(IDriver driver, INodeRepo nodeRepo)
        {
            _driver = driver;
            _nodeRepo = nodeRepo;
        }

        public async Task<IEnumerable<Belief>> GetBelievesOfPerson(string personId)
        {
            IResultCursor cursor;
            IAsyncSession session = _driver.AsyncSession();
            List<Belief> believes = new List<Belief>();
            try
            {
                cursor = await session.RunAsync(@"
MATCH (p:Person {id: $personId})-[:hasResult]->(b:Belief)
MATCH (b)<-[i:interaction]-(n:Node)
RETURN b.id AS id, b.hability AS hability, b.ignorance AS ignorance, b.dishability AS dishability, b.conflict AS conflict, COLLECT(n.id) AS nodeId, COLLECT(i.level) AS level  
", new {personId});
                believes = await cursor.ToListAsync(record =>
                {
                    IList<string> listNodeIs = record["nodeId"].As<IList<string>>();
                    IList<string> listLevel = record["level"].As<IList<string>>();

                    Belief belief = new Belief()
                    {
                        Id = record["id"].As<string>(),
                        Hability = record["hability"].As<float>(),
                        Ignorance = record["ignorance"].As<float>(),
                        Dishability = record["dishability"].As<float>(),
                        Conflict = record["conflict"].As<float>(),
                        PersonId = personId,
                        Interactions = new List<InteractionBelief>()
                    };

                    //belief.calculateColorGradient(belief.Hability, belief.Dishability, belief.Ignorance, belief.Conflict);

                    for (int i = 0; i < listNodeIs.Count; i++)
                    {
                        belief.Interactions.Add(new InteractionBelief()
                        {
                            NodeId = listNodeIs[i],
                            Level = listLevel[i],
                        });
                    }

                    return belief;
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

            return believes;
        }

        public async Task<Belief> GetBeliefById(string id)
        {
            IResultCursor cursor;
            IAsyncSession session = _driver.AsyncSession();
            Belief belief = null;
            try
            {
                cursor = await session.RunAsync(@"
MATCH (b:Belief {id: $id}) 
MATCH (b)<-[:hasResult]-(p:Person)
MATCH (b)<-[i:interaction]-(n:Node)
RETURN b.id AS id, b.hability AS hability, b.ignorance AS ignorance, b.dishability AS dishability, b.conflict AS conflict, p.id as personId, COLLECT(n.id) AS nodeId, COLLECT(i.level) AS level
", new {id});
                IRecord record = await cursor.SingleAsync();
                if (record != null)
                {
                    IList<string> listNodeIs = record["nodeId"].As<IList<string>>();
                    IList<string> listLevel = record["level"].As<IList<string>>();

                    belief = new Belief()
                    {
                        Id = record["id"].As<string>(),
                        Hability = record["hability"].As<float>(),
                        Ignorance = record["ignorance"].As<float>(),
                        Dishability = record["dishability"].As<float>(),
                        Conflict = record["conflict"].As<float>(),
                        PersonId = record["personId"].As<string>(),
                        Interactions = new List<InteractionBelief>()
                    };

                    //belief.calculateColorGradient(belief.Hability, belief.Dishability, belief.Ignorance, belief.Conflict);

                    for (int i = 0; i < listNodeIs.Count; i++)
                    {
                        belief.Interactions.Add(new InteractionBelief()
                        {
                            NodeId = listNodeIs[i],
                            Level = listLevel[i],
                        });
                    }
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

            return belief;
        }

        public async Task<Belief> GetBeliefFromParent(string personId, string parent1Id, string parent2Id)
        {
            Belief toSumBelief = new Belief();
            Belief fusionedBelief = new Belief();
            List<Belief> alreadyAddedBelieves = new List<Belief>();

            Node parent1 = await _nodeRepo.GetNodeById(parent1Id);
            Node parent2 = await _nodeRepo.GetNodeById(parent2Id);

            List<Node> list = new List<Node>();
            list.Add(parent1);
            list.Add(parent2);

            Stack<List<Node>> listParent = new Stack<List<Node>>();
            listParent.Push(list);

            while (listParent.Count != 0)
            {
                var nodes = listParent.Pop();
                foreach (Node node in nodes)
                {
                    var children = await _nodeRepo.GetChildrenOfNode(node);
                    if (children.Count() != 0) //si ce n'est pas une variable didactique, donc il a encore des enfants
                    {
                        listParent.Push(children.ToList());
                    }
                    else //c'est une variable didactique donc elle a possiblement des believes
                    {
                        var believes = await GetBeliefOfPersonAndDidactic(personId, node);
                        believes = believes.ToList();
                        foreach (Belief b in believes) //fusion des believes ayant leurs parents directs en commun
                        {
                            if (!alreadyAddedBelieves.Contains(b)
                            ) //si un belief n'a pas déjà été utilisé dans le calcul, on peut l'utiliser pour calculer
                            {
                                toSumBelief += b;
                            }

                            alreadyAddedBelieves
                                .Add(b); //dans le cas où un belief apparaît plusieurs fois, on ne le compte qu'une seule fois. On le sauvegarde donc dans une liste pour se souvenir des believes deja ajouté
                        }

                        fusionedBelief =
                            fusionedBelief +
                            toSumBelief; // la valeur du nouveau belief fusionné ajouté aux autres believes fusionnés
                        toSumBelief = new Belief(); //remise à 0 pour le prochain calcul
                    }
                }
            }

            return fusionedBelief;
        }

        public async Task<Belief> CreateBelief(Belief belief)
        {
            IAsyncSession session = _driver.AsyncSession();
            IResultCursor cursor;

            try
            {
                Belief oldBelief = await GetBeliefFromRelation(belief.PersonId, belief.Interactions);
                string query;
                if (oldBelief != null)
                {
                    belief = belief + oldBelief;
                    query = @"
MATCH (b:Belief{id:$belief.id})
SET b.hability = apoc.math.round($belief.hability,4,'HALF_UP'),
    b.ignorance = apoc.math.round($belief.ignorance,4,'HALF_UP'),
    b.dishability = apoc.math.round($belief.dishability,4,'HALF_UP'),
    b.conflict = apoc.math.round($belief.conflict,4,'HALF_UP')
RETURN b.id AS id";
                }
                else
                {
                    query = @"
MATCH (p:Person{id: $belief.personId})
MATCH (n0:Node{id: $node0.nodeId})
MATCH (n1:Node{id: $node1.nodeId})
WHERE n0.type = 'didactic' AND n1.type = 'didactic'
CREATE (b: Belief {id: apoc.create.uuid(), hability: apoc.math.round($belief.hability,4,'HALF_UP'), dishability: apoc.math.round($belief.dishability,4,'HALF_UP'), ignorance: apoc.math.round($belief.ignorance,4,'HALF_UP'), conflict: apoc.math.round($belief.conflict,4,'HALF_UP')})
create (p)-[:hasResult]->(b)
create (b)<-[:interaction{level: $node0.level}]-(n0)
create (b)<-[:interaction{level: $node1.level}]-(n1)
RETURN b.id AS id";
                }

                cursor = await session.RunAsync(query, new
                {
                    belief = belief.AsDictionary(),
                    node0 = belief.Interactions[0].AsDictionary(),
                    node1 = belief.Interactions[1].AsDictionary(),
                });

                IRecord record = await cursor.SingleAsync();
                belief.Id = record["id"].As<string>();

                return belief;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<string> DeleteBelief(string id)
        {
            IAsyncSession session = _driver.AsyncSession();
            IResultCursor cursor;
            try
            {
                cursor = await session.RunAsync(@"
MATCH (b: Belief {id: $id})
WITH b, b.id AS id
DETACH DELETE b
RETURN id", new {id});
                IRecord record = await cursor.SingleAsync();

                if (record != null)
                {
                    return record["id"].As<string>();
                }
                else
                {
                    return null;
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
        }

        private async Task<Belief> GetBeliefFromRelation(string personId, List<InteractionBelief> interactionBeliefs)
        {
            IResultCursor cursor;
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                cursor = await session.RunAsync(@"
MATCH (:Person{id: $personId})-[:hasResult]->(b:Belief)
MATCH (:Node{id: $node0.nodeId})-[:interaction{level: $node0.level}]->(b)
MATCH (:Node{id: $node1.nodeId})-[:interaction{level: $node1.level}]->(b)
RETURN coalesce(collect(b)) AS list
", new
                {
                    personId,
                    node0 = interactionBeliefs[0].AsDictionary(),
                    node1 = interactionBeliefs[1].AsDictionary(),
                });
                IRecord record = await cursor.SingleAsync();
                IList<INode> listBelieves = record["list"].As<IList<INode>>();
                if (listBelieves.Count == 0) return null;
                var nodeBelief = listBelieves[0].Properties;
                Belief belief = new Belief()
                {
                    Id = nodeBelief["id"].As<string>(),
                    Hability = nodeBelief["hability"].As<float>(),
                    Ignorance = nodeBelief["ignorance"].As<float>(),
                    Dishability = nodeBelief["dishability"].As<float>(),
                    Conflict = nodeBelief["conflict"].As<float>(),
                    PersonId = personId,
                    Interactions = new List<InteractionBelief>()
                };
                foreach (InteractionBelief interactionBelief in interactionBeliefs)
                    belief.Interactions.Add(interactionBelief);
                return belief;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IEnumerable<Belief>> GetBeliefOfPersonAndDidactic(string personId, Node node)
        {
            IResultCursor cursor;
            IAsyncSession session = _driver.AsyncSession();
            List<Belief> believes = new List<Belief>();
            try
            {
                cursor = await session.RunAsync(@"
MATCH (p:Person {id: $personId})-[:hasResult]->(b:Belief)
MATCH (b)<-[i:interaction]-(n:Node)
WHERE n.id = $node.id
RETURN b.id AS id, b.hability AS hability, b.ignorance AS ignorance, b.dishability AS dishability, b.conflict AS conflict, COLLECT(n.id) AS nodeId, COLLECT(i.level) AS level  
", new {personId, node = node.AsDictionary()});

                believes = await cursor.ToListAsync(record =>
                {
                    IList<string> listNodeIs = record["nodeId"].As<IList<string>>();
                    IList<string> listLevel = record["level"].As<IList<string>>();

                    Belief belief = new Belief()
                    {
                        Id = record["id"].As<string>(),
                        Hability = record["hability"].As<float>(),
                        Ignorance = record["ignorance"].As<float>(),
                        Dishability = record["dishability"].As<float>(),
                        Conflict = record["conflict"].As<float>(),
                        PersonId = personId,
                        Interactions = new List<InteractionBelief>()
                    };

                    //belief.calculateColorGradient(belief.Hability, belief.Dishability, belief.Ignorance, belief.Conflict);

                    for (int i = 0; i < listNodeIs.Count; i++)
                    {
                        belief.Interactions.Add(new InteractionBelief()
                        {
                            NodeId = listNodeIs[i],
                            Level = listLevel[i],
                        });
                    }

                    return belief;
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

            return believes;
        }
    }
}