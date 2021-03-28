# SFDScriptInjector

A simple script injector in the form of a console application for Superfighters Deluxe scripters. It takes a file containing a script and puts it inside an '.sfdm' map

## Prerequisites
The `.cs` file must contain a region named `Script To Copy` in order for to the tool to know which section inside the file to copy.

Example:

```csharp
using SFDGameScriptInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFDScripts
{
    public class Hardcore : GameScriptInterface
    {
        public Hardcore(IGame game = null) : base(game) { }
        
        #region Script To Copy
        
        [Your Script Code Here Must Be Here]

        #endregion
        
    }
}

```

## Usage:

`.\SFDScriptInjector C:\Script\FileContainingScript.cs C:\Maps\MapToBeInjected.sfdm`


