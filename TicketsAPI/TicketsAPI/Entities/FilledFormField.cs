using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Entities
{
    public class FilledFormField : CrudEntities
    {
        [Key]
        public long IdFilledFormField { get; set; }

        public bool? IsChecked { get; set; }
        public string? TextValue { get; set; }
        public decimal? NumericValue { get; set; }
        public DateTime? DateTimeValue { get; set; }

        // Relación con FilledForm
        [ForeignKey("FilledForm")]
        public long FilledFormId { get; set; }
        public FilledForm FilledForm { get; set; }

        

        // Relación con Option
        [ForeignKey("SelectedOption")]
        public long? SelectedOptionId { get; set; }
        public Option? SelectedOption { get; set; }
    }
}
