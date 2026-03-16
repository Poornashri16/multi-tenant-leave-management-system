using System;

namespace SaaSPlatform.Application.DTOs
{
    

    public class CreateReimbursementDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
    }

    
}