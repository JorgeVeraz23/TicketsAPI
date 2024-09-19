using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TicketsAPI.Entities;

namespace TicketsAPI.DTO
{
    public class FormGroupDTO
    {

        public long IdFormGroup { get; set; }

   
        public string Name { get; set; } = string.Empty;

    
        public long FormId { get; set; }


    }
}
