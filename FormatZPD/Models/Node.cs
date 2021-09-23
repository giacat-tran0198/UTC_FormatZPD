using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FormatZPD.Models
{
    public class Node
    {
        /// <summary>
        /// Cette collection indique les valeurs "type" que peuvent prendre les noeuds.
        /// Le type "root" ne peut être assigné qu'à la racine.
        /// Le type didactic est donné aux noeuds feuilles (ne possédant pas de fils)
        /// Le type knowledge est donné à tous les autres noeuds.
        /// </summary>
        
        #region Collection

        public const string ROOT = "root";
        public const string KNOWLEDGE = "knowledge";
        public const string DIDACTIC = "didactic";

        #endregion


        /// <summary>
        /// Node représente une connaissance.
        /// Il contient un identifiant, un titre, un type, l'identifiant de son parent, 
        /// et un booléen removed qui indique si on l'a supprimé de la base de données ou non.
        /// </summary>
        #region Public properties

        public Node()
        {
        }

        [JsonPropertyName("id")] public string Id { get; set; }

        [JsonPropertyName("title")] public string Title { get; set; }

        [JsonPropertyName("type")] public string Type { get; set; }

        [JsonPropertyName("parentId")] public string ParentId { get; set; }

        [JsonPropertyName("removed")] public bool Removed { get; set; }

        #endregion

        #region Dictionary

        public IDictionary<string, object> AsDictionary()
        {
            return new Dictionary<string, object>
            {
                {"id", Id},
                {"title", Title},
                {"type", Type},
                {"parentId", ParentId},
            };
        }

        #endregion
    }
}