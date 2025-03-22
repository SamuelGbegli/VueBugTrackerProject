using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VueBugTrackerProject.Classes
{
    /// <summary>
    /// Describes an account that can access the application.
    /// </summary>
    [Table("Accounts")]
	public class Account
	{
		/// <summary>
		/// Unique identifier for the class.
		/// </summary>
		[Key]
		[Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string ID { get; set; }

        /// <summary>
        /// The account’s username.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The account’s email address.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// The hashed value of the user’s password.
        /// </summary>
        [Required]
        public string PasswordHash { get; set; }

        /// <summary>
        /// The icon that will be shown with the account’s username.
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// The role and privileges the account has in the application.
        /// </summary>
        [Required]
        public AccountRole Role { get; set; }

        /// <summary>
        /// "If true, the user cannot log into the application.
        /// </summary>
        [Required]
        public bool Suspended { get; set; }

        /// <summary>
        /// The date and time the account was created.
        /// </summary> 
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }
	
		/// <summary>
		/// A JWT used to verify the user when performing various actions.x
		/// </summary>
		public string? LoginToken { get; set; }
	
	}
}
