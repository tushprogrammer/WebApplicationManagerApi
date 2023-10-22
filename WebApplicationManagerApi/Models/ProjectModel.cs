using System.ComponentModel.DataAnnotations;

namespace WebApplicationManagerApi.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string NameCompany { get; set; }
        public string Description { get; set; }
        public string Image_name { get; set; }
        public byte[] Image_byte { get; set; }
        public string Name_page { get; set; }
    }
}
