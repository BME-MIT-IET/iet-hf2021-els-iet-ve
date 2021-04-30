using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RDFSharp.Model.RDFVocabulary;

namespace RDFSharp.UnitTests
{
    [TestClass]
    public class QueryTest
    {
        RDFGraph graph;
        //RDFQuery query;

        [TestMethod]
        public void TestSelectAll()
        {
            // Arrange
            graph = new RDFGraph();
            var query = new RDFSelectQuery();

            // Act 
            BuildGraph(graph);
            var x = new RDFVariable("x");
            var y = new RDFVariable("y");
            var z = new RDFVariable("z");
            query.AddPatternGroup(
                new RDFPatternGroup("PatternGroup1").AddPattern(new RDFPattern(x, y, z))
                )
                .AddProjectionVariable(x)
                .AddProjectionVariable(y)
                .AddProjectionVariable(z);
            var result = query.ApplyToGraph(graph);
            
            // Assert
            Assert.AreEqual(30, result.SelectResultsCount);
        }

        private void BuildGraph(RDFGraph graph)
        {
            const int OBJ_COUNT = 3;
            const int SUBJ_COUNT = 10;

            for (int i = 0; i < OBJ_COUNT; i++)
            {
                RDFResource obj = new RDFResource(string.Concat(RDF.BASE_URI, "example_object" + i));
                for (int j = 0; j < SUBJ_COUNT; j++)
                {
                    RDFResource tempSubject = new RDFResource(string.Concat(RDF.BASE_URI, "example_subject" + j));
                    graph.AddTriple(new RDFTriple(tempSubject, RDF.TYPE, obj));
                }
            }
        }
    }
}
