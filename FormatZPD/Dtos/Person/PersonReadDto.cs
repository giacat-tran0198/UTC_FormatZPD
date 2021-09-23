namespace FormatZPD.Dtos.Person
{
    /// <summary>
    /// PersonReadDto indique tous les attributs que la requête de l'API va renvoyer
    /// </summary>
    public class PersonReadDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Removed { get; set; }
    }
}