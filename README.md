# SmartGym
SmartGym egy ASP.NET Core alapú edzőtermi menedzsment rendszer. Lehetővé teszi bérletek kezelését és vásárlását, szekrényfoglalások nyomon követését, valamint felhasználói értesítések megjelenítését. A projekt három komponensből áll: egy RESTful backend API-ból, egy Razor Pages alapú webes frontendből, és egy WPF-alapú adminisztrációs asztali alkalmazásból.

---

## Funkciók

**Felhasználók**
- Regisztráció és bejelentkezés (JWT alapú hitelesítés)
- Profil megtekintése és kezelése

**Bérletek**
- Bérlet vásárlása (heti, havi stb.)
- Stackelt bérletek támogatása — a bérletek egymás után indulnak
- Aktív, jövőbeli és lejárt bérletek áttekintése

**Szekrények**
- Aktív foglalás ellenőrzése
- Automatikus státuszkezelés (nyitott / zárt)
- Felhasználóhoz kötött foglalások

**Admin felület (WPF)**
- Asztali adminisztrációs alkalmazás Windows platformon

---

## Technológiák

| Réteg | Technológia |
|---|---|
| Backend | ASP.NET Core Web API |
| Frontend | Razor Pages, Bootstrap 5 |
| Admin felület | WPF (.NET) |
| ORM | Entity Framework Core |
| Adatbázis | MySQL |
| Hitelesítés | JWT (JSON Web Token) |

---

## Projekt struktúra

```
SmartGym/
├── SmartGym/                  # ASP.NET Core Web API (backend)
│   ├── Controllers/
│   ├── Models/
│   ├── Data/
│   └── Services/
├── GymFrontend/               # Razor Pages webes frontend
│   ├── Pages/
│   ├── Shared/
│   └── wwwroot/
├── SmartGymAdminWPF/          # WPF admin asztali alkalmazás
│   ├── Services/
│   ├── Views/
│   ├── App.xaml
│   └── MainWindow.xaml
├── SmartGym.slnx
└── README.md
```

---

## Bérlet logika

A rendszer stackelt bérleteket támogat: ha a felhasználónak már van aktív bérlete, az új bérlet nem azonnal, hanem az aktuális bérlet lejárta után kezdődik.

```
Havi bérlet:  2025.04.01 → 2025.04.30
Heti bérlet:  2025.04.30 → 2025.05.07   ← automatikusan indul
```

---

## Telepítés és futtatás

### 1. Adatbázis létrehozása

Hozz létre egy üres `smartgym` nevű adatbázist MySQL-ben, majd a Package Manager Console-ban futtasd:

```
Update-Database
```

Ez létrehozza a szükséges táblákat az Entity Framework migrációk alapján.

### 2. Connection string beállítása

Az `appsettings.json`-ban add meg a saját adatbázis-kapcsolatot:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;database=smartgym;user=root;password=JELSZO;"
}
```

### 3. Multiple Startup Projects beállítása

A projekt kizárólag Visual Studio-ban futtatható, Multiple Startup Projects konfigurációval.

1. Nyisd meg a `SmartGym.slnx` solution fájlt Visual Studio-ban
2. Jobb klikk a **Solution**-re a Solution Explorerben → **Set Startup Projects...**
3. Válaszd a **Multiple startup projects** opciót
4. Mindhárom projektet állítsd **Start** értékre:
   - `SmartGym` *(backend API)*
   - `GymFrontend` *(webes frontend)*
   - `SmartGymAdminWPF` *(admin felület)*
5. Kattints **OK**-ra, majd indítsd el **F5**-tel

---

## Tervezett fejlesztések

- [ ] Automatikus bérlet aktiválás
- [ ] Real-time értesítések (SignalR)
- [ ] Online fizetési integráció
- [ ] E-mail értesítések

---

## Készítők

Magyar Balázs · Imre Gábor · Tóth Martin
