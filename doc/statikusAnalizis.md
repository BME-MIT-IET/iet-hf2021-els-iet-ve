# Manuális kód átvizsgálás + Statikus analízis eszköz futtatása

A feladat célja a projekt kódjának átvizsgálása statikus elemzési eszközökkel. Az elemzéshez a SonarLint-et futtatam, parancssorból, Roslyn analizátor formájában:

```msbuild /t:rebuild /p:ErrorLog=static.sarif```

Az így kapott log fájlt a Microsoft SARIF Viewer VS bővítménnyel nyitottam meg.

A következőkben láthatók a talált hibák, és értékelésük:

## S101 Types should be named in PascalCase.

Ez a figyelmeztetés a projekt összes osztályára megjelent, mivel mind RDF-el kezdődik. (`RDFCollection`, `RDFContainer`, `RDFDataSource`, stb.) Ez konvenció kérdése, a kód nem hibás.

##  S1066 Merge this if statement with the enclosing one.

Ez is egy stilisztikai probléma, ami sok helyen előfordul a kódban.

```
if (patternGroup != null)
{
    if (!this.GetPatternGroups().Any(q => q.Equals(patternGroup)))
    {
        this.QueryMembers.Add(patternGroup);
    }
}
```

Az ilyen esetekben a két egymásba ágyazott `if` helyett elég lenne egy, a két feltétel konjunkciójáva. Az itt használt módszerrel azonban egyértelműen jelezve van, hogy a `null` ellenőrzés a többi feltétel kiértékelése előtt történik.

## S1125 Remove the unnecessary Boolean literal(s).

Ez a hiba ilyen típusú konstrukciók formájában jelenik meg:

```aConcept != null && bConcept != null && conceptScheme != null ? conceptScheme.GetExactMatchConceptsOf(aConcept).Concepts.ContainsKey(bConcept.PatternMemberID) : false```

Ami egyszerűbben kifejezve:

```aConcept != null && bConcept != null && conceptScheme != null && conceptScheme.GetExactMatchConceptsOf(aConcept).Concepts.ContainsKey(bConcept.PatternMemberID)```

Ez ismét egy próbálkozásnak tűnik annak az explicit jelzésére, hogy a `null` ellenőrzés a függvényhívás előtt történik, véleményem szerint inkább csak rontja a kód olvashatóságát.

## S1075 Refactor your code not to use hardcoded absolute paths or URIs.

Ez a jelzés az `RDFVocabulary` alosztályaiban jelent meg. Általános esetben ennek a figyelmeztetésnek a célja, hogy a program ne függjen hard-kódolt szerver útvonalaktól, könnyítve ezzel a tesztelést. Ebben az esetben teljesen másról van szó, ezek szabványos RDF sémákhoz tartozó URI-k.

## S3218 Rename this field to not shadow the outer class' member with the same name.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/Vocabularies/SKOS.cs#L224

Ez ismét az `RDFVocabulary`-ra jellemző figyelmeztetés. Itt végülis ez elfogadható, nem hiszem, hogy gondot okozna.

## S108 Either remove or fill this block of code.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Semantics/OWL/Ontology/RDFOntologyHelper.cs#L1544-L1556

Ez egy valódi hiba. Jelen állapotában a `try` blokk így nem szolgál semmilyen célt. Lehetséges, hogy `finally` helyett valamilyen kivételt kellett volna itt elkapni és lenyelni (`catch(Exception) { }`), ám ez sem egy ajánlott művelet...

## S112 'System.Exception' should not be thrown by user code.

Ez sok helyen megjelenik a kódban. Exception típusú kivételeket nem illik közvetlenül dobni, mert így a felhasználó kód nem tud specifikusan egy hibára szűrni. Az Exception leszármazottjainak használta ajánlott. Ennek ellenére ez a probléma sok projektben megjelenik.

## S1168 Return an empty collection instead of null.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/Validation/Abstractions/RDFShapesGraph.cs#L143-L154

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/Validation/RDFValidationHelper.cs#L150-L163

