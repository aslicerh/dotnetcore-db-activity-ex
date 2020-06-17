using System.ComponentModel.DataAnnotations;

namespace RazorPagesContacts.Data
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public double Amount { get; set; }
    }
}
