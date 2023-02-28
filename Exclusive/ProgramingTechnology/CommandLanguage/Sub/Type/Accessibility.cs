using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiMFa.General;

namespace MiMFa.Exclusive.ProgramingTechnology.CommandLanguage
{
    [Serializable]
    public class Accessibility
    {
        public ProgrammingAccessMode Status = ProgrammingAccessMode.Internal;
        public long ID = UniqueService.CreateNewLong();
        public Accessibility Parent = null;

        public static bool IsFamily(Accessibility node1, Accessibility node2)
        {
            Accessibility n1 = new Accessibility() {ID = node1.ID,Parent = node1.Parent , Status =node1.Status };
            Accessibility n2 = new Accessibility() {ID = node2.ID,Parent = node2.Parent , Status =node2.Status };
            do
            {
                n1 = n1.Parent;
                do
                {
                    n2 = n2.Parent;
                    if(IsInsider(n1, n2)) return true;
                }
                while (n2.Parent != null);
            }
            while (n1.Parent != null);
            return false;
        }
        public static bool IsParent(Accessibility parent, Accessibility child)
        {
            if (parent == null)
                if (child == null) return false;
                else return true;
            if (child.Parent == null) return false;
            if (IsInsider(parent, child.Parent)) return true;
            if (IsParent(parent, child.Parent)) return true;
            return false;
        }
        public static bool IsBrother(Accessibility node1, Accessibility node2)
        {
            if (IsInsider(node2.Parent,node1.Parent)) return true;
            return false;
        }
        public static bool IsInsider(Accessibility node1, Accessibility node2)
        {
            if (node2 == node1) return true;
            if (node2.ID == node1.ID) return true;
            return false;
        }

        public static bool IsAccess(Accessibility environment, Accessibility node)
        {
            if (node == null) return true;
            if (environment == null)
                if (node.Status == ProgrammingAccessMode.Public 
                    || node.Status == ProgrammingAccessMode.Internal) return true;
                else return false;
            if (node.Status == ProgrammingAccessMode.Public) return true;
            if (IsInsider(node, environment)) return true;
            if (IsParent(node, environment)) return true;
            int acn = Convert.ToInt16(node.Status);
            if (IsBrother(node, environment) && acn < 2) return true;
            if (IsFamily(node, environment) && acn < 2) return true;
            return false;
        }
    }
}