A visszaadott típus implementálja az IEnumerable interfészt, de nem beépített kollekció, szóval elvileg lehetne értelmes jelentése a `null` válasznak. Itt viszont csak akkor `null` a kimenet, ha a bemenet is null volt. Ebben az estben inkább `ArgumentNullException`-t kéne dobni.

## S1172 Remove this unused method parameter.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/Serializers/RDFTurtle.cs#L327

Érdekes módon ez a paraméter már a fájl első verziójában is szerepel, nem szolgál semmi célt. A Turtle szerializáló másik két privát metódusában is megjelenik egy nem használt paraméter.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Query/Mirella/RDFQueryPrinter.cs#L44-L45

Nagyon nehéz megállapítani, hogy ennek a kódrésznek pontosan mi a célja. Lehetséges, hogy a tartalmazó függvény `indentLevel` paramétere helyett a lokális függvény `indLevel` paraméterét kellett volna használni, ám a lokális függvény összes meghívásakor épp az `indentLevel`-t kapja paraméterül. Itt nagy valószínűséggel nincs is szükség lokális függvényekre, a kód érthetetlen, refaktorálni kéne...

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Semantics/OWL/Ontology/RDFOntologyHelper.cs#L2131

Ez a hiba már szintén a kód első verziójában jelen volt.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Semantics/OWL/RDFSemanticsUtilities.cs#L177

Ez a fájl szintén tartalmaz rejtélyesen haszontalan paramétereket.

## S125 Remove this commented out code.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/Serializers/RDFXml.cs#L720

Ez igazából nem kód, csak a SonarLint hitte annak a sorvégi `;` miatt.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Query/Mirella/RDFQueryPrinter.cs#L906

Ez valószínűleg diagnosztikai célt szolgált. Ki kéne törölni.

## S1481 Remove the unused local variable.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/Serializers/RDFXml.cs#L217

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Query/RDFQueryUtilities.cs#L236

Itt igazából a konstruktor meghívása végzi el a bemenő string értékének validálását a `RDFModelUtilities.ValidateTypedLiteral` függvényen keresztül. Ez nem a legszebb megoldás, inkább át kéne írni a `ValidateTypedLiteral`-t hogy `RDFTypedLiteral` létrehozása nélkül is hívható legyen.

## S1854 Remove this useless assignment to local variable.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/Serializers/RDFNTriples.cs#L247-L254

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Store/Serializers/RDFNQuads.cs#L238-L247

Ezeket a változókat a cikluson belül kéne deklarálni.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/Serializers/RDFTurtle.cs#L491-L496

Itt a két `ReadCodePoint` hívásnál fölösleges az értéket eltárolni `bufChar`-ba. A `ReadCodePoint` csak arra szolgál, hogy eldobja az előzőleg `PeekCodePoint`-al kiolvasott karaktert. Ez a hiba végső soron megbocsájtható.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Query/Mirella/RDFQueryEngine.cs#L1311

Nincs szükség létrehozni üres listát, hiszen mindegyik ágban értékül adunk a változónak egy másik listát, mielőtt kiolvasnánk. Még jobb lenne, ha ez a változó a `PopulateTable` hívások blokkjaiban lenne deklarálva.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Query/Mirella/RDFQueryEngine.cs#L1446

Itt szükség van a külső blokkban deklarált változóra, mivel ezzel tér vissza a függvény, az értékadás viszont nem szükséges. Ha valamelyik ágban nem adnánk neki értéket, az úgyis fordítási hibát eredményezne.

Ugyan ez a minta pár helyen az `OntologyHelper` osztályban is megtalálható.

## S1871 Either merge this branch with the identical one or change one of the implementations.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/RDFModelUtilities.cs#L778-L918

Ennek a kódrészletnek a célja, hogy ellenőrizze a dátumok formátumát és eltávolítsa az időzóna információkat. Itt nagyon sok a kódismétlés, érdemes lenne bevezetni egy dátum ellenőrző segédfüggvényt, vagy legalább összevonni az egyező törzsű `if` párokat.

## S2223 Change the visibility of 'NumberRegex' or make it 'const' or 'readonly'.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Semantics/OWL/RDFSemanticsUtilities.cs#L32

