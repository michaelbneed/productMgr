using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PM.Entity.Models
{
    public partial class Note
    {
        public int Id { get; set; }
		[Required]
        public string NoteText { get; set; }
        public int? RequestId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Request Request { get; set; }
    }
}
