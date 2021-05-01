using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static RDFSharp.Model.RDFVocabulary;

namespace RDFSharp.UnitTests
{
    [TestClass]
    public class QueryTest
    {
        RDFGraph graph;
        //RDFQuery query;

        #region selection tests
        [TestMethod]
        public void TestSelectAll()
        {
            // Arrange
            graph = new RDFGraph();
            RDFSelectQuery query;
            BuildGraph(graph);

            // Act 
            query = BuildSelectAllQuery();
            var result = query.ApplyToGraph(graph);
            Console.WriteLine(query.ToString());

            // Assert
            Assert.AreEqual(30, result.SelectResultsCount);
        }

        [TestMethod]
        public void TestSelectRegexFilter()
        {
            // Arrange
            graph = new RDFGraph();
            RDFSelectQuery query;
            BuildGraph(graph);

            // Act 
            query = new RDFSelectQuery();
            var x = new RDFVariable("x");
            var y = new RDFVariable("y");
            var z = new RDFVariable("z");
            var patternGroup = new RDFPatternGroup("PatternGroup1").AddPattern(new RDFPattern(x, y, z));

            var filter = new RDFRegexFilter(x, new Regex(@".1$", RegexOptions.IgnoreCase)); // Filter subjects ending with 1
            patternGroup.AddFilter(filter);

            query.AddPatternGroup(patternGroup)
                .AddProjectionVariable(x)
                .AddProjectionVariable(y)
                .AddProjectionVariable(z);

            var result = query.ApplyToGraph(graph);

            // Assert
            Assert.AreEqual(3, result.SelectResultsCount);
        }

        [TestMethod]
        public void TestSelectDistinctModifier()
        {
            // Arrange
            graph = new RDFGraph();
            RDFSelectQuery query;
            BuildGraph(graph);
            graph.AddTriple(        // add duplicate triple
                new RDFTriple(
                        new RDFResource(string.Concat(RDF.BASE_URI, "example_subject" + 0)),
                        RDF.TYPE,
                        new RDFResource(string.Concat(RDF.BASE_URI, "example_object" + 0))
                    )
                );

            // Act 
            query = BuildSelectAllQuery();
            query.AddModifier(new RDFDistinctModifier());
            var result = query.ApplyToGraph(graph);

            // Assert
            Assert.AreEqual(30, result.SelectResultsCount);
        }

        [TestMethod]
        public void TestSelectLimitModifier()
        {
            // Arrange
            graph = new RDFGraph();
            RDFSelectQuery query;
            BuildGraph(graph);

            // Act 
            query = BuildSelectAllQuery();
            query.AddModifier(new RDFLimitModifier(5));

            var result = query.ApplyToGraph(graph);

            // Assert
            Assert.AreEqual(5, result.SelectResultsCount);
        }

        [TestMethod]
        public void TestSelectOffsetModifier()
        {
            // Arrange
            graph = new RDFGraph();
            RDFSelectQuery query;
            BuildGraph(graph);

            // Act 
            query = BuildSelectAllQuery();
            query.AddModifier(new RDFOffsetModifier(10));

            var result = query.ApplyToGraph(graph);

            // Assert
            Assert.AreEqual(20, result.SelectResultsCount);
        }

        [TestMethod]
        public void TestSelectNumericAggregators()
        {
            // Arrange
            graph = new RDFGraph();
            RDFSelectQuery query;
            BuildGraphWithValues(graph);

            // Act 
            query = new RDFSelectQuery();
            var x = new RDFVariable("x");
            var type = new RDFVariable("type");
            var value = new RDFVariable("value");
            var patternGroup1 = new RDFPatternGroup("PatternGroup1").AddPattern(new RDFPattern(x, RDF.TYPE, type));
            var patternGroup2 = new RDFPatternGroup("PatternGroup2").AddPattern(new RDFPattern(x, RDF.VALUE, value));
            query.AddPatternGroup(patternGroup1)
                .AddPatternGroup(patternGroup2)
                .AddProjectionVariable(type);

            var gm = new RDFGroupByModifier(new List<RDFVariable>() { type });
            gm.AddAggregator(new RDFAvgAggregator(value, new RDFVariable("avg")));
            gm.AddAggregator(new RDFSumAggregator(value, new RDFVariable("sum")));
            gm.AddAggregator(new RDFCountAggregator(value, new RDFVariable("count")));
            gm.AddAggregator(new RDFMinAggregator(value, new RDFVariable("min"), RDFQueryEnums.RDFMinMaxAggregatorFlavors.Numeric));
            gm.AddAggregator(new RDFMaxAggregator(value, new RDFVariable("max"), RDFQueryEnums.RDFMinMaxAggregatorFlavors.Numeric));

            query.AddModifier(gm);

            var result = query.ApplyToGraph(graph);

            // Assert
            var row = result.SelectResults.AsEnumerable().ElementAt(0);

            // average
            string resultStr = (string)row.ItemArray.ElementAt(1);
            int resultNum = int.Parse(resultStr.Split('^')[0]);
            Assert.AreEqual(30, resultNum);
            // sum
            resultStr = (string)row.ItemArray.ElementAt(2);
            resultNum = int.Parse(resultStr.Split('^')[0]);
            Assert.AreEqual(150, resultNum);
            // count
            resultStr = (string)row.ItemArray.ElementAt(3);
            resultNum = int.Parse(resultStr.Split('^')[0]);
            Assert.AreEqual(5, resultNum);
            // min
            resultStr = (string)row.ItemArray.ElementAt(4);
            resultNum = int.Parse(resultStr.Split('^')[0]);
            Assert.AreEqual(10, resultNum);
            // max
            resultStr = (string)row.ItemArray.ElementAt(5);
            resultNum = int.Parse(resultStr.Split('^')[0]);
            Assert.AreEqual(50, resultNum);
        }

        #endregion

        #region helper methods
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

        private void BuildGraphWithValues(RDFGraph grap)
        {
            const int SUBJ_COUNT = 5;
            const int VAL_COUNT = 5;

            RDFResource obj = new RDFResource(string.Concat(RDF.BASE_URI, "example_object"));
            for (int j = 0; j < SUBJ_COUNT; j++)
            {
                RDFResource tempSubject = new RDFResource(string.Concat(RDF.BASE_URI, "example_subject" + j));
                graph.AddTriple(new RDFTriple(tempSubject, RDF.TYPE, obj));
                for (int k = 0; k < VAL_COUNT; k++)
                {
                    int tempValue = (j + 1) * 10;
                    graph.AddTriple(new RDFTriple(
                        tempSubject,
                        RDF.VALUE,
                        new RDFTypedLiteral(tempValue.ToString(), RDFModelEnums.RDFDatatypes.XSD_INTEGER)));
                }
            }
        }

        private RDFSelectQuery BuildSelectAllQuery(RDFFilter filter = null)
        {
            var query = new RDFSelectQuery();
            var x = new RDFVariable("x");
            var y = new RDFVariable("y");
            var z = new RDFVariable("z");
            var patternGroup = new RDFPatternGroup("PatternGroup1").AddPattern(new RDFPattern(x, y, z));

            if (filter != null)
                patternGroup.AddFilter(filter);

            query.AddPatternGroup(patternGroup)
                .AddProjectionVariable(x)
                .AddProjectionVariable(y)
                .AddProjectionVariable(z);

            return query;
        }
        #endregion
    }
}

