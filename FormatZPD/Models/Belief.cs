using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Drawing;
using System.Linq;

namespace FormatZPD.Models
{
    /// <summary>
    /// Constantes des valeurs par défaut de chaque attributs du Belief
    /// </summary>
    public class Belief : IEquatable<Belief>
    {
        const int BELIEF_DEFAULT_HABILITY = 0;
        const int BELIEF_DEFAULT_DISHABILITY = 0;
        const int BELIEF_DEFAULT_IGNORANCE = 1;
        const int BELIEF_DEFAULT_CONFLICT = 0;

        const float EPSILON = 0.0001f;
        const float EPSILON_MAX = 1 - EPSILON;

        private float hability = BELIEF_DEFAULT_HABILITY;
        private float dishability = BELIEF_DEFAULT_DISHABILITY;
        private float ignorance = BELIEF_DEFAULT_IGNORANCE;
        private float conflict = BELIEF_DEFAULT_CONFLICT;

        //Getter & Setter
        [JsonPropertyName("id")] public string Id { get; set; }

        [JsonPropertyName("hability")]
        public float Hability
        {
            get => (float) Math.Round(hability,4);
            set => hability = zeroTest(value);
        }

        [JsonPropertyName("dishability")]
        public float Dishability
        {
            get => (float) Math.Round(dishability,4);
            set => dishability = zeroTest(value);
        }

        [JsonPropertyName("ignorance")]
        public float Ignorance
        {
            get => (float) Math.Round(ignorance,4);
            set => ignorance = zeroTest(value);
        }

        [JsonPropertyName("conflict")]
        public float Conflict
        {
            get => (float) Math.Round(conflict,4);
            set => conflict = zeroTest(value);
        }

        /// <summary>
        /// PersonId est l'identifiant de la personne qui possède le Belief
        /// Interactions est de la classe InteractionBelief et représente le niveau d'interaction avec un parent du Belief.
        /// On utilise donc une liste de InteractionBelief pour spécifier les interactions entre plusieurs parents.
        /// </summary>
        
        [JsonPropertyName("personId")] public string PersonId { get; set; }
        [JsonPropertyName("interactions")] public List<InteractionBelief> Interactions { get; set; }

        /// <summary>
        /// ColorBelief est l'attribut renvoyant la couleur d'un Belief
        /// </summary>
        public string ColorBelief
        {
            get => calculateColorGradient();
        }

        //Constructeur
        public Belief(float hability, float dishability, float ignorance, float conflict)
        {
            Hability = hability;
            Dishability = dishability;
            Ignorance = ignorance;
            Conflict = conflict;
            Interactions = new List<InteractionBelief>();
        }

        //Constructeur par défaut
        public Belief()
        {
            Interactions = new List<InteractionBelief>();
        }

        //Surcharge de l'opérateur +. Créer un nouvel objet Belief. Surcharge également l'opérateur +=
        public static Belief operator +(Belief a, Belief b)
        {
            float h = a.Hability * b.Hability + a.Ignorance * b.Hability + a.Hability * b.Ignorance;
            float d = a.Dishability * b.Dishability + a.Ignorance * b.Dishability + b.Ignorance * a.Dishability;
            float i = a.Ignorance * b.Ignorance;

            return new Belief
            {
                Hability = h,
                Dishability = d,
                Ignorance = i,
                conflict = 1 - h - d - i,
                Id = a.Id ?? b.Id,
                PersonId = a.PersonId ?? b.PersonId,
                Interactions = a.Interactions.Count != 0 ? a.Interactions : b.Interactions
            };
        }

        /// <summary>
        /// calculateColorGradient calcule la couleur du Belief à partir de hability, dishability, conflict, et ignorance du Belief
        /// </summary>

        private string calculateColorGradient()
        {
            float colorR, colorG, colorB;

            //Calcule le maximum entre conflict, hability, dishability, ignorance
            float[] anArray = {hability, dishability, ignorance, conflict};
            float varMax = anArray.Max();

            if (varMax == conflict)
            {
                colorR = Color.Red.R * varMax;
                colorG = Color.Red.G * varMax;
                colorB = Color.Red.B * varMax;
            }
            else if (varMax == hability)
            {
                colorR = Color.Blue.R * varMax;
                colorG = Color.Blue.G * varMax;
                colorB = Color.Blue.B * varMax;
            }
            else if (varMax == dishability)
            {
                colorR = Color.Yellow.R * varMax;
                colorG = Color.Yellow.G * varMax;
                colorB = Color.Yellow.B * varMax;
            }
            else //cas où ignorance est le maximum
            {
                colorR = Color.LightGray.R * varMax;
                colorG = Color.LightGray.G * varMax;
                colorB = Color.LightGray.B * varMax;
            }

            return Color.FromArgb((int) colorR, (int) colorG, (int) colorB).ToString();
        }


        //Methode de test de Float
        private static float oneTest(float a)
        {
            return a > EPSILON_MAX ? 1 : a;
        }

        private static float zeroTest(float a)
        {
            return oneTest(a < EPSILON ? 0 : a);
        }

        #region Dictionary

        public IDictionary<string, object> AsDictionary()
        {
            return new Dictionary<string, object>
            {
                {"id", Id},
                {"hability", Hability},
                {"dishability", Dishability},
                {"ignorance", Ignorance},
                {"conflict", Conflict},
                {"personId", PersonId},
            };
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Belief);
        }

        public bool Equals(Belief other) //surcharge de Equals pour tester l'égalité entre deux Belief
        {
            return other != null &&
                   Id == other.Id &&
                   Hability == other.Hability &&
                   Dishability == other.Dishability &&
                   Ignorance == other.Ignorance &&
                   Conflict == other.Conflict &&
                   PersonId == other.PersonId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Hability, Dishability, Ignorance, Conflict, PersonId);
        }

        #endregion
    }
}