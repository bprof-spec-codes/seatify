## Mountain bike felfüggesztés beállításokat támogató webalkalmazás

**Testreszabási javaslatokat nyújtó intelligens rendszer**

A modern mountain bike-ok felfüggesztése rendkívül sok paraméterrel rendelkezik, amelyek precíz hangolásával nagymértékben javítható a komfort, a stabilitás és a teljesítmény. A felhasználók többsége azonban nem tudja kihasználni ezeket a lehetőségeket, mert hiányzik a szakmai tudás a beállítások hatásairól.

**Technológiák:** C#, Angular, ASP.NET Core, Entity Framework, SQL Server

### A probléma

A modern mountain bike-ok felfüggesztése rendkívül sok paraméterrel rendelkezik (pl. levegőnyomás, visszaút- és kompressziócsillapítás), amelyek precíz hangolásával nagymértékben javítható a komfort, a stabilitás és a teljesítmény. A felhasználók többsége azonban nem tudja kihasználni ezeket a lehetőségeket, mert:

- hiányzik a szakmai tudás a beállítások hatásairól,
- bizonytalanok abban, hogy a módosítás javít vagy ront a helyzeten,
- nem jegyzik fel és nem követik nyomon korábbi beállításaikat,
- sokszor egy „fix" alapkonfigurációval használják a kerékpárt minden terepen.

Ennek eredményeképp a kerékpár képességeit nem használják ki teljes mértékben, az élmény csökken, sőt helytelen beállítás esetén a biztonság is veszélybe kerülhet.

### Fontosság

A felfüggesztés helyes beállítása alapvetően meghatározza a mountain bike élményt és teljesítményt. Egy rosszul hangolt rendszer:

- kényelmetlen és rázós menetet eredményez,
- technikás terepen instabilitást és sérülésveszélyt is hordoz,
- a drágább, prémium kerékpárok esetében különösen kihasználatlan potenciált jelent.

Egy felhasználóbarát, adatbázis-alapú és visszajelzésekből tanuló alkalmazás lehetőséget adna a bringásoknak arra, hogy könnyebben megértsék a beállításokat, és tudatosan optimalizálják kerékpárjukat különböző terepviszonyokhoz.

### Megoldás

A tervezett webalkalmazás célja, hogy egyszerűen használható felületet biztosítson a felfüggesztés konfigurálásához és nyomon követéséhez. Fő funkciói:

- **Adatbázis alapú kiválasztás:** a felhasználó kiválaszthatja a kerékpárjához tartozó felfüggesztést a rendszer adatbázisából, az alapbeállításokkal együtt.
- **Beállítási útmutató:** lépésről-lépésre magyarázatot ad arról, hogy az egyes paraméterek milyen hatással vannak a kerékpár viselkedésére.
- **Konfigurációk mentése és összehasonlítása:** a beállítások elmenthetők, így a felhasználó több különböző konfigurációt tud tárolni és gyorsan visszaállítani.
- **Felhasználói visszajelzés → javaslat:**
    - Menet után a felhasználó egyszerű, hétköznapi nyelven ad visszajelzést (pl. „túl rázós volt a köves lejtőn", „nagyon mélyen ült a bringa az ugrásoknál").
    - A rendszer egy szabályrendszer alapján alakítja át ezt konkrét paramétermódosításokra (pl. levegőnyomás 5 PSI csökkentése, kompresszió puhítása, rebound gyorsítása).

**Fejlesztési potenciál:** későbbi bővítésként a rendszer képes lehet nyelvi modellek (pl. Gemini API) bevonására, amelyek a beszélt nyelven megadott visszajelzést automatikusan fordítanák konkrét beállítási paraméterekké, így még intuitívabbá téve a használatot.

### Fejlesztési eszközök

**Programozási nyelvek:**
- C# (.NET backend)
- TypeScript (Angular frontend)

**Könyvtárak és keretrendszerek:**
- Angular (frontend egyoldalas alkalmazás)
- ASP.NET Core (REST API backend)
- Entity Framework Core (adatbáziskezelés és ORM)

**Adatbázis:**
- Microsoft SQL Server

**Fejlesztői környezet:**
- Visual Studio és Visual Studio Code
- Git verziókezelés
- Docker (opcionális konténerizáció)

### Összegzés

A projekt egy olyan webalkalmazás fejlesztését célozza, amely egyszerűsíti a mountain bike felfüggesztések beállítását, és a felhasználói visszajelzések alapján konkrét javaslatokat ad paraméterek módosítására. A megoldás hozzájárul a komfort, a teljesítmény és a biztonság növeléséhez, valamint lehetőséget ad az innovatív nyelvi modellek későbbi integrációjára is.
