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