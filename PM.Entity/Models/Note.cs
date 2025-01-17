﻿using System;
using System.Collections.Generic;

namespace PM.Entity.Models
{
    public partial class Note
    {
        public int Id { get; set; }
        public string NoteText { get; set; }
        public bool SendEmailRequestor { get; set; }
        public bool SendEmailSupplier { get; set; }
        public int? RequestId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Request Request { get; set; }
    }
}
