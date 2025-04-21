using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
    // Contains all constants for out website
    public static class SD
    {
        public const string Role_Admin = "ADMIN";
        public const string Role_Company = "COMPANY";
        public const string Role_Employee = "EMPLOYEE";
        public const string Role_Customer = "CUSTOMER";

        // Customer
        // Status:  Pending -> Approved -> Processing ->  Shipped
        // Payment: Pending -> Approved ->  Approved  ->  Approved

        // Company
        // Status:     Approved    ->   Processing   ->      Shipped     ->  Shipped
        // Payment: DelayedPayment -> DelayedPayment ->  DelayedPayment  ->  Approved

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProccess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "DelayedPayment";
        public const string PaymentStatusRejected = "Rejected";

        public const string SessionCart = "SessionShoppingCart";
    }
}
