using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Engine.Template
{
    public abstract class TemplatableBase : ITemplatable
    {
        public virtual bool HasTemplator => Templator != null;
        public virtual ITemplator Templator { get; set; } = null;

        public virtual void Template(Control control)
        {
            if(HasTemplator) Templator.Update(control);
            else Default.Template(control);
        }
    }
}
