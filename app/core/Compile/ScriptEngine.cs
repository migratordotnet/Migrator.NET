using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace Migrator.Compile
{
    public class ScriptEngine
    {
        public readonly string[] extraReferencedAssemblies;

        private readonly CodeDomProvider _provider;
        private string _codeType = "csharp";

        public ScriptEngine() : this(null, null)
        {
        }

        public ScriptEngine(string[] extraReferencedAssemblies)
            : this(null, extraReferencedAssemblies)
        {
        }

        public ScriptEngine(string codeType, string[] extraReferencedAssemblies)
        {
            if (!String.IsNullOrEmpty(codeType))
                this._codeType = codeType;
            this.extraReferencedAssemblies = extraReferencedAssemblies;

            _provider = CodeDomProvider.CreateProvider(_codeType);
        }

        public Assembly Compile(string directory)
        {
            string[] files =  Directory.GetFiles(directory, String.Format("*.{0}", _provider.FileExtension));
            Console.Out.WriteLine("Compiling:");
            Array.ForEach(files, delegate(String file) { Console.Out.WriteLine(file); });

            return Compile(files);
        }

        public Assembly Compile(params string[] files)
        {
            CompilerParameters parms = SetupCompilerParams();

            CompilerResults compileResult = _provider.CompileAssemblyFromFile(parms, files);
            if (compileResult.Errors.Count != 0)
            {
                foreach (CompilerError err in compileResult.Errors)
                {
                    Console.Error.WriteLine("{0} ({1}:{2})  {3}", err.FileName, err.Line, err.Column, err.ErrorText);
                }
            }
            return compileResult.CompiledAssembly;
        }

        private CompilerParameters SetupCompilerParams()
        {
            CompilerParameters parms = new CompilerParameters();
            parms.GenerateExecutable = false;
            parms.GenerateInMemory = true;
            parms.IncludeDebugInformation = true;

            // Add Default referenced assemblies
            parms.ReferencedAssemblies.Add("System.dll");
            parms.ReferencedAssemblies.Add("System.Data.dll");
            parms.ReferencedAssemblies.Add(FullAssemblyPath("Migrator.Framework.dll"));
            if (null != extraReferencedAssemblies && extraReferencedAssemblies.Length > 0)
            {
                Array.ForEach(extraReferencedAssemblies,
                              delegate(String assemb) { parms.ReferencedAssemblies.Add(assemb); });
            }
            return parms;
        }
        
        private string FullAssemblyPath(string assemblyName)
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            return Path.Combine(Directory.GetParent(assemblyLocation).FullName, "Migrator.Framework.dll");
        }
    }
}