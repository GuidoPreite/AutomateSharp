using Microsoft.CSharp;
using Microsoft.Xrm.Sdk;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace AutomateSharp
{
    public class RunCSharpCode : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Custom API Inputs
            string inputParameters = context.InputParameters["InputParameters"] as string;
            string referencedAssemblies = context.InputParameters["ReferencedAssemblies"] as string;
            string usingStatements = context.InputParameters["UsingStatements"] as string;
            string code = context.InputParameters["Code"] as string;

            // Custom API Outputs
            bool success = false;
            string outputParameters = "";
            string exceptionMessage = "";

            try
            {
                string[] codeToCompile = {
                "using System; using System.Dynamic;" +
                usingStatements  + // Add user Using Statements
                "namespace AutomateSharp {"  +
                "   public class AutomateSharp {" +
                "       static public string Main(string inputParameters) {" +
                "           string outputParameters = String.Empty;" +
                            code + // Add user Code
                "           return outputParameters;" +
                "       }" +
                "   }" +
                "}"};

                List<string> listReferencedAssemblies = new List<string> { "System.dll", "System.Core.dll", "Microsoft.CSharp.dll" };

                // Add user Referenced Assemblies
                if (!string.IsNullOrWhiteSpace(referencedAssemblies))
                {
                    listReferencedAssemblies.AddRange(referencedAssemblies.Split(','));
                }

                CompilerParameters compilerParameters = new CompilerParameters
                {
                    GenerateInMemory = true,
                    TreatWarningsAsErrors = false,
                    GenerateExecutable = false,
                    CompilerOptions = "/optimize"
                };
                compilerParameters.ReferencedAssemblies.AddRange(listReferencedAssemblies.ToArray());

                CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
                CompilerResults compilerResults = cSharpCodeProvider.CompileAssemblyFromSource(compilerParameters, codeToCompile);

                if (compilerResults.Errors.HasErrors == true)
                {
                    string exception = "Compiler Errors:";
                    foreach (CompilerError ce in compilerResults.Errors) { exception += Environment.NewLine + ce.ToString(); }
                    throw new Exception(exception);
                }

                // Run code
                Module module = compilerResults.CompiledAssembly.GetModules()[0];
                Type moduleType = module.GetType("AutomateSharp.AutomateSharp");
                MethodInfo methodInfo = moduleType.GetMethod("Main");
                object objResult = methodInfo.Invoke(null, new object[] { inputParameters });

                // Set values
                outputParameters = Convert.ToString(objResult);
                success = true;
            }
            catch (Exception ex) { exceptionMessage = ex.Message; }

            // Set OutputParameters values
            context.OutputParameters["Success"] = success;
            context.OutputParameters["OutputParameters"] = outputParameters;
            context.OutputParameters["ExceptionMessage"] = exceptionMessage;
        }
    }
}