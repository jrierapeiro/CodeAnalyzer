What is this repository for?

Objectives:
  - We want to have a tool that checks our code using our custom diagnostics
  - We want to get a report about all the code references in a project, moreover, we want to apply custom rules to carry out this report:
  - The TestClasses are excluded for the report.
  - The constructor without parameters are excluded for the report.
  - The non-static public actions inside a MVC controller shouldn't have references. The report show a warning if this    condition is not accomplished.
  - All method and property without references will be highlighted in the report.
    
CodeAnalyzer has been written using some of the new C# features (write the code), the Roslyn API (compilation, solution, references, etc…), our custom diagnostics (old terms and async methods) and RazorEngine (generate the html output).

https://twitter.com/jriera.

How do I get set up?    http://www.visualstudio.com/en-us/downloads/visual-studio-2015-downloads-vs
Based on Roslyn: https://roslyn.codeplex.com/   https://www.nuget.org/profiles/RoslynTeam
  
