using FinanceTracker.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Api.DTOS
{
    public class CategoryCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [EnumDataType(typeof(TransactionType))]
        public TransactionType Type { get; set; }
    }
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
    }
}
