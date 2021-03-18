namespace Conduit.Data
{
    public class Favorite
    {
        public int PersonId { get; set; }
        public int EditorialId { get; set; }
        public Person Person { get; set; }
        public Editorial Editorial { get; set; }
    }
}
