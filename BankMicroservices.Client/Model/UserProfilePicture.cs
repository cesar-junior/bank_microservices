using BankMicroservices.Client.Model.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankMicroservices.Client.Model
{
    [Table("profile_pictures")]
    public class UserProfilePicture : BaseEntity
    {
        [Column("user_id")]
        [Required]
        public required string UserId { get; set; }
        [Column("profile_picture")]
        [Required]
        public required string ProfilePicture { get; set; }
    }
}
