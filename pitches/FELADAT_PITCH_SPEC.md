# Szoftver specializáció – Projektmunka
## 1. Feladat: 3 Pitch és Specifikáció Elkészítése

**Projektmanager:** @szczukabendeguz  
**Határidő / Prezentáció hossza:** 5 perc / pitch (Összesen 15 perc prezentáció)  
**Leadandó formátum:** **Álló formátumú PDF** dokumentum (könnyen görgethető prezentáláshoz)  
**Tervező eszközök:** Figma vagy PowerPoint

---

## 🤝 Csapatmunka és Alapelvek

Ez az első közös feladatunk, és szeretném, ha ebben a félévben mindenki egyenlően kivenné a részét a munkából. 
Ha elakadsz vagy nem tudsz haladni, kérlek azonnal jelezd! Segítsünk egymásnak, merjetek segítséget kérni a többi csapattagtól is, nincs rossz kérdés. A lényeg, hogy együtt, csapatként haladjunk a félév során.

---

## 📅 Ütemezés és Határidők

*   **Ma / Holnap:** Mindenképpen vegyétek fel a kapcsolatot a saját (2 fős) csapatotokkal! Beszéljetek meg egymás között legalább egy idősávot a hétvégére/hétfőre, amiben együtt tudtok dolgozni ezen, hogy egyik csapat se fusson ki az időből.
*   **Február 2. (Hétfő) 18:00 - Online Meeting:** Közös státuszegyeztetés. Ide már **prezentálható tartalmat** hozzatok, amit be tudtok mutatni, és egymás között tudunk véleményezni, finomítani rajta.
*   **Szerda 10:45 - VÉGSŐ HATÁRIDŐ ÉS BEMUTATÓ:** Ekkor prezentáljuk a 3 pitchet az órán. Addigra a kész PDF-eknek a repóban kell lenniük.

---

## 👥 Csapatbeosztás

A 6 fős csapatunk három, egyenként 2 fős mikrocsapatra oszlik:

