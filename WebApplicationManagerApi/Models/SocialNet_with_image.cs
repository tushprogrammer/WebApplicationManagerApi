namespace WebApplicationManagerApi.Models
{
    public class SocialNet_with_image
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string? Image_name { get; set; }
        public byte[]? Image_byte { get; set; }
        public string? ImgSrc { get; set; }
    }
}
