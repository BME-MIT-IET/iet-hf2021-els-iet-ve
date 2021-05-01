using RDFSharp.Model;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace RDFSharp.Convert
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            RootCommand root = new RootCommand("Convert between different file formats supported by RDFSharp.");

            root.AddArgument(new Argument<RDFModelEnums.RDFFormats>("formatSource"));
            root.AddArgument(new Argument<FileInfo>("fileSource").ExistingOnly());
            root.AddArgument(new Argument<RDFModelEnums.RDFFormats>("formatDestination"));
            root.AddArgument(new Argument<FileInfo>("fileDestination").LegalFilePathsOnly());

            root.Handler = CommandHandler.Create<RDFModelEnums.RDFFormats, FileInfo, RDFModelEnums.RDFFormats, FileInfo>(Convert);

            root.Invoke(args);
        }

        public static void Convert(RDFModelEnums.RDFFormats formatSource, FileInfo fileSource, RDFModelEnums.RDFFormats formatDestination, FileInfo fileDestination)
        {
            RDFGraph graph = RDFGraph.FromFile(formatSource, fileSource.FullName);
            graph.ToFile(formatDestination, fileDestination.FullName);
        }
    }
}