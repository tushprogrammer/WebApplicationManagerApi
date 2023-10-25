using ApplicationManager_ClassLibrary.Entitys;

namespace WebApplicationManagerApi.Models
{
    public class ContactsModel
    {
        public IQueryable<Contacts> Contacts { get; set; }
        public string? Image_name { get; set; }
        public byte[]? Image_byte { get; set; }
        public string? ImgSrc { get; set; }

        public IQueryable<SocialNet_with_image> Nets { get; set; }
        public string Name_page { get; set; }
    }
}