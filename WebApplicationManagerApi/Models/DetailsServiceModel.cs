using System.ComponentModel.DataAnnotations;

namespace WebApplicationManagerApi.Models
{
    public class DetailsServiceModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Name_page { get; set; }
    }
}
