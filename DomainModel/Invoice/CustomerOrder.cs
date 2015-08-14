using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel
{
    public class CustomerOrder
    {
        public int OrderID{get;set;}
        public string CustomerName{get;set;}
        public string EmployeeName { get; set; }
        public DateTime? Date { get; set; }
        public bool IsInvoiced { get; set; }        
        
        public Order Order { get; set; }
        public IEnumerable OrderDetails { get; set; }
    }
}
