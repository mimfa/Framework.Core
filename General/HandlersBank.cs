using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.General
{
    public delegate void GenericEventListener<TSender, TArgIn>(TSender sender, TArgIn arg);
    public delegate void GenericEventListener<TSender, TArgIn1, TArgIn2>(TSender sender, TArgIn1 arg1, TArgIn2 arg2);
    public delegate void GenericEventListener<TSender, TArgIn1, TArgIn2, TArgIn3>(TSender sender, TArgIn1 arg1, TArgIn2 arg2, TArgIn3 arg3);
    public delegate void GenericEventListener<TSender, TArgIn1, TArgIn2, TArgIn3, TArgIn4>(TSender sender, TArgIn1 arg1, TArgIn2 arg2, TArgIn3 arg3, TArgIn4 arg4);
   
    public delegate TOut GenericEventHandler<TOut>();
    public delegate TOut GenericEventHandler<TSender, TOut>(TSender sender);
    public delegate TOut GenericEventHandler<TSender, TArgIn, TOut>(TSender sender, TArgIn arg);
    public delegate TOut GenericEventHandler<TSender, TArgIn1, TArgIn2, TOut>(TSender sender, TArgIn1 arg1, TArgIn2 arg2);
    public delegate TOut GenericEventHandler<TSender, TArgIn1, TArgIn2, TArgIn3, TOut>(TSender sender, TArgIn1 arg1, TArgIn2 arg2, TArgIn3 arg3);
    public delegate TOut GenericEventHandler<TSender, TArgIn1, TArgIn2, TArgIn3, TArgIn4, TOut>(TSender sender, TArgIn1 arg1, TArgIn2 arg2, TArgIn3 arg3, TArgIn4 arg4);

    public delegate void GenericArgEventListener<TArgIn>(object sender, TArgIn arg);
    public delegate void GenericArgEventListener<TArgIn1, TArgIn2>(object sender, TArgIn1 arg1, TArgIn2 arg2);
    public delegate void GenericArgEventListener<TArgIn1, TArgIn2, TArgIn3>(object sender, TArgIn1 arg1, TArgIn2 arg2, TArgIn3 arg3);
    public delegate void GenericArgEventListener<TArgIn1, TArgIn2, TArgIn3, TArgIn4>(object sender, TArgIn1 arg1, TArgIn2 arg2, TArgIn3 arg3, TArgIn4 arg4);
    
    public delegate TOut GenericArgEventHandler<TArgIn, TOut>(object sender, TArgIn arg);
    public delegate TOut GenericArgEventHandler<TArgIn1, TArgIn2, TOut>(object sender, TArgIn1 arg1, TArgIn2 arg2);
    public delegate TOut GenericArgEventHandler<TArgIn1, TArgIn2, TArgIn3, TOut>(object sender, TArgIn1 arg1, TArgIn2 arg2, TArgIn3 arg3);
    public delegate TOut GenericArgEventHandler<TArgIn1, TArgIn2, TArgIn3, TArgIn4, TOut>(object sender, TArgIn1 arg1, TArgIn2 arg2, TArgIn3 arg3, TArgIn4 arg4);
}
