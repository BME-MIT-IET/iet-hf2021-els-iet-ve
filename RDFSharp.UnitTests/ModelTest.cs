using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;
using System;
using System.IO;
using static RDFSharp.Model.RDFVocabulary;

namespace RDFSharp.UnitTests
{
    [TestClass]
    public class ModelTest
    {

        RDFGraph graph;

        [TestMethod]
        public void TestModelBuilding()
        {
            // Arrange
            graph = new RDFGraph();

            // Act 
            BuildGraph(graph);

            // Assert
            Assert.AreEqual(50, graph.TriplesCount);

        }

        [TestMethod]
        public void TestSerialization()
        {
            // Arrange
            graph = new RDFGraph();
            BuildGraph(graph);

            // Act
            SerializeGraph(graph, RDFModelEnums.RDFFormats.NTriples);
            RDFGraph newGraph = DeserializateGraph(RDFModelEnums.RDFFormats.NTriples);

            // Assert
            Assert.AreEqual(graph.TriplesCount, newGraph.TriplesCount);
        }

        private void BuildGraph(RDFGraph graph)
        {
            RDFResource obj = new RDFResource(string.Concat(RDF.BASE_URI, "example_object"));
            for (int i = 0; i < 50; i++)
            {
                RDFResource tempSubject = new RDFResource(string.Concat(RDF.BASE_URI, "example_subject" + i));
                graph.AddTriple(new RDFTriple(tempSubject, RDF.TYPE, obj));
            }
        }

        private void SerializeGraph(RDFGraph graph, RDFModelEnums.RDFFormats format)
        {
            // create file path
            DirectoryInfo di = Directory.CreateDirectory("graphs");

            switch (format)
            {
                case RDFModelEnums.RDFFormats.NTriples:
                    graph.ToFile(RDFModelEnums.RDFFormats.NTriples, "graphs/NTriples.txt");
                    break;
                case RDFModelEnums.RDFFormats.RdfXml:
                    graph.ToFile(RDFModelEnums.RDFFormats.RdfXml, "graphs/RdfXml.txt");
                    break;
                case RDFModelEnums.RDFFormats.TriX:
                    graph.ToFile(RDFModelEnums.RDFFormats.TriX, "graphs/TriX.txt");
                    break;
                case RDFModelEnums.RDFFormats.Turtle:
                    graph.ToFile(RDFModelEnums.RDFFormats.Turtle, "graphs/Turtle.txt");
                    break;
            }
        }

        private RDFGraph DeserializateGraph(RDFModelEnums.RDFFormats format)
        {
            try
            {
                switch (format)
                {
                    case RDFModelEnums.RDFFormats.NTriples:
                        return RDFGraph.FromFile(RDFModelEnums.RDFFormats.NTriples, "graphs/NTriples.txt");
                    case RDFModelEnums.RDFFormats.RdfXml:
                        return RDFGraph.FromFile(RDFModelEnums.RDFFormats.RdfXml, "graphs/RdfXml.txt");
                    case RDFModelEnums.RDFFormats.TriX:
                        return RDFGraph.FromFile(RDFModelEnums.RDFFormats.TriX, "graphs/TriX.txt");
                    case RDFModelEnums.RDFFormats.Turtle:
                        return RDFGraph.FromFile(RDFModelEnums.RDFFormats.Turtle, "graphs/Turtle.txt");
                    default:
                        return null;
                }
            }
            catch (RDFModelException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
