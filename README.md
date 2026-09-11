# CarShare

CarShare je web aplikacija za P2P iznajmljivanje automobila. Platforma omogućava vlasnicima da postavljaju vozila, upravljaju dostupnošću i odgovaraju na rezervacije, dok renter korisnici mogu da pretražuju, rezervišu i ocenjuju vozila.

Projekat je izrađen kao studentski projekat iz predmeta Napredne baze podataka.

## Tehnologije

### Backend

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- PostGIS
- MediatR
- CQRS
- Repository Pattern
- Unit of Work
- AutoMapper
- FluentValidation
- JWT autentifikacija
- Refresh token rotacija
- Serilog
- Swagger/OpenAPI
- API versioning
- RFC 7807 Problem Details
- Docker

### Frontend

- React
- Vite
- React Router
- Axios
- Lucide React
- CSS
- OpenStreetMap Nominatim geocoding

## Glavne funkcionalnosti

### Korisnici

- Registracija i prijavljivanje
- JWT access i refresh tokeni
- Automatsko osvežavanje access tokena
- Uloge: Renter, Owner i Admin
- Pregled korisničkog profila
- Slanje vozačke dozvole na verifikaciju
- Zaštita frontend i backend ruta prema ulozi

### Vozila

- Postavljanje vozila
- Izmena i brisanje vozila
- Upload više fotografija sa računara
- Galerija fotografija
- Automatsko određivanje latitude i longitude koordinata
- Pretraga prema lokaciji i periodu
- Pregled detalja vozila
- Kalendar dostupnosti
- Kreiranje i brisanje perioda nedostupnosti
- Slanje dokumenta vozila na verifikaciju
- Administratorsko odobravanje i odbijanje vozila

### Rezervacije

- Slanje zahteva za rezervaciju
- Provera preklapanja termina
- Odobravanje i odbijanje zahteva
- Otkazivanje rezervacije
- Pregled renter rezervacija
- Pregled rezervacija vlasnika
- Izračunavanje procenjene cene
- Statusi rezervacije:
  - Na čekanju
  - Odobrena
  - Odbijena
  - U toku
  - Završena
  - Otkazana
  - Otvoren spor

### Plaćanja

- Mock plaćanje rezervacije
- Evidentiranje transakcije u bazi
- Bez povezivanja sa pravim platnim sistemom
- Test kartica: `4111111111111111`

### Primopredaja

- Potvrda preuzimanja od strane rentera i vlasnika
- Evidentiranje kilometraže pri preuzimanju
- Upload fotografije pri preuzimanju
- Potvrda vraćanja od strane obe strane
- Evidentiranje kilometraže i fotografije pri vraćanju

### Ocene i sporovi

- Ocenjivanje nakon završenog najma
- Ocena od 1 do 5
- Dodavanje komentara
- Prijavljivanje problema
- Administratorski pregled otvorenih sporova
- Rešavanje ili odbijanje spora

### Administracija

- Pregled statistike platforme
- Verifikacija vozačkih dozvola
- Verifikacija vozila
- Pregled i rešavanje sporova
- Ukupan broj korisnika
- Ukupan broj vozila
- Ukupan broj rezervacija
- Broj završenih rezervacija
- Broj otvorenih sporova
- Ukupan mock prihod

## Arhitektura

Backend koristi Clean Architecture organizaciju:

```text
src/
├── CarShare.API
├── CarShare.Application
├── CarShare.Domain
└── CarShare.Infrastructure
```

### CarShare.Domain

Sadrži:

- entitete
- enumeracije
- osnovne domenske klase
- poslovne modele

### CarShare.Application

Sadrži:

- CQRS komande i upite
- MediatR handlere
- validatore
- ViewModel klase
- interfejse repozitorijuma
- Unit of Work interfejs
- autentifikacione modele

### CarShare.Infrastructure

Sadrži:

