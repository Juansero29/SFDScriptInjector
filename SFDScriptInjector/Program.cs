﻿using SFDScriptInjector.Extensions;
using SFDScriptInjector.Model.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SFDScriptInjector
{
    static class Program
    {
        private static string ScriptFilePath;

        private static string MapFilePath;

        static async Task<int> Main(string[] args)
        {
            try
            {
                await GetAndCheckFilePathsFromArguments(args);
                var scriptToInject = await PrepareScriptToInject();
                await InjectScriptIntoMap(scriptToInject);
                WriteEndingMessagesToConsole();
                return 0;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + e.Message);
                Console.ResetColor();
                return 1;
            }
        }

        private static void WriteEndingMessagesToConsole()
        {
            Console.WriteLine($"Injected map saved to {MapFilePath}");
            Console.WriteLine($"Press 'ENTER' to exit...");
            Console.ReadLine();
        }

        private static async Task InjectScriptIntoMap(Script scriptToInject)
        {
            var map = new Map()
            {
                Path = MapFilePath
            };

            var scriptInjector = new ScriptInjector();
            await scriptInjector.InjectScriptIntoMap(scriptToInject, map);
            await scriptInjector.SaveInjectedMap();
        }

        private static async Task<Script> PrepareScriptToInject()
        {
            var scriptLoader = new ScriptLoader();
            var scriptToInject = await scriptLoader.LoadScriptAsync(ScriptFilePath, scriptSurroundingRegionName: "Script To Copy");
            if (ScriptFilePath.Contains("Hardcore"))
            {
                var mapScript = await scriptLoader.LoadScriptAsync(GetMapScriptFilePath(), scriptSurroundingRegionName: "Map Dependant Data");
                scriptToInject = await ScriptAggregator.MergeIntoOneScript(mapScript, scriptToInject);
            }
            return scriptToInject;
        }

        private static string GetMapScriptFilePath()
        {
            var lastSlashIndex = MapFilePath.LastIndexOf("\\");
            var mapDependantDataFolder = MapFilePath.Substring(0, MapFilePath.Substring(0, lastSlashIndex).LastIndexOf("\\") + 1);
            var mapFileName = MapFilePath.Substring(lastSlashIndex + 1, (MapFilePath.Length - 1) - lastSlashIndex);
            var mapClassFileName = mapFileName.Replace(".sfdm", ".cs").Replace(" ", string.Empty);
            var mapClassFilePath = mapDependantDataFolder + mapClassFileName;
            return mapClassFilePath;
        }

        #region Get & Check Arguments
        private static async Task GetAndCheckFilePathsFromArguments(string[] args)
        {
            await Task.Run(() =>
            {
                VerifyArgumentsAndGetFilePaths(args);
            });
        }

        private static void VerifyArgumentsAndGetFilePaths(string[] args)
        {
            CheckThatArgumentsAreFilled(args);
            CheckAndRecoverBothFilePaths(args);
        }

        private static void CheckThatArgumentsAreFilled(string[] args)
        {
            if (!ThereAreEnoughArguments(args))
            {
                throw new InvalidOperationException("Use the app by passing a C# file path as the first parameter and a SFD Map (*.sfdm) as a second parameter.\nExample: .\\SFDScriptInjector C:\\Script\\FileContainingScript.cs C:\\Maps\\MapToBeInjected.sfdm");
            }
        }

        private static void CheckAndRecoverBothFilePaths(string[] args)
        {
            ScriptFilePath = RecoverFirstArgument(args);
            MapFilePath = RecoverSecondArgument(args);

            CheckThatBothFilesExist(ScriptFilePath, MapFilePath);
        }

        private static void CheckThatBothFilesExist(string cSharpFilePath, string sfdMapFilePath)
        {
            if (!File.Exists(cSharpFilePath))
                throw new InvalidOperationException($"A file does not exist at path: \"{cSharpFilePath}\"");

            if (!File.Exists(sfdMapFilePath))
                throw new InvalidOperationException($"A file does not exist at path: \"{sfdMapFilePath}\"");
        }

        private static string RecoverSecondArgument(string[] args)
        {
            return args[1];
        }

        private static string RecoverFirstArgument(string[] args)
        {
            return args[0];
        }

        private static bool ThereAreEnoughArguments(string[] args)
        {
            return args.Length > 1;
        }

        #endregion


    }
}
