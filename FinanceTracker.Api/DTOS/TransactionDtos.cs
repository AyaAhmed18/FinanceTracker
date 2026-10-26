using FinanceTracker.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Api.DTOS
{
    public class TransactionCreateDto
    {
        [Range(0.01, 999999999)]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public int CategoryId { get; set; }
    }
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime Date { get; set; }
        public string? Note { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