A változó `readonly` és `private` kéne, hogy legyen. Csak ez az osztály fér hozzá, és csak olvasás céljából.

## S2234 Parameters to 'CheckIsTransitiveAssertionOf' have the same names but not the same order as the method arguments.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Semantics/OWL/Ontology/RDFOntologyChecker.cs#L152-L157

A függvény leírásából látható, hogy itt valóban a fordított sorrendben való hívás a helyes, a jelzés fals pozitív.

## S2259 'subjNode' is null on at least one execution path.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/Serializers/RDFXml.cs#L201

Ez a figyelmeztetés is fals pozitívnak tűnik. A jelzést az okozhatja, hogy a fenti `switch` nem rendelkezik `default` ággal, de igazából az enum összes megengedett értékét lefedi.

## S2589 Change this condition so that it does not always evaluate to 'true'.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Semantics/SKOS/RDFSKOSHelper.cs#L871-L872

Ez a hiba az `RDFSKOSHelper` osztályban sok helyen szerepel. A paraméter `null` értékét még az előtt kéne ellenőrizni, hogy hozzáférnénk egy tulajdonságához, `ArgumentNullException`-t kéne dobni.

## S2755 Disable access to external entities in XML parsing.

Az RDFSharp szerializálói explicit bekapcsolják a Document Type Definition ellenőrzést az XML olvasókon:

```reader.DtdProcessing = DtdProcessing.Parse;```

Ezt általában érdemes kikapcsolni, mert így a program sérülékeny lehet XML External Entities típusú támadásokra. Legalább az `XmlUrlResolver`-t konfigurálni kéne, hogy ne érjen el külső erőforrásokat.

## S2953 Either implement 'IDisposable.Dispose', or totally rename this method to prevent confusion.

Az `RDFOntologyPropertyModel` helyesen implementálja a Dispose mintát, ám valamiért nem származik az `IDisposable` interfészből. Ez valószínűleg véletlenül maradt le.

Az IDisposable interfész implementációja a könyvtárban kicsit furcsa, hiszen sehol sem kezel unmanaged erőforrásokat. A Dispose itt csak arra szolgál, hogy kiürítse az osztályban található listákat. Nem tudom megállapítani, hogy ennek milyen haszna lehet, a témáról szóló issue-t úgy tűnik törölték:

https://github.com/mdesalvo/RDFSharp/commit/fda76e7074a5a44bc9223e805178fb1ae6be6532
https://github.com/mdesalvo/RDFSharp/issues/190

## S3060 Offload the code that's conditional on this 'is' test to the appropriate subclass and remove the test.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/RDFDataSource.cs#L29-L61

Az ilyen fajta `is`-es típus ellenőrzések már magukban nem megfelelő OO tervezésre utalnak. Mivel itt a típusellenőrzések már a projekt szerves részét képezik, elég nehéz lenne ezen javítani. Legalább azzal lehetne szépíteni a dolgon, ha az `RDFDataSource`-ban ezek a metódusok virtuálisak lennének, `false` eredményt adnának vissza alapból, és a leszármazottak felülírhatnák `true`-ra. Így a `RDFDataSource`-nak nem kéne közvetlenül ismerni a leszármazottjait.

## S3247 Replace this type-check-and-cast sequence with an 'as' and a null check.

https://github.com/BME-MIT-IET/iet-hf2021-els-iet-ve/blob/1d8b3f151de5fb85fbd752dc2119453585f68647/RDFSharp/Model/Validation/RDFValidationHelper.cs#L501-L502

Az ilyen típusú ellenőrzések nagyon gyakoriak a kódban:

```
if (thing is T)
    doSomethingWith((T)thing);
```

Eltekintve attól, hogy a típus ellenőrzés nem szép dolog, ez a módszer nem optimális. Itt igazából kétszer történik típusellenőrzés, egyszer az `is` által, és még egyszer a kasztolás során. Helyette inkább a C# 7-től elérhető Pattern Matching funkciót lenne érdemes használni:

```
if (thing is T t)
    doSomethingWith(t);
```