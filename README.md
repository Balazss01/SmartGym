# SmartGym

SmartGym egy ASP.NET Core alapú webalkalmazás, amely edzőtermi rendszerként működik.  
Lehetővé teszi bérletek kezelését, vásárlását, valamint felhasználói értesítések megjelenítését.

---

## Fő funkciók

### Felhasználók
- Regisztráció / bejelentkezés (JWT alapú)
- Profil kezelés

### Bérletek
- Bérlet vásárlás
- Több bérlet kezelése
- Stackelt bérletek támogatása (egymás után indulnak)
- Aktív / lejárt / jövőbeli bérletek megjelenítése

### Szekrények
- Aktív foglalás ellenőrzése
- Automatikus státusz (nyitott / zárt)
- Felhasználóhoz kötött foglalások

### Értesítések
- Értesítések megjelenítése
- Olvasott / olvasatlan állapot
- UI badge

---

## Bérlet logika

A rendszer támogatja a stackelt bérleteket.

Ha van aktív bérlet:
- az új bérlet nem azonnal indul
- hanem a jelenlegi lejárata után

### Példa:

```
Havi: 04.01 - 04.30
Heti: 04.30 - 05.07
```

## Projekt struktúra

```
SmartGym/
├── GymWebApiBackend/
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
└── README.md

```

## Technológiák

- ASP.NET Core
- Razor Pages
- Entity Framework Core
- MySQL
- JWT Authentication
- Bootstrap 5

---

## Futtatás

1. Backend indítása:
```
GymWebApiBackend
```

2. Frontend & WPF (Admin site) indítása:
```
GymFrontend
SmartGymAdminWPF
```

## További fejlesztések

- Automatikus bérlet aktiválás
- Real-time értesítések (SignalR)
- Fizetés integráció
- Admin felület

---

## Készítette

Magyar Balázs & Imre Gábor & Tóth Martin
