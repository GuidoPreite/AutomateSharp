# Automate Sharp
Automate Sharp is a Dataverse Custom API to run C# code inside a Power Automate Cloud Flow.

Supported syntax is C# 4.0, it allows the <i>dynamic</i> keyword but doesn't support <a target="_blank" href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated">String interpolation</a>
<h2>Instructions</h2>
<ol>
  <li>Download the Managed Solution from <a target="_blank" href="https://github.com/GuidoPreite/AutomateSharp/releases/">Releases</a> page</li>
  <li>Inside the Cloud Flow add a block "Perform an unbound action"

![image](https://github.com/user-attachments/assets/6fe86b68-19a0-4e30-8691-7b16d4dddc3f)
</li>
<li>Select <b>as_AutomateSharp</b> as Action Name, the Custom API has 4 optional input parameters:

  ![image](https://github.com/user-attachments/assets/d00f8045-2aa2-4516-9c23-62a52dcc38e8)
<ul>
  <li><b>InputParameters</b>: string, the value is passed to the code as a variable called "inputParameters"</li>
  <li><b>ReferencedAssemblies</b>: string, a list of assemblies (registered inside the GAC of the environment) separated by a comma</li>
  <li><b>UsingStatements</b>: string, a list of using (from the available assemblies) separated by a semicolon</li>
  <li><b>Code</b>: string, the C# code to execute</li>
</ul>
</li>
<li>The Custom API has 3 output parameters:

![image](https://github.com/user-attachments/assets/5b094272-7e06-4d93-9b65-abf94837e1c2)
<ul>
  <li><b>Success</b>: bool, true if the code has been run, false if there is an exception</li>
  <li><b>OutputParameters</b>: string, the value of a variable called "outputParameters"</li>
  <li><b>ExecptionMessage</b>: string, empty if Success equals true, the exception message if Success equals false</li>
</ul>
</li>
</ol>
<h2>Simple Example</h2>
Reverse a string:
<ul>
  <li>InputParameters: Power Automate</li>
  <li>Code:
    
    char[] array = inputParameters.ToCharArray();
    Array.Reverse(array);
    outputParameters = new String(array);
  </li>
  
  ![image](https://github.com/user-attachments/assets/a869baf5-0573-4a6f-93a7-5a9cffb4f8b4)
</ul>
The result inside the output parameter OutputParameters will be <i>etamotuA rewoP</i>

<h2>JSON Example</h2>
Inside some Dataverse environments the library Newtonsoft.Json is available, if we reference it we can consider the InputParameters as a serialized JSON object and also serialize an object to the OutputParameters variable.

<ul>
  <li>InputParameters: {"number": 5, "text": "Power Automate"}</li>
  <li>ReferencedAssemblies: Newtonsoft.Json.dll</li>
  <li>UsingStatements: using Newtonsoft.Json; using Newtonsoft.Json.Linq;</li>    
  <li>Code:
    
    dynamic dynamicInput = JsonConvert.DeserializeObject(inputParameters);
    int value = dynamicInput.number;
    string str = dynamicInput.text;
    value = value * 3;
    char[] array = str.ToCharArray();
    Array.Reverse(array);
    str = new String(array);
    Array.Reverse(array);
    dynamic result = new ExpandoObject();
    result.P1 = value;
    result.P2 = str;
    outputParameters = JsonConvert.SerializeObject(result);
  </li>
  
  ![image](https://github.com/user-attachments/assets/6340fd04-c0d3-4cf4-84ce-e3a596668a66)
</ul>  
The result inside the output parameter OutputParameters will be <i>{"P1":15,"P2":"etamotuA rewoP"}</i>
