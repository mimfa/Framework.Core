using MiMFa.Model;
using MiMFa.Exclusive.DateAndTime;
using MiMFa.General;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiMFa.Model.Structure.Instance
{
    [Serializable]
    public class Person : StructureBase
    {
        public virtual Image Image { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string FatherName { get; set; }
        public virtual GenderMode Gender { get; set; } = GenderMode.Null;
        public virtual SmartDate BirthDate { get; set; } = new SmartDate();
        public virtual MaritalMode Marital { get; set; } = MaritalMode.Null;
        public virtual int NumberOfChildren { get; set; } = 0;
        public virtual EducationalDegreeMode EducationalDegree { get; set; }
        public virtual string NID { get; set; }
        public virtual string PID { get; set; }
        public virtual string JobTitle { get; set; }
        public virtual string InsuranceID { get; set; }
        public virtual string MobileNumber { get; set; }
        public virtual string HomePhoneNumber { get; set; }
        public virtual string HomeAddress { get; set; }
        public virtual string OfficePhoneNumber { get; set; }
        public virtual string OfficeAddress { get; set; }
        public virtual string FaxNumber { get; set; }
        public virtual string Email { get; set; }

        public Person() : base() { }
    }
}
