namespace WebApplicationManagerApi.Models
{
    public class MainPageUploadModel
    {
        public string Title { get; set; }
        public string RequestTitle { get; set; }
        public string ButtonTitle { get; set; }
        public byte[] Image_byte { get; set; }
        public string ImgSrc { get; set; }
        public string Image_name { get; set; }
        public IFormFile? Image { get; set; }
    }
}
