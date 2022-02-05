using System.ComponentModel.DataAnnotations;

namespace Sample.Net6.Project.Models
{
    public class UserInfoViewModel
    {
        public int Id { get; set; }
        [Display(Name = "帳號")]
        public string? Name { get; set; }
        public string? Account { get; set; }
        [Display(Name = "密碼")]
        public string? Password { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }
        public DateTime LoginTime { get; set; }
    }
}
