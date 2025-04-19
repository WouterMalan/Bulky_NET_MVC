using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime OrderDate  { get; set; }

        public DateTime ShippingDate { get; set; }  

        public decimal OrderTotal { get; set; }

        public string? OrderStatus { get; set; }    

        public string? PaymentStatus { get; set; }  

        public string? TrackingNumber { get; set; } 

        public string? Carrier { get; set; }    

        public DateTime PaymentDate { get; set; }   

        public DateOnly PaymentDueDate { get; set; }    

        public string? SessionId { get; set; }

        public string? PaymentIntentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }
        
        [Required]
        [Display(Name = "Postal Code")]
        public string PostalAddress { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}