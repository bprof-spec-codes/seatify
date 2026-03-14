# TrailTune - UI / UX Képernyő Specifikáció

Ez a dokumentum a TrailTune prototípus aktuálisan lefejlesztett, böngészőben tesztelhető képernyőit, azok funkcióit és a közöttük lévő navigációs logikát (flow) foglalja össze. Nincs szükség backend szerverre az interakciók bemutatásához, a prototípus tisztán HTML/CSS (és minimális JS) alapokon működik.

---

### Képernyő 0: Landing Page (Érkezési oldal új látogatóknak)

**Leírás (Cél és Megjelenés):**
Célja az új érdeklődők figyelmének felkeltése és az alkalmazás bemutatása. Megjelenését tekintve egy modern, hosszan görgethető, marketing fókuszú weboldal sötét témával.

* **Header (Fejléc):** Bal oldalon az alkalmazás logója, jobb oldalon egy diszkrét, áttetsző „Belépés” gomb.
* **Hero Section (Fő banner):** A képernyő felső részét egy dinamikus, mountain bike-os háttérkép uralja. Rajta egy nagy, figyelemfelkeltő címsor, alatta egy rövid értékajánlat, és egy hangsúlyos zöld „Kezdd el ingyen” gomb.
* **Features (Funkciók) szekció:** Lejjebb görgetve 3 minimalista kártya mutatja be a fő funkciókat ikonokkal.

**Interakciók és Flow:**

* **Fejléc „Belépés” gomb:** A már regisztrált felhasználóknak szól. Átirányít a **Képernyő 1-re**, méghozzá úgy, hogy a „Bejelentkezés” fül legyen aktív.
* **„Kezdd el ingyen” (Hero) / „Próbáld ki most” (Bottom) gombok:** Az új érdeklődőket célozza. Átirányít a **Képernyő 1-re**, aktiválva a „Regisztráció” fület.

---

### Képernyő 1: Kezdőlap és Autentikáció

**Leírás (Cél és Megjelenés):**
Célja a felhasználók fiókba való bejelentkeztetése vagy új fiók létrehozása. Megjelenít egy egyszerű, kártya alapú nézetet középre igazítva. Felül az alkalmazás logója, alatta két nagyméretű, CSS-alapú fül (Tab): „Bejelentkezés” és „Regisztráció”.

**Interakciók és Flow:**

* **Alkalmazás Logója (Felül):** Rákattintva visszairányít a **Képernyő 0-ra (Landing Page)**.
* **Bejelentkezés / Regisztráció fülek:** Kattintásra a képernyőn belül cseréli az űrlapot (nincs oldalváltás).
* **„Elfelejtett jelszó” link:** Egy sötét, CSS-only felugró (Modal) ablakot nyit meg a jelszó-visszaállításhoz (Elmosott, blur hátterű overlay).
* **„Fiók létrehozása” gomb (Regisztráció végén):** A rendszer feltételezi, hogy nincs még megadott adat, ezért a **Képernyő 3-ra (Új Kerékpár Hozzáadása)** irányít.
* **„Belépés” gomb (Bejelentkezés végén):** A rendszer a **Képernyő 2-re (Dashboard)** irányít.

---

### Képernyő 2: Főoldal (Dashboard) és Garázs

**Leírás (Cél és Megjelenés):**
Célja, hogy áttekintést nyújtson a felhasználó aktív kerékpárjáról és a mentett beállításairól; ez a felhasználó fő bázisa. Megjelenésében felül a jelenleg aktív kerékpár nagy képes kártyája látható. Alatta egy görgethető rácsos lista a mentett pályák (beállítások) kártyáival. A képernyő jobb alsó sarkában egy lebegő akciógomb (FAB), alul pedig egy fix navigációs sáv (Navbar) található.

**Interakciók és Flow:**

