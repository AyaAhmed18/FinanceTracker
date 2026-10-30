using FinanceTracker.Api.Entities;

namespace FinanceTracker.Api.DTOS.ReportDtos
{
    public class CategorySummaryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }
    }
}
