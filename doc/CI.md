# Build keretrendszer és CI

A feladat kiírása szerint be kellett üzemelni egy build keretrendszert, ha még nem lett volna, és folytonos integrációra is fel kellett készíteni a projektet.

## Build keretrendszer

A projektet feltehetően visual studio-ban írták, így automatikusan az MSBuild keretrendszert használja: megvannak ehhez a szükséges konfigurációs fájlok és a standard solution szerkezet. A konkrét build feladatokat a crossplatformos dotnet bináris látja el a CI résznél.

## CI keretrendszer

A CI-hez a Github Actions szolgáltatást választottam.

### Alap build és tesztek

Az alap build folyamat minden branchre (kivéve a masterre) push-olásnál lefut, hogy hamar értesüljenek a fejlesztők a hibákról. Továbbá a masterre való PR kéréskor is megtörténik ez, így csökkentjük a hibázás valószínűségét.
A buildeléskor 5.0.x-es dotnet verziót használunk, ez kompatibilis a projektben meghatározott netstandradt-2.0-val.
Buildelés után a CI lefuttatja a projekt unit tesztjeit is. Ennek segítségével felderítettünk egy linux specifikus hibát, amit a csapat ki is javított.
Tanulság: egy CI-vel, ha különböző platformokon buildelünk (mi esetünkben csak linux), könnyen felderíthetőek a platformfüggő hibák.

### Kód analízis

Felmerült még a gondolat, hogy beintegráljunk a CI-be egy kódanalízis eszközt. Ennek segítségével standard módon lehetne mérni a kódminőséget, fejlesztőtől és fejlesztői környezettől függetlenül.
Ennek ellenére most ennek alkalmazása ellen döntöttünk, hiszen a tagok a fejlesztőkörnyezetükbe telepítettek sonarlintet, így az információ redundáns lett volna.

### Delivery támogatás

Ezen projekt szempontjából a legkézenfekvőbb a Nuget csomagok létrehozása és terjesztése, melyet a nuget.yml workflow végez.
A munka ezen része vitte el számomra a ráfordított idő oroszlánrészét.
Eredetileg a Github package repository-ját kívántam használni, ám nem jártam sikerrel: valószínűleg nem rendelkeztem megfelelő jogosultsággal ennek megvalósításához.
Így a MS Azure cloudban hoztam létre egy artifact repository-t. Ennál a megoldásnál is az automatikus authentikáció okozta a problémát.
Alapesetben a MS interaktív beléptetést javasol, ám ez értelemszerűen nem jöhet szóba CI alkalmazásakor.
Nehézséget jelentett továbbá, hogy a repoba pusholáshoz három különböző eszköz áll rendelkezésre, három különböző interfésszel és nem feltétlenül bőbeszédű dokumentációval.
Kézenfekvő volt, hogy létre kell hoznom egy Personal Access Tokent a repohoz, a kérdés csak az volt, hogy ezt hogy adom át a dotnet eszköznek.
Hosszas keresés után a megoldás a következőképpen néz ki:
- A VSS_NUGET_EXTERNAL_FEED_ENDPOINTS környezeti változó tárolja  a kapcsolódási adatokat Json formátumban
- fel kellett telepíteni egy Credential providert a CI környezetbe, mert ez tudja használni a fenti változóban levő információt.
- A dotnet nuget push parancsba pedig egy tetszőleges stringet lehetett írni a z api-key helyére
  
Ezen felül megoldottam továbbá, hogy a különböző buildekhez buildszám rendelődjön, így mindegyik változatot feltölthetjük a repóba, nem kell feltétlenül külön a verziózásra figyelni. Ha valami miatt ugyanolyan csomagnév készül, mint ami egyszer már fel lett töltve, nem dob hibát a job és sikeresen lezárul.

Mivel a github repónkban egyszerűsített Github Flow-t alkalmaztunk, és a master ágba stabil kód kell, hogy kerüljön, úgy döntöttem, hogy a deployment minden masterre pusholásnál fusson le, miután persze a tesztek lefutottak.
