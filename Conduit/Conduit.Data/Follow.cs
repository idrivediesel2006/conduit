namespace Conduit.Data
{
    public class Follow
    {
        public int Follower { get; set; }
        public int Following { get; set; }
        public virtual Person FollowerNavigation { get; set; }
        public virtual Person FollowingNavigation { get; set; }
    }
}
