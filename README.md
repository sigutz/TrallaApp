# TrallaApp - Platforma de Task Management - Proiect ASP.NET Core MCV

### Echipa:
1. **Barcan Silviu-Ioan** [@sigutz](https://github.com/sigutz) -> Grupa 244
2. **Cocut Ioana-Maria** [@ioanyaa](https://github.com/ioanyaa) -> Grupa 244


Aceasta aplicatie este o platforma de gestionare a task-urilor si proiectelor precum Trello, dezvoltata pemtru laboratorul de Dezvoltarea Aplicatiilor Web 2025-2026. <br />
Aplicatia permite utilizatorilor sa creeze proiecte, sa invite membrii, sa gestioneze task-uri si sa genereze rezumate AI.

## Tehnologii Utilizate:
- **Framework:** ASP.NET Core MCV (.NET 9)
- **Limbaj:** C#
- **ORM:** Entity Framework Core
- **Baza de date:**

-----

# 🐳 TrallaApp - Ghid de Instalare și Rulare (Docker)

Acest ghid explică cum să configurezi și să rulezi aplicația folosind Docker, inclusiv pașii pentru prima configurare a bazei de date și procedurile pentru update-uri majore.

## 1\. Instalare Docker Desktop

Înainte de a începe, asigură-te că ai Docker Desktop instalat și pornit pe mașina ta.

  * **Windows:** [Instrucțiuni de instalare](https://docs.docker.com/desktop/setup/install/windows-install/) 
  * **MacOS:** [Instrucțiuni de instalare](https://docs.docker.com/desktop/setup/install/mac-install/) 
  * **Linux:** [Instrucțiuni de instalare](https://docs.docker.com/desktop/setup/install/linux/)

## 2\. Clonare Repo

Descarcă proiectul pe calculatorul tău:

```bash
git clone https://github.com/sigutz/TrallaApp.git
cd TrallaApp
```

## 3\. Configurare Environment

Creează un fișier numit `.env` în **root-ul proiectului** (lângă `docker-compose.yml`).
Copiază în el variabilele de mediu pe care ți le-am trimis în privat.

> **Notă:** Asigură-te că variabila `DOCKER_PROJECT_NAME` din fișier corespunde cu numele folderului proiectului.

## 4\. Pornire Aplicație

Deschide un terminal în folderul proiectului și rulează comanda pentru a descărca imaginile și a porni containerele în fundal:

```bash
docker compose up -d
```

Așteaptă câteva momente până când containerele sunt active.

## 5\. Configurarea Inițială a Bazei de Date (Doar la prima rulare)

Deoarece rulezi proiectul într-un container nou, baza de date este goală. Trebuie să generăm și să aplicăm migrațiile.

Rulează următoarele comenzi în ordine:

1.  **Oprește containerul aplicației** (pentru a elibera fișierele):

    ```bash
    docker compose stop app
    ```


2.  **Generează Migrația Inițială:**

    ```bash
    docker compose run --rm app sh -c "cd /src/DockerProject && dotnet ef migrations add InitialMigration"
    ```

    *(Notă: Dacă primești eroare de path, verifică dacă numele folderului din container este diferit de `/src/TrallaApp`)*.

3.  **Repornește aplicația:**

    ```bash
    docker compose start app
    ```


4.  **Aplică Migrația pe Baza de Date:**

    ```bash
    docker compose exec app dotnet ef database update
    ```


Acum aplicația ar trebui să fie accesibilă la `http://localhost:8080` (sau portul definit în configurare).

-----

## ⚠️ Procedură Update Major (Versiuni v X.Y.Z)

Proiectul folosește versionare semantică (`v X.Y.Z`).

**Regulă:** De fiecare dată când **Major Version (X)** se schimbă (ex: treci de la v1.2.0 la v2.0.0), înseamnă că au existat modificări structurale în baza de date. Trebuie să rulezi manual o nouă migrație.

### Pași pentru update de versiune majoră:

1.  **Oprește aplicația:**

    ```bash
    docker compose stop app
    ```

2.  **Creează Migrația de Update** (Dă-i un nume relevant, ex: `Update_v2`):

    ```bash
    docker compose run --rm app sh -c "cd /src/DockerProject && dotnet ef migrations add Update_Major_vX"
    ```

3.  **Pornește aplicația:**

    ```bash
    docker compose start app
    ```

4.  **Actualizează Baza de Date:**

    ```bash
    docker compose exec app dotnet ef database update
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
