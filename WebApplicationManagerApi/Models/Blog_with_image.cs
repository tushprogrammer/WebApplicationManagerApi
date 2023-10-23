namespace WebApplicationManagerApi.Models
{
    public class Blog_with_image
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public string Image_name { get; set; }
        public byte[] Image_byte { get; set; }
    }
}
