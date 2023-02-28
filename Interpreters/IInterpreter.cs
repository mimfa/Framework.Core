using MiMFa.General;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Interpreters
{
    /// <summary>
    /// Interpreters Engine Interface
    /// </summary>
    public interface IInterpreter
    {
        string Name { get; }
        string Description { get; }
        string Extension { get; }
        List<string> Libraries { get; }
     
        bool HasObject(string propertyName);

        /// <summary>
        /// Get an object of interpreter
        /// </summary>
        /// <param name="propertyName">Variable Name</param>
        /// <returns></returns>
        object GetObject(string propertyName);

        /// <summary>
        /// Get an object of interpreter
        /// </summary>
        /// <param name="propertyName">Variable Name</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns></returns>
        T GetObject<T>(string propertyName, T defaultValue);

        /// <summary>
        /// Get an object of interpreter
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, object>> GetObjects();

        /// <summary>
        /// Add an Internal Object to Engine
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="obj">The object to introduce to engine</param>
        /// <returns></returns>
        void InjectObject(string name, object obj);

        /// <summary>
        /// Add an Internal Type to Engine
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="type">The Type to introduce to engine</param>
        /// <returns></returns>
        void InjectType(string name, Type type);
        void InjectType(Type type);

        /// <summary>
        /// Add an Internal Assembly to Engine
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="assemblyNames">Assembly names</param>
        /// <returns></returns>
        void InjectAssembly(string name, params string[] assemblyNames);

        /// <summary>
        /// Add an Internal Assembly to Engine
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="assemblies">Assemblies</param>
        /// <returns></returns>
        void InjectAssembly(string name, params Assembly[] assemblies);
       
        /// <summary>
        /// Initialize the engine
        /// </summary>
        /// <returns></returns>
        void Initialize();
        void Initialize(bool injectBasic, bool injectDefault);
        void Initialize(params string[] rootAssemblies);
        void Finalize();

        void InjectBasics();
        void InjectDefaults();
        void InjectDefaultAssemblies();
        void InjectDefaultTypes();
        void InjectDefaultObjects();


        void Interrupt();

        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="codes">The code</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        T Evaluate<T>(string codes, T defaultValue = default);
        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="code">The concurrent codes</param>
        /// <param name="documentName">The concurrent name</param>
        object Evaluate(string documentName, string code);
        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="code">The code</param>
        /// <returns></returns>
        object Evaluate(string code);
        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="pathOrScript">The code</param>
        /// <returns></returns>
        object[] Evaluate(string[] pathOrScript, bool withException = true);
        /// <summary>
        /// Evaluate Codes in the Files
        /// </summary>
        /// <param name="pathOrScript">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        IEnumerable<object> Evaluate(bool withException, params string[] pathOrScript);

        /// <summary>
        /// Execute Codes
        /// </summary>
        /// <param name="code">The concurrent codes</param>
        /// <param name="documentName">The concurrent name</param>
        void Execute(string documentName, string code);
        /// <summary>
        /// Execute Codes
        /// </summary>
        /// <param name="code">The code</param>
        /// <returns></returns>
        void Execute(string code);
        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="pathOrScript">The code</param>
        /// <returns></returns>
        void Execute(string[] pathOrScript, bool withException = true);
        /// <summary>
        /// Execute Codes in the Files
        /// </summary>
        /// <param name="pathOrScript">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        void Execute(bool withException, params string[] pathOrScript);

        /// <summary>
        /// Execute Codes
        /// </summary>
        /// <param name="code">The concurrent codes</param>
        /// <param name="documentName">The concurrent name</param>
        string ExecuteCommand(string documentName, string code);
        /// <summary>
        /// Execute Codes
        /// </summary>
        /// <param name="codes">The code</param>
        /// <returns></returns>
        string ExecuteCommand(string codes);
        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="pathOrScript">The code</param>
        /// <returns></returns>
        string[] ExecuteCommand(string[] pathOrScript, bool withException = true);
        /// <summary>
        /// Execute Codes in the Files
        /// </summary>
        /// <param name="pathOrScript">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        IEnumerable<string> ExecuteCommand(bool withException, params string[] pathOrScript);


        /// <summary>
        /// Embed Modules
        /// </summary>
        /// <param name="code">The concurrent codes</param>
        /// <param name="documentName">The concurrent name</param>
        object Module(string documentName, string code);
        /// <summary>
        /// Embed Modules
        /// </summary>
        /// <param name="codes">The code</param>
        /// <returns></returns>
        object Module(string codes);
        /// <summary>
        /// Evaluate Modules
        /// </summary>
        /// <param name="codes">The code</param>
        /// <returns></returns>
        object[] Module(string[] pathOrScript, bool withException = true);
        /// <summary>
        /// Embed Modules
        /// </summary>
        /// <param name="pathOrScript">The Module codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        IEnumerable<object> Module(bool withException, params string[] pathOrScript);


        object Use(bool withExceptions = true);
        object Use(string pathOrScript, bool withExceptions = true);
        object Use(string pathOrScript, string extension, bool withExceptions = true);
        object[] Use(string[] pathOrScript, bool withExceptions = true);
        IEnumerable<object> Use(bool withException, params string[] pathOrScript);

        Task UseAsync(bool withExceptions = true);
        Task UseAsync(string pathOrScript, bool withExceptions = true);
        Task UseAsync(string pathOrScript, string extension, bool withExceptions = true);
        Task[] UseAsync(string[] pathOrScript, bool withExceptions = true);
        IEnumerable<Task> UseAsync(bool withException, params string[] paths);


        object Once(bool withExceptions = true);
        object Once(string pathOrScript, bool withExceptions = true);
        object Once(string pathOrScript, string extension, bool withExceptions = true);
        object[] Once(string[] pathOrScript, bool withExceptions = true);
        IEnumerable<object> Once(bool withException, params string[] pathOrScript);

        Task OnceAsync(bool withExceptions = true);
        Task OnceAsync(string pathOrScript, bool withExceptions = true);
        Task OnceAsync(string pathOrScript, string extension, bool withExceptions = true);
        Task[] OnceAsync(string[] pathOrScript, bool withExceptions = true);
        IEnumerable<Task> OnceAsync(bool withException, params string[] paths);
    }
}
