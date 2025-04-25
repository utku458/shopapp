using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;

namespace shopapp.entity
{
    public class Order
    {
        public int Id { get; set; }
        
        public string OrderNumber { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        public string UserId { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        
        public string Note { get; set; }
        
        public EnumOrderStatus OrderStatus { get; set; }
        
        
        public List<OrderItem> OrderItems { get; set; }


        public string PaymentId { get; set; }
        
        public string ConversationId { get; set; }
        
        public EnumPaymentTypes PaymentTypes { get; set; }
        
    }

    public enum EnumOrderStatus
    {
        pending=0,
        unpaid=1,
        completed=2,
    }
    
    public enum EnumPaymentTypes
    {
        CreditCart=0,
        Eft=1,
        
    }
}