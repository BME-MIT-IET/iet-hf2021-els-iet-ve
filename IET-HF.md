# Ellenőrzési Technikák 2. Házi Feladat

A feladat célja, az [RDFSharp](https://github.com/mdesalvo/RDFSharp) könyvtár fejlesztése, CI/CD és egyéb eszközökkel.

Az RDFSharp egy .NET Standard 2.0 könyvtár szemantikus web alkalmazások modellezésére. A könyvtár funkciói, többek között:

- RDF modellek létrehozása és kezelése
- Szabványos RDF formátumok kezelése (N-Triples, TriX, Turtle, RDF/Xml)
- RDF lekérdezések létrehozása és futtatása
- OWL-DL ontológiák kezelése

A házi feladat során a következő területekkel szeretnénk foglalkozni:

## Build keretrendszer + CI beüzemelése (#1)

A projekt automatikus fordítását GitHub Actions segítségével szeretnénk megoldani.

## Deployment segítése (#3)

GitHub Actions segítségével NuGet csomag automatikus létrehozása és feltöltése package repository-ba.

## Egységtesztek készítése + kódlefedettség (#4)

Egységtesztek készítése egy elterjedt .NET Unit Test framework segítségével. Tesztek elkészítése után lefedettség vizsgálata.

A projekt jelenleg egyáltalán nem tartalmaz teszteket, ezen a téren bőven van lehetőség fejlesztésre. 

## Manuális kód átvizsgálás + Statikus analízis eszköz futtatása (#2)

A kódban potenciális hibák, Code Smell-ek keresése kézzel, illetve statikus analízis eszköz segítségével, a talált hibák dokumentálása, lehetőség szerint javítása.

## Nem-funkcionális jellemzők vizsgálata (teljesítmény) (#5)

Benchmark készítése a különböző RDF bemeneti formátumok értelmezési sebességének összehasonlítására. Ebből kiderülne, hol érdemes fejleszteni a könyvtár sebességét.

## Manuális tesztek megtervezése, végrehajtása és dokumentálása (példa alkalmazás) (#6)

Az API használhatóságának elemzése egy egyszerű példaprogram elkészítésével, ami a könyvtár több modulját is felhasználva kapcsolatba lép egy létező RDF adatbázissa.