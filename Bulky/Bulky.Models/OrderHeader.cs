using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }
        
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        public DateTime PaymentDate { get; set; }
        // Due date will be type of DateOnly - in EF Core special feature 'DateOnly'
        public DateOnly PaymentDuoDate { get; set; }

        // Stripe payments or credit card payments require from as to also add needed information
        // For Stripe - we are getting unique ID from Stripe which will be PaymentIntentId
        // It will uniquely identify each payment that's done for order and save that in db
        public string? PaymentIntentId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required] 
        public string City { get; set; }
        [Required] 
        public string State { get; set; }
        [Required] 
        public string PhoneNumber { get; set; }
    }
}
