#  SmartGym

**SmartGym** egy ASP.NET Core alapú edzőtermi menedzsment rendszer, amely lehetővé teszi bérletek kezelését és vásárlását, szekrényfoglalások nyomon követését. A projekt három fő komponensből áll: egy RESTful backend API-ból, egy Razor Pages alapú webes frontendből, és egy WPF-alapú adminisztrációs asztali alkalmazásból.

---

##  Tartalomjegyzék

- [Funkciók](#-funkciók)
- [Technológiák](#-technológiák)
- [Projekt struktúra](#-projekt-struktúra)
- [Bérlet logika](#-bérlet-logika)
- [Telepítés és futtatás](#-telepítés-és-futtatás)
- [Adatbázis](#-adatbázis)
- [További fejlesztések](#-további-fejlesztések)
- [Készítők](#-készítők)

---

##  Funkciók

###  Felhasználók
- Regisztráció és bejelentkezés (JWT alapú hitelesítés)
- Profil megtekintése és kezelése

###  Bérletek
- Bérlet vásárlása (heti, havi stb.)
- Több bérlet egyidejű kezelése
- **Stackelt bérletek** támogatása — a bérletek egymás után indulnak
- Aktív, jövőbeli és lejárt bérletek áttekintése

###  Szekrények
- Aktív foglalás ellenőrzése
- Automatikus státuszkezelés (nyitott / zárt)
- Felhasználóhoz kötött foglalások

###  Admin felület (WPF)
- Asztali adminisztrációs alkalmazás Windows platformon
- Felhasználók, bérletek és szekrények kezelése

---

##  Technológiák

| Réteg | Technológia |
|---|---|
| Backend | ASP.NET Core Web API |
| Frontend | Razor Pages, Bootstrap 5 |
| Admin felület | WPF (.NET) |
| ORM | Entity Framework Core |
| Adatbázis | MySQL |
| Hitelesítés | JWT (JSON Web Token) |

---

##  Projekt struktúra

```
SmartGym/
├── SmartGym/                  
│   ├── Controllers/           
│   ├── Models/                
│   ├── Data/                  
│   └── Services/             
│
├── GymFrontend/              
│   ├── Pages/                
│   ├── Shared/                
│   └── wwwroot/               
│
├── SmartGymAdminWPF/          
│   ├── Services/              
│   ├── Views/                
│   ├── App.xaml              
│   └── MainWindow.xaml        
│
├── SmartGym.slnx             
├── SwaggerTesztek             
└── README.md
```

---

##  Bérlet logika

A rendszer **stackelt bérleteket** támogat: ha a felhasználónak már van aktív bérlete, az új bérlet nem azonnal, hanem az aktuális bérlet lejárta után kezdődik.

**Példa:**
```
Havi bérlet:  2025.04.01 → 2025.04.30
Heti bérlet:  2025.04.30 → 2025.05.07   ← automatikusan indul
```

---

##  Telepítés és futtatás

### Előfeltételek
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MySQL szerver (pl. XAMPP, MySQL Workbench)
- Visual Studio 2022+ (ajánlott)

### 1. Adatbázis beállítása

- Létre kell hozni egy adatbázist smartgym néven
- Visual Studioban, Package Manager Consoleban ki kell adni az alábbi parancsot: "Update-database"

### 2. Connection string beállítása

Az `appsettings.json` fájlban add meg a saját adatbázis-kapcsolatot:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;database=smartgym;user=root;password=JELSZO;"
}
```

### 3. Backend indítása

```bash
cd SmartGym
dotnet run
```

Az API alapértelmezés szerint a `https://localhost:7XXX` címen fut. A Swagger UI elérhető: `/swagger`

### 4. Frontend indítása

```bash
cd GymFrontend
dotnet run
```

### 5. Admin felület indítása

Nyisd meg a `SmartGymAdminWPF` projektet Visual Studio-ban, és futtasd.

---

##  További fejlesztések

-  Automatikus bérlet aktiválás (háttérfolyamat / Hangfire)
-  Valós idejű értesítések (SignalR)
-  Online fizetési integráció
-  E-mail értesítések
-  Mobilbarát dizájn fejlesztése

---

##  Készítők

| Magyar Balázs |
| Imre Gábor |
| Tóth Martin |

---

> *Vizsgaremek projekt – SmartGym edzőtermi menedzsment rendszer*
