using System.ComponentModel.DataAnnotations;

namespace WebApplicationManagerApi.AuthApp
{
    public class UserModel
    {
        [Required, MaxLength(20)]
        public string LoginProp { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public string Email { get; set; }

    }
}
