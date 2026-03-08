# Seatify – jegyfoglaló és jegyértékesítő platform

Seatify egy jegyfoglaló/jegyértékesítő webalkalmazás, amely kis- és közepes szervezőknek ad olcsó, mégis testreszabható megoldást események és nézőterek kezelésére, a vevőknek pedig egyszerű, grafikus jegyfoglalási élményt.

## Célok (miért létezik)
- Egyszerűbb és professzionálisabb alternatíva „űrlapos” megoldásokhoz képest.
- Költséghatékony belépési küszöb szervezőknek.
- Rugalmas nézőtér / helyelosztás kezelés, gyors alkalmazkodás különböző helyszínekhez.

## Szerepkörök / felhasználók
- Szervező (B2B): helyszínek, nézőterek, események és foglalások adminisztrációja + brand/testreszabás.
- Végfelhasználó / vásárló (B2C): helyválasztás, foglalás véglegesítése.

## Csapat és felelősségek
- [@szczukabendeguz](https://github.com/szczukabendeguz) – Project Manager
- [@batoriandras](https://github.com/batoriandras) – Architect
- [@fbarnabas55](https://github.com/fbarnabas55) – Fullstack + Design lead
- [@gergo-battyanyi](https://github.com/gergo-battyanyi) – Fullstack
- [@NagyBendeguz](https://github.com/NagyBendeguz) – Fullstack
- [@misih26](https://github.com/misih26) – Fullstack

## Scope (MVP) – tervezett funkciók

> **Megjegyzés a monetizációhoz és a tranzakciókhoz (MVP korlát)**
> A szoftver egyetemi projekt (MVP) szintjén tisztán **Jegyfoglaló Platformként** működik. A tényleges fizetés és jegyátvétel a helyszínen, a szervező hatáskörében történik. Az online fizetési modul (Stripe Connect piactér) és az automatikus számlázás (Számlázz.hu API) a rendszerarchitektúrában "előkészített" jövőbeli funkciókként jelennek meg a felületen, de a mögöttes API-integrációjuk jelenleg out of scope.

### 1) Landing Page (Termékbemutató oldal)
- Értékajánlat kommunikálása potenciális szervezők felé (olcsó, testreszabható jegyértékesítés).
- Call-to-action (CTA) gombok és linkek a bejelentkezés / regisztráció felületekre.

### 2) Szervezői (admin) funkciók
- Regisztráció / bejelentkezés szervezőknek.
- Esemény dashboard:
  - Események listázása kártyás áttekintőben.
  - Új esemény létrehozása.
  - Esemény módosítása.
  - Esemény másolása.
  - Esemény törlése (megerősítéssel).
  - Foglalások áttekintése.
  - Foglaló oldal előnézet.
- Helyszín dashboard:
  - Helyszínek listázása és részletek megtekintése.
  - Új helyszín létrehozása / módosítása.
  - Helyszín törlése (megerősítéssel).
  - Nézőtér(ek) létrehozása / módosítása adott helyszínhez.
  - Nézőtér törlése (megerősítéssel).
- Nézőtér szerkesztés (seat map editor, MVP-szinten):
  - 1 vagy több „mátrix” létrehozása (sor/oszlop szám megadással).
  - Szektorok kezelése (külön mátrixok külön szektorokhoz).
  - Sor- és oszlop-azonosítók (egyszerű számozás; komplexebb szabály későbbi bővítés lehet).
  - Csoportos kijelölés alapú árazás beállítás.
- Esemény létrehozása/módosítása:
  - Név, leírás, helyszín, nézőtér, időpont.
  - Árazás (foglalási díjak):
    - Alap (nézőtér alapárai),
    - Egyedi árazás (ülőhely/szektor szerint),
    - Dinamikus árazás (idő/darabszám/kereslet elv).
  - Esemény kinézetének testreszabása (brand alapján, alapértelmezett fallback-kel).
- Profil + brand beállítás:
  - Profil adatok módosítása (pl. név, telefonszám, jelszó).
  - Brand/kinézet beállítások (alap megjelenés).
  - *Előkészített Monetizációs Beállítások (UI-only):* Stripe Connect fiók összekötése, Bankszámlaszám, Számlázz.hu API kulcs megadása.
  - **Beléptető modul (Check-in Scanner):**
  - Dedikált felület az adminban, amely a szervező (mobil) eszközének kameráját használja.
  - QR-kód szkennelése (Angular: `@zxing/ngx-scanner` vagy hasonló alapokon).
  - Helyszíni azonosítás, fizetendő összeg és jegyek listázása, majd státuszváltás "Beléptetve/Fizetve" állapotra.

### 3) Vásárlói funkciók (esemény link alapján)
- Foglaló oldal (event booking UI):
  - Grafikus, átlátható helyválasztás.
  - Ár vizuális jelölése (árkategória színezés).
  - Hover/infó a helyről és árról.
  - Kosár / kiválasztott jegyek összesítése.
  - „Tovább a foglaláshoz” funkció.
  - Link alapú megosztás a vevőknek (ezt hirdeti a szervező).
- Foglalás véglegesítése (checkout):
  - Esemény- és kosárinformációk ellenőrzése.
  - Foglaláshoz szükséges adatok bekérése (Név, E-mail, Telefonszám).
  - *Foglalás rögzítése:* A kiválasztott helyek státusza "Foglalt"-ra vált, a felhasználó átirányításra kerül egy sikeres képernyőre (fizetési kapu integráció helyett).
- E-mail visszajelzés a sikeres foglalás után:
  - Rendszerüzenet a foglalás adataival és egy **generált QR-kóddal**, ami a helyszíni fizetéshez és jegyátvételhez szükséges.
