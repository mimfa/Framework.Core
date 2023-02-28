using MiMFa.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.ProgramingTechnology.CommandLanguage
{
    public class Address : MiMFa.Config
    {
        public string MCLExtension = ".mcl";
        public string FunctionExtension = ".mclf";
        public string ClassExtension = ".mclc";
        public string ProjectExtension = ".mclp";
        public string BinaryExtension = ".mbo";
        public string WorkSpaceExtension = ".mws";
        public string dbCompiler;

        public string Kernel;
        public string Constructor;
        public string Destructor;
        public string Initialize;
        public string Finalize;
        public string CommandDirectory { get;  set; }
        public string BaseDirectory { get;  set; }
        public string BaseFunctionDirectory { get;  set; }
        public string BaseClassDirectory { get;  set; }
        public string BaseProjectDirectory { get;  set; }
        public string BaseOtherDirectory { get;  set; }
        public string MainDirectory { get;  set; }
        public string HelpDirectory { get;  set; }
        public string AttachDirectory { get; set; }

        public Address() : base()
        {
            DefaultValues();
        }
        public new void DefaultValues()
        {
            Config.DefaultValues();
            dbCompiler = ConfigurationDirectory + "Compiler" + DataBaseExtension + ConfigurationExtension;
            CommandDirectory = ThisDirectory + @"Command\";
            MainDirectory = CommandDirectory + @"Main\"; 
            BaseDirectory = CommandDirectory + @"Base\";
            BaseFunctionDirectory = BaseDirectory + @"Function\";
            BaseClassDirectory = BaseDirectory + @"Class\"; ;
            BaseProjectDirectory = BaseDirectory + @"Project\"; ;
            BaseOtherDirectory = BaseDirectory + @"Other\"; ;
            HelpDirectory = CommandDirectory + @"Help\";
            AttachDirectory = CommandDirectory + @"Attach\";
            Kernel = MainDirectory + "Kernel" + MCLExtension;
            Constructor = MainDirectory + "Constructor" + MCLExtension;
            Destructor = MainDirectory + "Destructor" + MCLExtension;
            Initialize = MainDirectory + "Initialize" + MCLExtension;
            Finalize = MainDirectory + "Finalize" + MCLExtension;
        }
    }
}