* **„Új bringa hozzáadása” ikon (Fejléc):** Átirányít a **Képernyő 3-ra**.
* **„Szerkesztés / Finomhangolás” (Ceruza ikon az aktív kerékpáron):** Átirányít a **Képernyő 8-ra (Manuális szerkesztés)**.
* **„Mentett Pályáim” kártyák:** Bármelyik beállítás kártyájára bökve a rendszer a **Képernyő 7-re (Pályára érkezés)** ugrik.
* **Lebegő Akciógomb (FAB):** Zöld, villáskulcs ikonnal ellátott gomb. Elindítja a hibakeresést, átirányít a **Képernyő 9-re (Intelligens visszajelzés)**.
* **Alsó Navbar „Közösség” gomb:** Átirányít a **Képernyő 5-re (Közösségi Felfedező)**.
* **Alsó Navbar „Profil” gomb:** Megnyit egy lebegő felugró menüt (Modal), ahonnan a **Képernyő 11-re (Profil)** vagy a Képernyő 1-re (kijelentkezés) juthatunk.

---

### Képernyő 3: Új Kerékpár és Felfüggesztés Hozzáadása

**Leírás (Cél és Megjelenés):**
Célja az új kerékpárhoz tartozó alapadatok megadása a rendszer számára. Egy letisztult űrlap jelenik meg, amely bekéri a hardveres adatokat.

* **Kerékpár fotó:** Egy stílusozott gomb, ami megnyitja a készülék képfeltöltőjét.
* **Legördülő menük (Select):** Kerékpár márka, Villa, Rugóstag.
* **Súly-mező:** Szám beviteli mező az összsúlyra (felszerelésben).
* **Gumi paraméterek:** Külön blokk (Gumiszélesség választó és egy Tubeless (Csobi) kapcsoló).

**Interakciók és Flow:**

* **„Vissza” gomb a bal felső sarokban:** Változtatások nélkül visszadob a **Képernyő 2-re**.
* **„Mentés és Alapbeállítás lekérése” (Nagy gomb lent):** Validálja az űrlapot. Megnyomásakor a rendszer továbbléptet a **Képernyő 4-re (Gyári Alapbeállítások)**.

---

### Képernyő 4: Gyári Alapbeállítások (Base Setup)

**Leírás (Cél és Megjelenés):**
Célja a megadott adatok alapján kalkulált gyári kiinduló értékek vizuális bemutatása a felfüggesztéshez és a gumikhoz. Megjelenésében nagy, jól olvasható számok paneljeiből áll. Három fő csoport van: Villa, Rugóstag és a Gumi paraméterek alapján számított Ajánlott Guminyomás panel. A paraméterek mellett kis (i) ikonok találhatók.

**Interakciók és Flow:**

* **Információs („i”) ikonok:** Rábökve/fölé víve az egeret egy Tooltip (kicsi felugró fekete buborék) magyarázza el röviden, mi az a PSI, BAR, LSC vagy a Rebound. Mindez JS mentesen történik.
* **„Adatok módosítása” másodlagos gomb:** Visszavisz a **Képernyő 3-ra**, ha a felhasználó mondjuk elírta a súlyát.
* **„Beállítottam, mehetünk!” (Zöld gomb):** Véglegesíti az első lépéseket. Átirányít a **Képernyő 2-re (Dashboard)**.

---

### Képernyő 5: Közösségi Felfedező (Social Setup Explorer)

**Leírás (Cél és Megjelenés):**
Célja, hogy lehetőséget adjon más riderek beállításainak böngészésére, keresésére és letöltésére. Kinézetre legfelül egy keresősáv, alatta vízszintesen görgethető (rejtett görgetősávval rendelkező) szűrő-címkék (Chips). Alatta az Accordion típusú, profilképpel ellátott listakártyák.

**Interakciók és Flow:**

* **Szűrő-címkék (Toggle Chips):** CSS-alapú kapcsolók. Rányomva aktív (zöld)/inaktív állapotba váltanak valós időben.
* **Közösségi beállítás kártya (Accordion):** Rákattintva a kártya animálva lenyílik, és megmutatja a konkrét letöltendő teleszkóp értékeket.
* **„Beállítás Importálása” gomb (A lenyílt kártyán belül):** Ezzel indítható el a mentési folyamat. Átirányít a **Képernyő 6-ra (Mentés)**.
* **Alsó Navbar „Garázs” gomb:** Átirányít a **Képernyő 2-re**.
* **Alsó Navbar „Profil” gomb:** Átirányít a **Képernyő 11-re (Profil Szerkesztés)**.

