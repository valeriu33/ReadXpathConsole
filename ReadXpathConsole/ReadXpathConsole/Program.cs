using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CommandLine;
using CsvHelper;
using ReadXpathConsole.Exceptions;

namespace ReadXpathConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<ConsoleArguments>(args)
                    .WithParsed(ReadXpath);
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void ReadXpath(ConsoleArguments args)
        {
            if (!File.Exists(args.OutputFilePath))
                File.Create(args.OutputFilePath);

            if (!Directory.Exists(args.XmlsFolderPath))
                throw new WarningException("The path to the output folder doesn't exist");
            
            var listOfActions = new List<Action>();
            var documentsNames = Directory.GetFiles(args.XmlsFolderPath);
            
            if (documentsNames.Any(documentName => Path.GetExtension(documentName) != ".xml"))
                throw new WrongPathException("The path contains a file without XML extension");

            using (var writer = new StreamWriter(args.OutputFilePath))
            {
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteHeader(typeof(CsvModel));
                    csvWriter.NextRecord();
                    writer.Flush();
                }
            }
            foreach (var documentName in documentsNames)
            {
                listOfActions.Add(() =>
                {
                    var result = ReadXpathFromFile(args, documentName);

                    AddToCsv(args, Path.GetFileName(documentName), result);
                });
            }
            
            var options = new ParallelOptions { MaxDegreeOfParallelism = args.NumberOfThreads };
            Parallel.Invoke(options, listOfActions.ToArray());
        }

        private static string ReadXpathFromFile(ConsoleArguments args, string documentName)
        {
            var doc = new XmlDocument();   
            doc.Load(Path.Combine(args.XmlsFolderPath, documentName));
                
            XmlNode? root = doc.DocumentElement;
            if (root == null)
                throw new WrongDocumentException("The document contains no root element");
                
            var xmlNamespaceManager = new XmlNamespaceManager(doc.NameTable);
            xmlNamespaceManager.AddNamespace("ns", "http://www.tempuri.org/XML");

            var node = root.SelectSingleNode(args.XpathExpression, xmlNamespaceManager);
            if (node == null)
                throw new WrongDocumentException($"The path: {args.XpathExpression} doesn't exist");

            return node.InnerXml;
        }

        private static void AddToCsv(ConsoleArguments args, string fileName, string value)
        {
            using var writer = new StreamWriter(args.OutputFilePath);
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csvWriter.WriteRecord(new CsvModel { FileName = fileName, Value = value });
            csvWriter.NextRecord();
            
            writer.Flush();
        }
    }
}