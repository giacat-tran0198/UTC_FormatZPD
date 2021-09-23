using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FormatZPD.Models
{
    public class InteractionBelief
    {
        /// <summary>
        /// Cette collection indique la force de l'interaction entre un belief et ses parents.
        /// </summary>
        #region Collection

        public const string STRONG = "strong";
        public const string MEDIUM = "medium";
        public const string WEAK = "weak";

        #endregion

        /// <summary>
        /// Un InteractionBelief représente le lien entre le Belief et un de ses parents.
        /// Il est représenté par l'identifiant d'un parent, et la force de l'interaction entre le parent et le Belief.
        /// </summary>
        [JsonPropertyName("nodeId")] public string NodeId { get; set; }
        [JsonPropertyName("level")] public string Level { get; set; }
        
        #region Dictionary

        public IDictionary<string, object> AsDictionary()
        {
            return new Dictionary<string, object>
            {
                {"nodeId", NodeId},
                {"level", Level},
            };
        }

        #endregion
    }
}