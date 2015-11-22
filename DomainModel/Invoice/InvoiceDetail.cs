using System;

namespace GestionCommerciale.DomainModel.Invoice
{
    public class InvoiceDetail 
    {
        /// <summary>
        /// Gets or Sets invoice number
        /// </summary>
        public int InvoiceNumber { get; set; }
        /// <summary>
        /// Gets or Sets invoice date
        /// </summary>
        public DateTime InvoiceDate { get; set; }
        /// <summary>
        /// Gets or Sets payment mode { Chéque, Espèce,Credit }
        /// </summary>
        public string PayMode { get; set; }
        /// <summary>
        /// Gets or Set customer raison sociale  { Customer company name }
        /// </summary>
        public string RS { get; set; }
        /// <summary>
        /// Gets or Sets product name
        /// </summary>
        public string ProductNAme { get; set; }
        /// <summary>
        /// Gets or Sets product quantity on order
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Gets or Sets product unit price
        /// </summary>
        public decimal UnitPrice { get; set; }

        public String Unite { get; set; }
        /// <summary>
        /// Gets or Sets Amount { Amount = UnitPrice*Quantity }
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Gets or Sets TVA 
        /// </summary>
        public decimal TVA { get; set; }
        /// <summary>
        /// Gets or Sets TotalHT { TotalHT = SUM(Amount) }
        /// </summary>
        public decimal TotalHT { get; set; }
        /// <summary>
        /// Gets or Sets Total { Total = TotalHT * ( TVA + 1) }
        /// </summary>
        public decimal Total { get; set; }
        /// <summary>
        /// Gets or Sets Plus TVA { PTVA = TotalHT * TVA }
        /// </summary>
        public decimal PTVA { get; set; }
        /// <summary>
        /// Gets or Sets Timbre value
        /// </summary>
        public decimal Timbre { get; set; }

    }
}