---

### Képernyő 6: Pályaspecifikus Beállítás Mentése

**Leírás (Cél és Megjelenés):**
Célja a letöltött vagy manuálisan megadott pálya beállításának mentése és publikálási opcióinak kezelése. Megjelenése egy mentési összesítő űrlap. Felül szövegmezők (pálya neve és körülmények). Ez alatt egy zöld hátterű információs sáv jelzi („Rád szabva!”), hogy a szoftver adaptálta az importált értékeket a felhasználó adataira. Alatta egy blokkosított "nyugta" szerű lista összegzi az értékeket, majd jönnek a mentési opciók és gombok.

**Interakciók és Flow:**

* **Láthatóság kapcsoló (Toggle/Switch):** Átkapcsolható telefonos stílusú gomb a „Privát” és „Publikus megosztás” között.
* **„Mégsem” gomb (Felül és Alul is):** Megszakítja a folyamatot, visszavisz a **Képernyő 5-re**.
* **„Pálya Profil Mentése” (Zöld gomb):** Végrehajtja a mentést és visszatér a **Képernyő 2-re**.
* **„Mentés és beállítás (Irány a pálya)” (Sötét gomb):** Végrehajtja a mentést, majd azonnal átlép a valós beállítást segítő **Képernyő 7-re**.

---

### Képernyő 7: Pályára Érkezés és Alkalmazás (Delta / Abszolút Nézet)

**Leírás (Cél és Megjelenés):**
Célja az adott pályához tartozó szükséges klikkek/módosítások bemutatása a kerékpáron a jelenlegi állapothoz viszonyítva. Kialakítása egy célirányos szerelési nézet. Felül a kiválasztott pálya neve és viszonyítási alapként a jelenlegi beállítás. Alatta egy Sár/Csúszós terep panel. Középen zölddel kiemelt fülekkel lehet váltani a nézetek között, amik a konkrét listaszerű szerelési instrukciókat (zöld/piros) tartalamazzák.

**Interakciók és Flow:**

* **„Sár / Csúszós terep” Toggle:** Barna/Narancssárgás vizuális gomb, amely aktiválásával elméletben átszámolja az értékeket.
* **Nézetváltó Fülek (Különbség / Abszolút):** Zöld hátterű fülrendszer. A *„Mit tekerjek?”* fül zölddel (pl. +2 Katt) és pirossal (pl. -4 PSI) mutatja az eltéréseket az eredetihez képest. Az *„Abszolút”* fülön egyszerű fehér színnel jelenik meg, hogy mik a konkrét értékek az alap (teljesen zárt / üres) állapottól számolva.
* **„Kész, beállítottam a bringán!” (Jóváhagyó gomb):** Nyugtázza a fizikai módosítást a rendszerben, majd átirányít a **Képernyő 2-re**.
* **„Vissza” (Fejléc):** Visszavisz a **Képernyő 2-re** az adatok módosítása/alkalmazása nélkül.

---

### Képernyő 8: Paraméterek Manuális Szerkesztése

**Leírás (Cél és Megjelenés):**
Célja lehetővé tenni a tapasztaltabb felhasználóknak a meglévő beállítások gombokkal (stepper) történő egyedi és gyors testreszabását. Megjelenésében ez egy haladó, részletes nézet. Listák találhatók minden paraméterről (Villa, Rugóstag, és a Guminyomás panelje is helyet kapott). Az értékek mellett interaktív gombok találhatók.

**Interakciók és Flow:**

* **Plusz (+) és Mínusz (-) gombok (Stepper):** Minden sornál megtalálható apró, gomb alapú vezérlők. (A Tooltipek itt is üzemelnek az (i) betűknél).
* **Vissza (Fejléc):** Elveti a manuális próbálkozást és visszavisz a **Képernyő 2-re**.
* **„Mentés jelenlegi állapotként” gomb:** Rámenti az értékeket az aktuális profilra és visszavisz a **Képernyő 2-re**.
* **„Mentés új pályaként” másodlagos gomb (Outline):** Ha a kísérlet sikeres volt terepen és külön elmentené. Átirányít a **Képernyő 6-ra (Mentés)**, ahol nevet adhat neki.

