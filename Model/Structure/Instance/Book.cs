using System;
using System.Collections.Generic;
using MiMFa.Exclusive.DateAndTime;
using System.Drawing;

namespace MiMFa.Model.Structure.Instance
{
    [Serializable]
    public class Book : StructureBase
    {
        public virtual string Subject { get; set; }
        public virtual List<Person> AuthorPersons { get; set; } = new List<Person>();
        public virtual string Comment { get; set; }
        public virtual Image Image { get; set; }
        public virtual string Publisher { get; set; }
        public virtual SmartDate PublishDate { get; set; } = new SmartDate();
        public virtual decimal Edition { get; set; } = 1;
        public virtual decimal Cover { get; set; } = 1;
        public virtual decimal NumberOfCover { get; set; } = 1;
        public virtual decimal NumberOfSection { get; set; }
        public virtual decimal NumberOfPage { get; set; }
        public virtual string ISBN { get; set; }
        public virtual decimal Price { get; set; }

        public virtual object File { get; set; }
        public virtual string Path { get; set; }

        public Book() : base() { }
    }
}
