namespace Filip_Rados_lab2.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
