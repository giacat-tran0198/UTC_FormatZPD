using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FormatZPD.Models
{
    public class Person
    {
        /// <summary>
        /// Person représente un apprenant.
        /// Il contient un identifiant, un prénom, un nom et un boolén "removed" qui indique si on l'a supprimé de la base de données ou non.
        /// </summary>
        
        #region Public properties

        [JsonPropertyName("id")] public string Id { get; set; }

        [JsonPropertyName("firstname")] public string FirstName { get; set; }

        [JsonPropertyName("lastname")] public string LastName { get; set; }

        [JsonPropertyName("removed")] public bool Removed { get; set; }

        #endregion

        #region Dictionary

        public IDictionary<string, object> AsDictionary()
        {
            return new Dictionary<string, object>
            {
                {"id", Id},
                {"firstname", FirstName},
                {"lastname", LastName},
            };
        }

        #endregion
    }
}