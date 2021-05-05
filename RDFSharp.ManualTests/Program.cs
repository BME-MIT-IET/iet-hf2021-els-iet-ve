using RDFSharp.Model;
using RDFSharp.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace RDFSharp.ManualTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.Unicode;
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            var endpoint = GetSPARQLEndpoint();
            MainLoop(endpoint);
        }


        static RDFSPARQLEndpoint GetSPARQLEndpoint()
        {
            Console.WriteLine("Írd be az SPARQL Endpoint URL-ét amin keresztül elérhető az adatbázis:");
            var endpointUrlString = Console.ReadLine();

            return new RDFSPARQLEndpoint(new Uri(endpointUrlString));
        }



        static void MainLoop(RDFSPARQLEndpoint endpoint)
        {
            var isExitSelected = false;


            while (!isExitSelected)
            {
                Console.WriteLine("Válassz a következő opciók közül (opció kiválasztásához írd be az opció sorszámát):");
                Console.WriteLine("1. Alkotók számának lekérdezése");
                Console.WriteLine("2. Alkotók nevének lekérdezése");
                Console.WriteLine("3. Alkotók keresése név alapján");
                Console.WriteLine("4. Létezik-e az adatbázisban egy festmény adott névvel?");
                Console.WriteLine("5. X. alkotó nevének keresése (ABC sorrendben)");
                Console.WriteLine("0. Kilépés");

                var selectedOption=Console.ReadLine();

                switch (selectedOption)
                {
                    case "1":
                        QueryAllActorsCount(endpoint);
                        break;
                    case "2":
                        QueryAllActorNames(endpoint);
                        break;
                    case "3":
                        QueryActorsByName(endpoint);
                        break;
                    case "4":
                        QueryIfThingExistsByName(endpoint);
                        break;
                    case "5":
                        QueryActorByIndex(endpoint);
                        break;
                    case "0":
                        isExitSelected = true;
                        break;
                    default:
                        Console.WriteLine("A megadott opció nem található a választható opciók között. Próbálj meg a felsorolt opciók közül választani");
                        break;
                }
            }
            
        }

       

        static void QueryAllActorsCount(RDFSPARQLEndpoint endpoint)
        {
            //Variables
            var actor = new RDFVariable("actor");
            var type = new RDFResource(RDFVocabulary.RDF.BASE_URI + "type");
            var actorType = new RDFResource("http://erlangen-crm.org/current/E39_Actor");

            var hasTypeActorPattern = new RDFPattern(actor, type, actorType);

            //Query
            RDFSelectQuery selectQuery = new RDFSelectQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix("ecrm"))
             .AddPatternGroup(new RDFPatternGroup("pg1")
             .AddPattern(hasTypeActorPattern))
             .AddProjectionVariable(actor);

            RDFSelectQueryResult result = selectQuery.ApplyToSPARQLEndpoint(endpoint);

            //Result
            Console.WriteLine("");
            Console.WriteLine("Művészek száma: " + result.SelectResultsCount);
            Console.WriteLine("");
        }

        static void QueryAllActorNames(RDFSPARQLEndpoint endpoint)
        {
            //Variables
            var actor = new RDFVariable("actor");
            var actorAppellation = new RDFVariable("actor_appellation");
            var actorName = new RDFVariable("actor_name");

            var type = new RDFResource(RDFVocabulary.RDF.BASE_URI + "type");
            var isIdentifiedBy = new RDFResource("http://erlangen-crm.org/current/P131_is_identified_by");
            var hasNote = new RDFResource("http://erlangen-crm.org/current/P3_has_note");

            var actorType = new RDFResource("http://erlangen-crm.org/current/E39_Actor");
            var actorApellationType = new RDFResource("http://erlangen-crm.org/current/E82_Actor_Appellation");

            var hasTypeActorPattern = new RDFPattern(actor, type, actorType);
            var isIdentifiedByActorApellationPattern = new RDFPattern(actor, isIdentifiedBy, actorAppellation);
            var hasTypeActorAppellationPattern = new RDFPattern(actorAppellation, type, actorApellationType);
            var hasNotePattern = new RDFPattern(actorAppellation, hasNote, actorName);


            //Query
            RDFSelectQuery selectQuery = new RDFSelectQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix("ecrm"))
             .AddPatternGroup(new RDFPatternGroup("pg1")
             .AddPattern(hasTypeActorPattern)
             .AddPattern(isIdentifiedByActorApellationPattern)
             .AddPattern(hasTypeActorAppellationPattern)
             .AddPattern(hasNotePattern))
             .AddProjectionVariable(actor)
             .AddProjectionVariable(actorName)
             .AddModifier(new RDFOrderByModifier(actorName, RDFQueryEnums.RDFOrderByFlavors.ASC));

            RDFSelectQueryResult result = selectQuery.ApplyToSPARQLEndpoint(endpoint);

            //Result
            Console.WriteLine("");
            Console.Write("Talált sorok száma: ");
            Console.WriteLine(result.SelectResultsCount);
            Console.WriteLine("");

            Console.WriteLine("Az első 100 sor:");

            for (int i = 0; i < 100; i++)
            {
                var row = result.SelectResults.AsEnumerable().ElementAt(i);
                foreach (var item in row.ItemArray)
                {
                    Console.Write("   "+item);
                }
                Console.WriteLine("");
            }
            Console.Write("\n\n");
        }

        static void QueryActorsByName(RDFSPARQLEndpoint endpoint)
        {
            //Input
            Console.WriteLine("írj be egy nevet, vagy annak egy részletét");
            var nameString = Console.ReadLine();

            var nameRegex = new Regex(nameString.Trim(),RegexOptions.IgnoreCase);

            //Variables
            var actor = new RDFVariable("actor");
            var actorAppellation = new RDFVariable("actor_appellation");
            var actorName = new RDFVariable("actor_name");

            var type = new RDFResource(RDFVocabulary.RDF.BASE_URI + "type");
            var isIdentifiedBy = new RDFResource("http://erlangen-crm.org/current/P131_is_identified_by");
            var hasNote = new RDFResource("http://erlangen-crm.org/current/P3_has_note");

            var actorType = new RDFResource("http://erlangen-crm.org/current/E39_Actor");
            var actorApellationType = new RDFResource("http://erlangen-crm.org/current/E82_Actor_Appellation");

            var hasTypeActorPattern = new RDFPattern(actor, type, actorType);
            var isIdentifiedByActorApellationPattern = new RDFPattern(actor, isIdentifiedBy, actorAppellation);
            var hasTypeActorAppellationPattern = new RDFPattern(actorAppellation, type, actorApellationType);
            var hasNotePattern = new RDFPattern(actorAppellation, hasNote, actorName);

            
            //Query
            RDFSelectQuery selectQuery = new RDFSelectQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix("ecrm"))
             .AddPatternGroup(new RDFPatternGroup("pg1")
             .AddPattern(hasTypeActorPattern)
             .AddPattern(isIdentifiedByActorApellationPattern)
             .AddPattern(hasTypeActorAppellationPattern)
             .AddPattern(hasNotePattern)
             .AddFilter(new RDFRegexFilter(actorName, nameRegex)))
             .AddProjectionVariable(actor)
             .AddProjectionVariable(actorName)
             .AddModifier(new RDFOrderByModifier(actorName, RDFQueryEnums.RDFOrderByFlavors.ASC));

            RDFSelectQueryResult result = selectQuery.ApplyToSPARQLEndpoint(endpoint);


            //Result
            Console.WriteLine("");
            Console.Write("Talált sorok száma: " + result.SelectResultsCount);
            Console.WriteLine("");

            Console.WriteLine("Talált sorok: ");

            foreach (var row in result.SelectResults.AsEnumerable())
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write("   " + item);
                }
                Console.WriteLine("");
            }
            Console.Write("\n\n");
        }

        static void QueryIfThingExistsByName(RDFSPARQLEndpoint endpoint)
        {
            //Input
            Console.WriteLine("írd be egy festmény nevét (teljes nevet):");
            var nameString = Console.ReadLine();

            var nameRegex = new Regex("^"+nameString.Trim()+"$",RegexOptions.IgnoreCase);

            //Variables
            var thing = new RDFVariable("thing");
            var label = new RDFVariable("label");

            var type = new RDFResource(RDFVocabulary.RDF.BASE_URI + "type");
            var labelPredicate = new RDFResource(RDFVocabulary.RDFS.BASE_URI + "label");

            var physicalThingType =new RDFResource("http://erlangen-crm.org/current/E18_Physical_Thing");
            

            var thingTypeofPhysicalThingPattern = new RDFPattern(thing, type, physicalThingType);
            var thingHasLabelPattern = new RDFPattern(thing, labelPredicate, label);

            //Query
            var askQuery = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix("ecrm"))
                .AddPatternGroup(new RDFPatternGroup("pg1")
                .AddPattern(thingTypeofPhysicalThingPattern)
                .AddPattern(thingHasLabelPattern)
                .AddFilter(new RDFRegexFilter(label, nameRegex)));

            RDFAskQueryResult result = askQuery.ApplyToSPARQLEndpoint(endpoint);

            //Result
            Console.WriteLine("");
            if (result.AskResult)
            {
                Console.WriteLine("Van ilyen festmény");
            }
            else
            {
                Console.WriteLine("Nincs ilyen festmény");
            }
            Console.Write("\n\n");
        }

        static void QueryActorByIndex(RDFSPARQLEndpoint endpoint)
        {
            //Input
            Console.WriteLine("Hanyadik alkotó nevét szeretnéd visszakapni?");
            var actorIndexString = Console.ReadLine();
            var actorIndex = Int32.Parse(actorIndexString);


            //Variables
            var actor = new RDFVariable("actor");
            var actorAppellation = new RDFVariable("actor_appellation");
            var actorName = new RDFVariable("actor_name");

            var type = new RDFResource(RDFVocabulary.RDF.BASE_URI + "type");
            var isIdentifiedBy = new RDFResource("http://erlangen-crm.org/current/P131_is_identified_by");
            var hasNote = new RDFResource("http://erlangen-crm.org/current/P3_has_note");

            var actorType = new RDFResource("http://erlangen-crm.org/current/E39_Actor");
            var actorApellationType = new RDFResource("http://erlangen-crm.org/current/E82_Actor_Appellation");

            var hasTypeActorPattern = new RDFPattern(actor, type, actorType);
            var isIdentifiedByActorAppellationPattern = new RDFPattern(actor, isIdentifiedBy, actorAppellation);
            var hasTypeActorAppellationPattern = new RDFPattern(actorAppellation, type, actorApellationType);
            var hasNotePattern = new RDFPattern(actorAppellation, hasNote, actorName);


            //Query
            RDFSelectQuery selectQuery = new RDFSelectQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix("ecrm"))
             .AddPatternGroup(new RDFPatternGroup("pg1")
             .AddPattern(hasTypeActorPattern)
             .AddPattern(isIdentifiedByActorAppellationPattern)
             .AddPattern(hasTypeActorAppellationPattern)
             .AddPattern(hasNotePattern))
             .AddProjectionVariable(actorName)
             .AddModifier(new RDFOrderByModifier(actorName, RDFQueryEnums.RDFOrderByFlavors.ASC))
             .AddModifier(new RDFOffsetModifier(actorIndex-1))
             .AddModifier(new RDFLimitModifier(1));

            RDFSelectQueryResult result = selectQuery.ApplyToSPARQLEndpoint(endpoint);

            //Result
            Console.WriteLine("");
            Console.WriteLine(result.SelectResults.AsEnumerable().ElementAt(0).ItemArray.ElementAt(0));
            Console.Write("\n\n");

        }
    }
}
