## Észrevételek (a doksihoz)
- Landing page külön nem jelenik meg képernyőként/oldalként; a leírt képernyők a belépés/regisztráció környékéről indulnak.
- Az esemény dashboardnál szerepel, hogy “a jelenlegi foglalások egy mátrixos nézetben” láthatók; ez admin oldalon túl részletes lehet, ha cél csak az áttekintés.
- Van “Beágyazott oldal” koncepció, és kifejezetten IFrame-es integráció is említve van (mint vásárlói frontend), ami technikailag gyors, de valóban korlátozott kontrollt ad a megjelenés felett a host oldalon.
- A “Beágyazott oldal” leírásban az is szerepel, hogy a szabad helyek és azok árai megjelennek, illetve az árkategória kiválasztást színezés segíti; ebből következik, hogy az ár/árkategória és a helyek kapcsolata kritikus modellkérdés.
- A doksi jelen formájában nem tér ki külön arra, hogyan testreszabható a vásárlói felület kinézete (logó, színek, betűtípus, domain/white-label), pedig a pozicionálásban hangsúlyos a “személyre szabhatóság”.

## Kérdések (tisztázandó döntések)
- Flow-leírás: mi legyen a hivatalos end-to-end folyamat (seat hold, időzár, fizetési visszatérés, sikertelen fizetés utáni felszabadítás), és ezt hol dokumentáljuk röviden a képek mellé?
- Landing page: lesz-e nyilvános “eseménylista / esemény landing” (vagy szervezői marketing landing), és ha igen, mi az elsődleges CTA (esemény választás vs. belépés)?
- Foglalás dashboard (admin): tényleg kell-e szék-szintű foglaltság mátrix, vagy elég aggregált nézet (szektor/árkategória eladott vs. szabad; bevétel; időbeli trend)?
- Árazás adatmodell: a szék ára hol legyen “igazság forrása” (helyszín/nz̋tér szinten, vagy esemény szinten), és legyen-e opció mindkettőre (pl. “alapár” helyszínen, “felülírás” eseménynél)?
- Beágyazás vs. link: az elsődleges ajánlás legyen-e linkelt foglalási oldal (stabil UI kontroll), és csak opcionálisan IFrame?
- API igény: legyen-e publikus/partner API végpont (auth-al), amivel a szervező saját oldalon tud eseménylistát, időpontokat, elérhetőséget/foglaltságot lekérni, miközben a vásárlás továbbra is a ti felületeteken történik?

## Javasolt kiegészítő kérdések (hogy ne maradjon lyuk)
- Brand/white-label: kell-e egy “megjelenés beállítások” rész (logó, primary color, email sablonok, saját domain), és ez szervezőnként vagy eseményenként állítható?
- Foglalási biztonság: mennyi ideig tart egy “soft hold” (pl. 10 perc), mi történik lap bezárásakor, és kell-e rate limit / bot védelem?
- Fizetés és számlázás: milyen fizetési szolgáltató, milyen visszatérítés/lemondás folyamat, és ki állítja ki a számlát (platform vs. szervező)?
- Beléptetés: lesz-e jegyellenőrző felület/app (QR kód), offline mód, és hogyan kezelitek a duplikált/érvénytelen jegyeket?
- Jogosultságok: kell-e szerepkör (tulajdonos/admin/operátor), audit log (ki mit módosított), illetve több szervező egy eseményen?
- GDPR/adatkezelés: adatmegőrzés, törlés, export, és milyen személyes adat kell valóban a vásárláshoz?
