namespace FormatZPD.Dtos

{

    /// <summary>
    /// NodeReadDto indique tous les attributs que la requête de l'API va renvoyer
    /// </summary>
    public class NodeReadDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string ParentId { get; set; }
        public bool Removed { get; set; }
    }
}