---

### Képernyő 9: Intelligens Visszajelzés (Ride Feedback)

**Leírás (Cél és Megjelenés):**
Célja a bringán tapasztalt furcsa viselkedés (pl. felütés) strukturált rögzítése a mesterséges intelligencia elemzéséhez. Megjelenését tekintve ez egy dizájnos "panaszbejelentő" űrlap. Felül több, kategóriákba rendezett „chip” gomb, alatta pedig modern, csúszkás vezérlők, melyeket szubjektív érzetek skáláznak be (pl. Puha <-> Rázós).

**Interakciók és Flow:**

* **Gyorspanasz címkék (Multi-select Chips):** Egyszerre több is kiválasztható (pl. "Felüt az ugrásnál"). Rákattintva az alap szürke címke élénk zöldre vált.
* **Karakterisztika csúszkák (Range Sliders):** Fogd-és-vidd csúszkák a viselkedés megadására.
* **„Kihagy” (Fejléc):**  Visszavisz a **Képernyő 2-re**.
* **„Elemzés és Javaslat kérése” gomb:** Megnyomásakor egy homályosító töltőképernyő (Loading Overlay) jelentkezik egy forgó animációval és egy "Adatok elemzése és kalkuláció..." szöveggel.
* A töltőképernyő 2 másodperc múlva automatikusan eltűnik és a felhasználót a **Képernyő 10-re (Javaslat)** navigálja.

---

### Képernyő 10: Intelligens Javaslat és Véglegesítés

**Leírás (Cél és Megjelenés):**
Célja a visszajelzés alapján készített javaslat ismertetése és annak megindoklása, majd a korrekciók életbe léptetése. Külalakjában egy sikeres elemzést nyugtázó intelligens blokk fogad. Alatta egy „Előtte -> Utána” táblázat vizuális megjelenítése a változó paramétereknél. Legalul egy lenyitó fül (Accordion) magyarázza a műveletet, majd a gombok következnek.

**Interakciók és Flow:**

* **Összehasonlító kártyák:** A régi értékek áthúzott szürkével, az új - javított - értékek egy jobbra mutató nyíl után vastag zölddel jelennek meg.
* **„Miért javasoljuk ezt?” lenyíló fül (Accordion):** Rákattintva szövegesen, emberi nyelven elmagyarázza, miért módosította a program a légnyomást és a kompressziót a felütés ellen.
* **„Módosítások elvetése” másodlagos gomb:** Nincs változtatás, megszakítja a folyamatot és visszavisz a **Képernyő 2-re**.
* **„Javaslat alkalmazása a beállításra” elsődleges gomb:** Jóváhagyja az utasításokat, és beállítja aktuálisnak a profilján is, majd visszairányít a **Képernyő 2-re (Dashboard)**.

---

### Képernyő 11: Profil Szerkesztés és Fiókbeállítások

**Leírás (Cél és Megjelenés):**
Célja a személyes adatok, a profilkép és a jelszó kezelése, valamint lehetőséget adni a fiók törlésére is. Letisztult beállítások oldal, ahol a felhasználó a saját adatait menedzselheti.
Felül egy kör alakú profilkép található apró kamera ikonnal. Alatta a Jelszó megváltoztatása szekció három beviteli mezővel, legalul pedig egy vizuálisan elkülönülő (piros) veszélyzóna a fiók törléséhez. A mentés gomb alul lebegve kíséri végig.

**Interakciók és Flow:**

* **Profilkép feltöltés (Kamera ikon):** A gombra bökve megnyílik az eszköz natív fájlválasztója a kép cseréjéhez.
* **„Vissza” gomb a fejlécben:** Változtatás nélkül visszadob a **Képernyő 2-re**.
* **„Módosítások mentése” elsődleges gomb:** Jóváhagyja az űrlapot és visszanavigál a **Képernyő 2-re (Dashboard)**.
