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

        #region test methods
        [TestMethod]
        public void TestModelBuilding()
        {
            // Arrange
            graph = new RDFGraph();

            // Act 
            BuildGraph(graph, 50);

            // Assert
            Assert.AreEqual(50, graph.TriplesCount);

        }

        [TestMethod]
        public void TestNTriplesSerialization()
        {
            // Arrange
            graph = new RDFGraph();
            BuildGraph(graph, 50);

            // Act
            SerializeGraph(graph, RDFModelEnums.RDFFormats.NTriples);
            RDFGraph newGraph = DeserializateGraph(RDFModelEnums.RDFFormats.NTriples);

            // Assert
            Assert.AreEqual(graph.TriplesCount, newGraph.TriplesCount);
        }

        [TestMethod]
        public void TestRdfXmlSerialization()
        {
            // Arrange
            graph = new RDFGraph();
            BuildGraph(graph, 50);

            // Act
            SerializeGraph(graph, RDFModelEnums.RDFFormats.RdfXml);
            RDFGraph newGraph = DeserializateGraph(RDFModelEnums.RDFFormats.RdfXml);

            // Assert
            Assert.AreEqual(graph.TriplesCount, newGraph.TriplesCount);
        }

        [TestMethod]
        public void TestTriXSerialization()
        {
            // Arrange
            graph = new RDFGraph();
            BuildGraph(graph, 50);

            // Act
            SerializeGraph(graph, RDFModelEnums.RDFFormats.TriX);
            RDFGraph newGraph = DeserializateGraph(RDFModelEnums.RDFFormats.TriX);

            // Assert
            Assert.AreEqual(graph.TriplesCount, newGraph.TriplesCount);
        }

        [TestMethod]
        public void TestTurtleSerialization()
        {
            // Arrange
            graph = new RDFGraph();
            BuildGraph(graph, 50);

            // Act
            SerializeGraph(graph, RDFModelEnums.RDFFormats.Turtle);
            RDFGraph newGraph = DeserializateGraph(RDFModelEnums.RDFFormats.Turtle);

            // Assert
            Assert.AreEqual(graph.TriplesCount, newGraph.TriplesCount);
        }

        [TestMethod]
        public void TestVocabularies()
        {
            // Arrange
            RDFResource actualResource;
            string baseURI = RDFS.BASE_URI;
            string name = "label";
            string expected = baseURI + name;

            // Act
            actualResource = RDFS.LABEL;

            // Assert
            Assert.AreEqual(expected, actualResource.ToString());
        }
        #endregion

        #region helper methods
        private void BuildGraph(RDFGraph graph, int count)
        {
            RDFResource tempObject1 = new RDFResource(string.Concat(RDF.BASE_URI, "example_object_1"));
            RDFResource tempObject2 = new RDFResource(string.Concat(RDF.BASE_URI, "example_object_2"));
            for (int i = 0; i < count; i++)
            {
                RDFResource newSubject = new RDFResource(string.Concat(RDF.BASE_URI, "example_subject_" + i));
                RDFResource newObject = (count < 30) ? tempObject1 : tempObject2;
                graph.AddTriple(new RDFTriple(newSubject, RDF.TYPE, newObject));
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
        #endregion
    }
}
