using CommandLine;

namespace ReadXpathConsole
{
    public class ConsoleArguments
    {
        [Option('e', "xpathExpression", Required = true, HelpText = "XPATH to the value.")]
        public string XpathExpression { get; set; }
        
        [Option('n', "numberOfThreds", Required = true, HelpText = "Number of threads to use.", Default = 4)]
        public int NumberOfThreads { get; set; }
        
        [Option('p', "xmlsFolderPath", Required = true, HelpText = "Path for the XML file.")]
        public string XmlsFolderPath { get; set; }
        
        [Option('o', "outputFilePath", Required = true, HelpText = "Output file.")]
        public string OutputFilePath { get; set; }
    }
}