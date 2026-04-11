# Pitch review – kiegészítések és megjegyzések

Sziasztok!  
Köszi mindenkinek a rengeteg munkát és a sok belefektetett időt – mind a felkészülésbe, mind a megbeszélésbe. 
Az alábbi jegyzet a megbeszélés során elhangzott kiegészítések/javaslatok vázlatos összeszedése

## Pitch 1 – Jegykezelő / jegyértékesítés

### Kiegészítések / megjegyzések
- A doksiban jelenjen meg, hogy a beágyazott/foglaló felület **személyre szabható** (pl. háttér/színek), mert ez értékajánlat és eladási érv.
- Terminológia: „beágyazott oldal” helyett inkább „foglaló oldal”; az integrációt érdemes többféleképp keretezni (iframe + alternatívák említése, pl. JS csomag/plug-in jelleg).
- Opcionális (említés szinten) extra feature: dinamikus árazás eseményenként (idő/darabszám/kereslet alapján).
- Opcionális (említés szinten) extra: eseményszervezői API-k (pl. eseménylista, telítettség/„mennyi jegy van még”), amit a saját oldalukon fel tudnak használni.
- Prezentálhatóság: legyen legalább 1 rendesen kinéző landing/bemutató oldal (akár generált index.html), és normális screenshotok rövid leírással.
- Árazási logika tisztázása: ár helyszínhez kötött vs. eseményhez kötött (vagy mindkettő), illetve bevételi modell egyszerűsítése/egyértelműsítése (pl. jutalék a jegyárból).

## Pitch 2 – Home office / szabadság app

### Kiegészítések / megjegyzések
- Scope: a helyfoglalás/asztalfoglalás részt vegyétek ki (túl nagy falat, könnyen elviszi a projektet).
- Fókusz: a core (szabadság + home office) részt érdemes „kimaxolni” admin/policy irányból.
- Admin/policy kiegészítések: csoportkezelés, jogosultságok, kivételkezelés, beállítások kezelhetősége (ne „mindenkire ugyanaz globálisan” logika).
- Konfigurálható szabályok: max HO nap/hét, auto-elfogadás opció, emlékeztető/értesítés X nap után (jóváhagyás csúszására).
- Döntések kiszervezése a felhasználó cégnek: pl. foglalható időhorizont (2 hét vs több) legyen policy/admin beállítás, ne fejlesztői fix.
- Onboarding/beléptetés: legyen leírva opcióként (meghívós/linkes beléptetés vagy céges admin általi felvitel) – elég említés szinten, de legyen benne.
- Szabadság modul bővíthetősége (admin oldal): szabály alapú keretszámítás (életkor/belépés óta eltelt idő), áthozott napok kezelése, határidők/korlátok jelzése.
- Naptár „alapelvárások”: munkaszüneti napok / eltérő munkarend jelzése (hogy „normál naptár” szintet hozzon).
- Üzleti csomagolás: szolgáltatásként érdemes keretezni (nem „odaadjuk a programot a saját szerverre” fókusz).

## Pitch 3 – Kerékpár felfüggesztés asszisztens

### Kiegészítések / megjegyzések
- Az „intelligens hibakeresés/javaslatmotor” részt érdemes opcionális bővítésként kezelni vagy kivenni, mert könnyen túl nagy komplexitást és elvárást sugall.
- Maradjon tiszta core flow: garázs központ, bringa hozzáadás, gyári alapbeállítások (pl. testsúly alapján), pályaprofilok mentése/betöltése, manuális állítás.
- Prezentációs taktika: ne mutassatok túl sok extra funkciót/képernyőt, mert az „automatikus elvárássá” válik; inkább 1–2 erős use-case.
- Adatforrás keretezés: gyártói/publikus adatokból feltételezett adatbázis OK, de ne vállaljatok túl mély algoritmikus részleteket.
- Kockázat/korlát kimondása: a pontos algoritmikus átszámítások szakértői inputot igényelhetnek; ezt lehet „későbbi finomításként” kezelni.

## Összefoglaló
Mégegyszer köszi mindenkinek a beleölt időt és energiát.
Szerdán találkozunk, addig a fenti teendőket csináljuk meg pitchenként, hogy a scope tiszta legyen, az anyagok egységesek legyenek, és a prezentációk jól „eladhatóak” legyenek.
Ha valami blokkol (pl. mi kerüljön ki/be, mi legyen opcionális), írjátok meg előre, és szerdáig gyorsan lezárjuk.
