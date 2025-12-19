# TrallaApp – Platformă de Task Management

## 👥 Echipa

1. **Barcan Silviu-Ioan** [@sigutz](https://github.com/sigutz) – Grupa 244
2. **Cocut Ioana-Maria** [@ioanyaa](https://github.com/ioanyaa) – Grupa 244

---

TrallaApp este o aplicație web de tip **Task & Project Management** (similară cu Trello), dezvoltată pentru laboratorul **Dezvoltarea Aplicațiilor Web (2025–2026)**.

Aplicația permite:

* crearea și gestionarea proiectelor
* invitarea membrilor într-un proiect
* organizarea task-urilor
* generarea de rezumate folosind AI

---

## 🛠️ Tehnologii utilizate

* **Framework:** ASP.NET Core MVC (.NET 9)
* **Limbaj:** C#
* **ORM:** Entity Framework Core
* **Bază de date:** MySQL 8.4
* **Containerizare:** Docker & Docker Compose

---

# 🐳 Ghid de instalare și rulare (Docker)

Acest ghid explică rularea aplicației folosind Docker, inclusiv:

* prima inițializare a bazei de date
* rularea migrațiilor
* ce trebuie făcut când dai `pull` de pe GitHub

---

## 1️⃣ Instalare Docker Desktop

Asigură-te că Docker Desktop este instalat și pornit.

* **Windows:** [https://docs.docker.com/desktop/setup/install/windows-install/](https://docs.docker.com/desktop/setup/install/windows-install/)
* **MacOS:** [https://docs.docker.com/desktop/setup/install/mac-install/](https://docs.docker.com/desktop/setup/install/mac-install/)
* **Linux:** [https://docs.docker.com/desktop/setup/install/linux/](https://docs.docker.com/desktop/setup/install/linux/)

---

## 2️⃣ Clonare repository

```bash
git clone https://github.com/sigutz/TrallaApp.git
cd TrallaApp
```

---

## 3️⃣ Configurare Environment (.env)

Creează un fișier `.env` în **root-ul proiectului** (lângă `docker-compose.yml`).

Copiază în el variabilele de mediu primite.

> ⚠️ **Important:** valoarea `DOCKER_PROJECT_NAME` trebuie să corespundă cu numele folderului proiectului.

---

## 4️⃣ Pornirea aplicației

```bash
docker compose up -d
```

Așteaptă până când containerul de MySQL devine `healthy`.

### 🔧 Notă pentru Windows

Dacă apar erori legate de `entrypoint.sh` (semnul cel mai bun că acesta este cazul: trallaapp-app-1 din docker nu rulează):

* fișierul trebuie să fie **LF**, nu **CRLF**
* după conversie, rulează:

```bash
docker compose down -v
docker compose build --no-cache
docker compose up -d
```

---

## 5️⃣ Instalarea dependențelor (.NET packages)

Toate pachetele necesare sunt deja definite în fișierele `.csproj`.

Nu este nevoie să instalezi pachete manual.

Rulează o singură comandă:

```bash
docker compose run --rm sdk dotnet restore
```

Aceasta va instala **automat toate dependințele** proiectului.

---

## 6️⃣ Migrații – prima rulare

La prima rulare, baza de date este **goală**.

Trebuie doar să aplici migrațiile existente:

```bash
docker compose run --rm sdk dotnet ef database update
```

✔️ **Nu creezi migrații noi la prima rulare**

---

## 7️⃣ Ce faci când dai `git pull`

### 🔹 Cazul 1: Nu apar migrații noi

Doar pornești aplicația:

```bash
docker compose up -d
```

---

### 🔹 Cazul 2: Apar migrații noi în repo

Aplici migrațiile:

```bash
docker compose run --rm sdk dotnet ef database update
```

Atât. Nu trebuie nimic altceva.

---

## 8️⃣ Când SE creează migrații noi

Migrațiile se creează **doar** atunci când:

* se modifică modelele (`Models/`)
* se schimbă structura bazei de date

### Creare migrație nouă:

```bash
docker compose run --rm sdk dotnet ef migrations add NumeMigrare
```

### Aplicare migrație:

```bash
docker compose run --rm sdk dotnet ef database update
```

---

## 9️⃣ Versionare și update-uri majore

Proiectul folosește **Semantic Versioning**: `vX.Y.Z`

* **X (Major):** modificări structurale în DB
* **Y (Minor):** funcționalități noi
* **Z (Patch):** bugfix-uri

### 🔥 Regula importantă

➡️ Dacă **Major Version (X)** se schimbă, trebuie **obligatoriu**:

```bash
docker compose run --rm sdk dotnet ef database update
```

Migrațiile sunt deja în repo – trebuie doar aplicate.

---

## 🌐 Acces aplicație

Aplicația este disponibilă la:

```
http://localhost:8080
```

(sau portul configurat în `docker-compose.yml`)

---

## ✅ TL;DR (cel mai important)

```bash
# instalare dependințe
docker compose run --rm sdk dotnet restore

# aplicare migrații (prima rulare / după pull)
docker compose run --rm sdk dotnet ef database update
```

---

## Checklist cerinte:

### 1. Autentificare si roluri (cerinta 1):
- [ ] Implementare ASP.NET Identity (Login, Register, Logout)
- [ ] Configurare roluri:
    - [ ] **Administrator** (rol eplicit in BD)
    - [ ] **Membru** (utilizator standard inregistrat)
    - [ ] **Organizator** (creatorul unui proiect)
    - [ ] **Vizitator** (utilizator neinregistrat)
- [ ] Restrictionarea accesului pe baza rolului

### 2. Pagina de prezentare - Landing Page (cerinta 2):
- [ ] Design atractiv pentru vizitatorii neinregistrati
- [ ] Sectiuni obligatorii:
    - [ ] Descrierea platformei
    - [ ] Functionalitati
    - [ ] Testimoniale
    - [ ] Butoate de Call to Action
- **Nota** NU trebuie sa fie doar o pagina simpla de Login!!!

### 3. Gestionarea Proiectelor (cerinta 3 & 4):
- [ ] Crearea entitatii Proiect:
    - [ ] Id
    - [ ] Titlu
    - [ ] Descriere
    - [ ] Data Crearii
    - [ ] Id Organizator
- [ ] CRUD Proiecte (accesibil doar pentru membrii):
- [ ] Logica de back-end (ex: cel care creeaza proiectul devine Organizator)
- [ ] Pagina dedicata pentru fiecare proiect
- [ ] **Gestionarea echipei**:
    - [ ] Posibilitatea de a invita membrii (prin username/email)
    - [ ] Posibilitatea de a elimina membrii 
    - [ ] Vizualizarea listei de membri

### 4. Gestionarea Task-urilor (cerinta 5 & 6):
- [ ] Crearea entitatii Task (Titlu, Descriere, Status, Data Inceperii, Data Finalizare, Media Contents)
- [ ] **Validari:**
    - [ ] Toate campurile obligatorii
    - [ ] Data Finalizare > Data Inceperii
- [ ] Implementare upload media sau embed video (YouTube)
- [ ] **Workflow:**
    - [ ] Organizatorul poate crea/edita/sterge task-uri
    - [ ] Organizatorul poate atribui task-uri membrilor
    - [ ] Statusuri disponibile: Not Started, In Progress, Completed
    - [ ] Membrii pot vizualiza task-urile si le pot actualiza statusul

### 5. Sistem de comentarii (cerinta 7):
- [ ] Implementare entitate Comment
    - [ ] Continut
    - [ ] Data
    - [ ] Id 
    - [ ] Id Utilizator
- [ ] Afisarea comentariilor in pagina task-ului (cronologic)
- [ ] CRUD comentarii:
    - [ ] Adaugare comentariu (validare: nu poate fi gol)
    - [ ] Editare/Steregerea propriului comenatariu

### 7. Dashboard personalizat (cerinta 8):
- [ ] Creare Contoller/View pt dashboard
- [ ] Afisare proiecte active ale utilizatorului
- [ ] Afisare task-uri curente grupate dupa status
- [ ] Evidentiare deadline apropiate (ex: rosu daca e in mai putin de 24h)
- [ ] Filtre functionale

### 8. Integrare AI (cerinta 9):
- [ ] Configurare serviciu API in backend
- [ ] Creare prompt care primeste datele proiectului (task-uri, status, deadlines)
- [ ] Buton "Actualizeaza raportul proiectului" (pt organizator)
- [ ] Salvarea raspunsului AI in baza de date (pentru a nu apela API-ul la fiecare refresh)
- [ ] Afisare mesaj default daca nu exista activitate

### 9. Zona de Administrare (cerinta 10):
- [ ] panou Admin accesibil doar rolului Administrator
- [ ] Gestionare utilizatori (listare, dez/activare, stergere)
- [ ] Gestionare proiecte (vizualizare toate proiecte, stergere continut inadecvat)
- [ ] Gestionare task-uri/comentarii globale

### 10. Calitatea proiectului (cerinta 11):
- [ ] Organizare corecta a aplicatiei MCV
- [ ] Validari de  dare si mesaje de eroare clare
- [ ] Seed de date realist(min):
    - [ ] 3 utilizatori
    - [ ] 3 proiecte    
    - [ ] 5 taskuri
- [ ] README complet

---

## Roadmap: 
