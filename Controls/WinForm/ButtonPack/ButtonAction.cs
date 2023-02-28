using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Controls.WinForm.ButtonPack
{
    public class ButtonAction
    {
        public EventHandler Apply = null;//= new EventHandler((object sender, EventArgs e) => { });
        public EventHandler Reset = null; //new EventHandler((object sender, EventArgs e) => { });
        public EventHandler Close = null; // new EventHandler((object sender, EventArgs e) => { });
        public EventHandler Next = null; // new EventHandler((object sender, EventArgs e) => { });
        public EventHandler Back = null; // new EventHandler((object sender, EventArgs e) => { });
        public EventHandler Menu = null; // new EventHandler((object sender, EventArgs e) => { });
    }
}