- Entity Framework Core
- `CarShareDbContext`
- implementaciju repozitorijuma
- Unit of Work
- PostgreSQL konfiguraciju
- JWT servis
- persistence konfiguraciju
- migracije baze

### CarShare.API

Sadrži:

- kontrolere
- API ugovore
- middleware
- globalni exception handler
- Swagger konfiguraciju
- autentifikaciju i autorizaciju
- upload fajlova
- Serilog konfiguraciju

## Baza podataka

Koristi se PostgreSQL sa PostGIS ekstenzijom.

Glavne tabele:

- Users
- Vehicles
- VehiclePhotos
- Equipment
- VehicleEquipment
- AvailabilityBlocks
- Reservations
- Payments
- RentalHandovers
- Reviews
- Disputes
- RefreshTokens

PostGIS se koristi za rad sa geografskim koordinatama i pretragu vozila prema udaljenosti.

## Preduslovi

Potrebno je instalirati:

- .NET SDK 8
- Node.js
- npm
- Docker Desktop
- Git

Provera instalacija:

```bash
dotnet --version
node --version
npm --version
docker --version
git --version
```

## Pokretanje projekta

### 1. Kloniranje repozitorijuma

```bash
git clone https://github.com/HalilSacirovic/CarShare.git
cd CarShare
```

### 2. Pokretanje PostgreSQL baze

```bash
docker compose up -d carshare-db
```

Provera:

```bash
docker compose ps
```

Kontejner `carshare-postgres` treba da ima status `healthy`.

### 3. Primena migracija

Ako se projekat pokreće prvi put na praznoj bazi:

```bash
dotnet ef database update \
  --project src/CarShare.Infrastructure \
  --startup-project src/CarShare.API
```

Na Windows Command Prompt terminalu komanda može da se napiše u jednom redu:

```cmd
dotnet ef database update --project src/CarShare.Infrastructure --startup-project src/CarShare.API
```

### 4. Pokretanje API-ja kroz Docker

```bash
docker compose up -d --build carshare-api
```

Ili pokretanje cele Docker konfiguracije:

```bash
docker compose up -d --build
```

Swagger je dostupan na:

```text
http://localhost:8080/swagger/index.html
```

API osnovna adresa:

```text
http://localhost:8080/api/v1
```

### 5. Pokretanje React frontenda

```bash
cd frontend
npm install
npm run dev
```

Frontend je dostupan na:

```text
http://localhost:5173
```

## Demo korisnici

Demo podaci služe isključivo za lokalno testiranje.

### Administrator i vlasnik

```text
Email: halil@example.com
Lozinka: Test123!
```

### Renter

```text
Email: renter@example.com
Lozinka: Test123!
```

### Vlasnik i renter

```text
Email: petar@example.com
Lozinka: Test123!
```

## Primer toka korišćenja

### Postavljanje vozila

1. Prijaviti se kao vlasnik.
2. Otvoriti `Moja vozila`.
3. Kliknuti `Dodaj vozilo`.
4. Uneti podatke o vozilu.
5. Izabrati fotografije sa računara.
6. Kreirati vozilo.
7. Poslati dokument vozila na proveru.
8. Administrator odobrava vozilo.
9. Vozilo se prikazuje na glavljenom spisku dostupnih vozila.

### Rezervacija

1. Renter pronalazi verifikovano vozilo.
2. Bira slobodan period.
3. Šalje zahtev.
4. Vlasnik odobrava zahtev.
5. Renter izvršava mock plaćanje.
6. Obe strane potvrđuju preuzimanje.
7. Status rezervacije prelazi u `U toku`.
8. Obe strane potvrđuju vraćanje.
9. Status rezervacije prelazi u `Završena`.
10. Korisnici mogu ostaviti ocenu ili prijaviti spor.

## Upload fajlova

Dozvoljeni formati:

- JPG
- JPEG
- PNG
- WEBP
- PDF

Maksimalna veličina jednog fajla je 10 MB.

Fajlovi se čuvaju u:

```text
wwwroot/uploads
```
