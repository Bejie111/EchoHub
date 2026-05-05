using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace EchoHub.Models
{
    public class Collection
    {
        public int CollectionId { get; set; }
        public int EwasteId { get; set; }
        [ForeignKey("EwasteId")]
        [ValidateNever]
        public EwasteItem EwasteItem { get; set; }
        public DateTime? ScheduleDate { get; set; }

        public DateTime? CollectionDate { get; set; }
        public String? Status { get; set; }
    }
}