1.  **Jegyfoglaló és Rendezvényszervező Webalkalmazás** -> [@gergo-battyanyi](https://github.com/gergo-battyanyi), [@NagyBendeguz](https://github.com/NagyBendeguz)
2.  **Szabadság Jóváhagyó és Kezelő Rendszer** -> [@batoriandras](https://github.com/batoriandras), [@fbarnabas55](https://github.com/fbarnabas55)
3.  **Mountain Bike Felfüggesztés Beállítás-támogató Alkalmazás** -> [@szczukabendeguz](https://github.com/szczukabendeguz), [@misih26](https://github.com/misih26)

---

## 📝 A Témák Részletes Specifikációja

*FONTOS: Az alábbi leírásoktól természetesen eltérhettek a tervezés során, ezek csak az első meetingünkön felmerült alapötletek és irányvonalak!*

### 1. Jegyfoglaló és Rendezvényszervező Webalkalmazás
*   **Cél:** Egy olyan hiánypótló jegyértékesítő platform létrehozása, amely rugalmasabb és professzionálisabb egy Google Formsnál, költséghatékonyabb a nagy jegyirodáknál (pl. jegy.hu), és testreszabhatóságban felveszi a versenyt az egyedi fejlesztésű oldalakkal.
*   **Technológiai Stack:**
    *   **Frontend:** Angular (testreszabható UI komponensek)
    *   **Backend:** ASP.NET Core REST API
    *   **Adatbázis:** SQL Server (Entity Framework Core)
*   **Főbb funkciók:**
    *   **Szervezői regisztráció és profilkezelés:** A rendezvényszervezők saját profilt hozhatnak létre, ahová feltölthetik logójukat és beállíthatják a saját arculati színeiket (Custom/Customer look).
    *   **Vizuális nézőtér tervező:** Interaktív felület, ahol a szervező megrajzolhatja a nézőteret, elhelyezheti és számozhatja a székeket.
    *   **Eseménykezelés:** Rendezvények létrehozása, időpontok, leírások és jegyárak (pay rates) megadása.
    *   **Fizetési integráció:** Beépített, biztonságos 3rd party fizetési átjáró (pl. Stripe, Barion, SimplePay) a jegyvásárlások kezelésére.
    *   **Automatikus e-mail kommunikáció:** Visszaigazoló e-mailek küldése a sikeres vásárlásról (QR kóddal/elektronikus jeggyel), valamint automatizált lemondási és visszatérítési folyamat kezelése.
    *   **Többszintű árazási modell (SaaS):** Költségszintek (tearek) kialakítása a szervezőknek, például: havonta 1 ingyenes előadás alapfunkciókkal, afölött előfizetéses vagy jutalékos rendszer.

### 2. Szabadság Jóváhagyó és Kezelő Rendszer
*   **Cél:** Egy átlátható és szabályozható vállalati alkalmazás a munkavállalói távollétek igénylésére, engedélyezésére és nyomon követésére.
*   **Technológiai Stack:**
    *   **Frontend:** Angular (naptár nézetek, dinamikus jogosultságkezelés)
    *   **Backend:** ASP.NET Core REST API
    *   **Adatbázis:** SQL Server (Entity Framework Core)
*   **Főbb funkciók:**
    *   **Szerepkör-alapú hozzáférés (RBAC):** Három fő jogosultsági szint:
        *   *Dolgozó:* Szabadság igénylése, saját egyenleg és státusz megtekintése.
        *   *Manager:* Beosztottak igényeinek elbírálása (jóváhagyás/elutasítás), részleg naptárának áttekintése.
        *   *Admin:* Rendszerbeállítások, felhasználók és kvóták kezelése, teljes cég áttekintése.
    *   **Részleg- és kvótakezelés:** Beállítható, hogy egy adott napon vagy időszakban a cégnél, illetve az egyes részlegeknél (csoportoknál) maximum hány fő, vagy a létszám hány százaléka lehet egyidejűleg szabadságon.
    *   **Dinamikus naptár nézetek:** Dolgozói, vezetői és adminisztrátori naptárak, testreszabható láthatósággal (pl. beállítható, hogy a dolgozók lássák-e a többiek nevét, vagy csak a foglalt státuszt).
    *   **Naptár szinkronizáció:** Integráció külső naptáralkalmazásokkal (Google Calendar, Outlook/Microsoft 365 Calendar) iCal vagy API segítségével.
    *   **Single Sign-On (SSO):** Opcionális bejelentkezés Microsoft Entra ID (korábban Azure AD) vagy Google Workspace fiókkal a vállalati integrációhoz.

### 3. Mountain Bike Felfüggesztés Beállítás-támogató Alkalmazás
*   **Cél:** Egy intelligens rendszer, amely segít a kerékpárosoknak a bonyolult felfüggesztési paraméterek (levegőnyomás, csillapítás stb.) optimális beállításában, hogy növeljék a komfortot, teljesítményt és biztonságot.
*   **Technológiai Stack:**
    *   **Frontend:** Angular (reszponzív, terepen is könnyen használható mobilbarát felület)
    *   **Backend:** ASP.NET Core REST API
    *   **Adatbázis:** SQL Server (Entity Framework Core)
*   **Főbb funkciók:**
    *   **Alkatrész adatbázis:** A felhasználó kiválaszthatja a kerékpárjában lévő teleszkópot/rugóstagot egy előre feltöltött adatbázisból, a gyári alapbeállításokkal együtt.
    *   **Konfigurációk menedzsmentje:** Különböző beállítási profilok (pl. "köves lejtő", "ugratós pálya") mentése, nyomon követése és gyors összehasonlítása.
    *   **Visszajelzés-alapú javaslatok:** Menet utáni hétköznapi visszajelzések (pl. "túl rázós", "felütött az ugratónál") rögzítése, amiből a rendszer egy beépített szabályrendszer (később akár LLM/Gemini API) alapján konkrét fizikai állítási javaslatokat (pl. "-5 PSI", "lassabb rebound") generál.
    *   **Oktató modul:** Lépésről-lépésre útmutató, amely elmagyarázza a felhasználónak az egyes paraméterek (kompresszió, visszaút) fizikai hatásait a kerékpár viselkedésére.

---

## 📑 PITCH PDF KÖVETELMÉNYEK ÉS SABLON

A leadandó anyagnak egységesen **Álló formátumú PDF**-nek kell lennie, amelyet görgetve fogunk bemutatni. Témánként pontosan **8–10 darab mock** képernyőképnek kell benne lennie.

### A PDF Kötelező Felépítése (kb. 12-15 oldal összesen)

**Címlap** *(~1 oldal)*
*   Az alkalmazás fantázianeve, a téma címe, és a készítők nevei/GitHub azonosítói.

**Tartalomjegyzék** *(~1 oldal)*
*(Figyelem: Ez nem számozott fejezet, csak egy áttekintő lista a dokumentum felépítéséről oldal-hivatkozásokkal.)*


**1. Az alkalmazás célja** *(~1 oldal)*
*   Röviden, vázlatpontosan fejtsétek ki a következőket:
    *   **Mi az alkalmazás célja?** (A fő probléma, amit megold)
    *   **Miért jó?** (Kinek és miért jelent könnyebbséget)
    *   **Mire használható?** (Gyakorlati use-case)
    *   **Mi az értelme?** (A hozzáadott üzleti/felhasználói érték)

**2. Rendszer Flow** *(~1-2 oldal)*
*   Egy profi, jól átlátható vizuális folyamatábra (User Flow Diagram) arról, hogyan jut el a felhasználó a belépési ponttól a fő célja (funkció) végrehajtásáig.
*   **Kötelező elem:** A folyamatábrán a specifikációban szereplő képernyőtervek (mockok) neveinek egyértelműen meg kell jelenniük mint a folyamat állomásai (pl. *Regisztráció képernyő* -> *Dashboard* -> *Esemény létrehozása képernyő*). Ezzel kötjük össze a vizuális terveket a logikával.
*   **Eszköz javaslatok (Törekedjünk a minőségre, kerüljük a Paintes nyilazgatást!):**
    *   **Draw.io (diagrams.net):** Ingyenes, vizuális drag-and-drop szerkesztő. Nagyon szép, szabványos folyamatábrákat lehet vele készíteni.
    *   **Mermaid.js:** Ha inkább kódból generálnátok ábrát. Nagyon menő, Markdown-szerű szintaxissal írhattok folyamatokat, amit a GitHub vagy a Mermaid Live Editor azonnal profi ábrává generál.
    *   **Figma (FigJam):** Ha a UI terveket úgyis Figmában készítitek, a FigJam tökéletes és gyors megoldás a flow összekötésére, beépített folyamatábra komponensekkel.
*   **Keresési tippek inspirációhoz:** Mielőtt nekiálltok, érdemes rákeresni a Google-ben vagy a Pinteresten néhány iparági sztenderd példára. Keressetek ezekre a kifejezésekre:
    *   `UI UX user flow diagram example`
    *   `SaaS app user flow map`
    *   `System flow architecture diagram`
    *   `Mermaid.js flowchart examples`

**3. Képernyőtervek és Specifikáció** *(~8-10 oldal)*
*   *(Ez a dokumentum gerince. Minden képernyőkép KÜLÖN oldalra kerüljön, és mindegyik alá kötelezően be kell írni az alábbiakat:)*
*   **Képernyő [Sorszám]: [Képernyő neve, pl. "Szervezői Dashboard"]**
*   *[KÉPERNYŐKÉP BEILLESZTÉSE IDE]*
*   **Mire kattint a felhasználó?** (Fő gombok, input mezők felsorolása)
*   **Milyen lehetőségei vannak?** (Funkciók, pl. szűrés, adatbevitel, törlés)
*   **Mi történik utána?** (Hova vezet a gombnyomás? Melyik képernyő jön ezután, milyen háttérfolyamat indul el?)

**4. Technológiai Követelmények és Zárás** *(~1 oldal)*
*   Rövid összefoglaló a használt stackről (Angular, .NET, SQL Server) és a főbb külső integrációkról (pl. Stripe, Google Calendar API). Egy záró gondolat a pitch végére.

---

## 📂 Git Repository Struktúra és Feltöltés

A GitHub repóban létrehoztam/létrehozok 3 mappát. Kérlek, a kész anyagokat (a Figma forráslinkeket, a nyers képeket és a **KÉSZ PDF-et**) a saját mappátokba töltsétek fel a szerdai prezentáció előtt!

*   `/pitch_jegyfoglalo/`
*   `/pitch_szabadsag_kezelo/`
*   `/pitch_mtb_felfuggesztes/`
