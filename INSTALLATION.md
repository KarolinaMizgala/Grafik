## INSTRUKCJA INSTALACJI I KONFIGURACJI

### 1. Wymagania wstępne
- Visual Studio 2019 lub nowsze
- .NET Framework 4.7.2

**UWAGA:** Baza danych SQLite jest wbudowana w projekt - brak konieczności instalacji SQL Server!

### 2. Otwieranie projektu
1. Otwórz folder `C:\Users\karol\source\repos\ScheduleApp` w Visual Studio
2. Otwórz plik `ScheduleApp.sln`

### 3. Przywracanie pakietów NuGet
1. Tools → NuGet Package Manager → Manage NuGet Packages for Solution
2. Kliknij "Restore" aby przywrócić pakiety
   
   ALBO w Package Manager Console:
   ```
   Update-Package -Reinstall
   ```

### 4. Konfiguracja bazy danych (OPCJONALNE)

Baza danych będzie utworzona **AUTOMATYCZNIE** przy pierwszym uruchomieniu!

Jeśli chcesz ją stworzyć ręcznie, otwórz Package Manager Console i wpisz:
```
Update-Database
```

### 5. Uruchomienie aplikacji
1. Naciśnij **F5** lub Debug → Start Debugging
2. Aplikacja otworzy się w domyślnej przeglądarce
3. Powinna pokazać stronę główną z linkami do Harmonogramu i Pracowników

### 6. Pierwsze kroki
- Kliknij "Zarządzaj pracownikami"
- Dodaj kilka pracowników (testowych dane już są preload'owane)
- Wróć do Harmonogramu
- Kliknij na komórkę aby dodać wpis do harmonogramu

### WAŻNE - Jeśli pojawią się błędy:

**Error: Unable to load entity 'System.Data.SQLite'**
- Przywróć pakiety NuGet
- Przebuduj projekt: Ctrl+Shift+B

**Error: Cannot open database**
- Sprawdź folder `App_Data` - powinien tam być plik `ScheduleApp.db`
- Jeśli go brakuje, usuń go i uruchom aplikację ponownie (zostanie stworzony automatycznie)

**Baza nie tworzy się automatycznie**
Uruchom w Package Manager Console:
```
Update-Database
```

**Aplikacja się nie uruchamia**
1. Przebuduj: Ctrl+Shift+B
2. Wyczyść: Build → Clean Solution, potem Build → Build Solution
3. Sprawdź czy nie ma błędów w Error List

### Lokalizacja bazy danych

Baza SQLite znajduje się w: `App_Data/ScheduleApp.db`

Możesz ją:
- **Przeglądać** za pomocą SQLite Browser (darmowe narzędzie)
- **Usunąć** jeśli chcesz wyczyścić dane (zostanie automatycznie stworzona na nowo)
- **Backupować** zwykłym skopiowaniem pliku

### Struktura projektu

```
ScheduleApp/
├── Controllers/           # Logika aplikacji
│   ├── HomeController.cs
│   ├── EmployeesController.cs      (Zarządzanie pracownikami)
│   └── SchedulesController.cs      (Harmonogramy)
├── Models/               # Struktury danych
│   ├── Employee.cs
│   ├── ScheduleEntry.cs
│   └── ScheduleViewModel.cs
├── Views/               # Interfejs użytkownika
│   ├── Home/
│   ├── Employees/      (Formularze pracowników)
│   ├── Schedules/      (Widok harmonogramu)
│   └── Shared/         (Szablon strony)
├── Data/
│   └── ScheduleContext.cs (Kontekst bazy SQLite)
├── Migrations/
│   └── Configuration.cs (Migracje bazy danych)
├── App_Data/
│   └── ScheduleApp.db  (BAZA DANYCH - AUTOMATYCZNIE TWORZONA)
└── Web.config           (Konfiguracja SQLite)
```

### Wskazówki

1. **Harmonogram** - Kliknij na komórkę aby edytować
2. **Pracownicy** - Dodaj pracownika zanim stworzysz harmonogram
3. **Tygodnie** - Użyj przycisków nawigacji aby przechodzić między tygodniami
4. **Typy dni** - Normalny dzień wymaga godzin (od-do)
5. **SQLite** - Baza jest w jednym pliku `.db` - łatwo przenosić/backupować

### Czyszczenie danych

Aby wyczyścić wszystkie dane i zacząć od nowa:

1. Zamknij aplikację
2. Usuń plik `App_Data\ScheduleApp.db`
3. Uruchom aplikację ponownie - baza będzie automatycznie stworzona z danymi testowymi

### Narzędzia do przeglądania SQLite

- **DB Browser for SQLite** - https://sqlitebrowser.org (darmowe)
- **SQLiteStudio** - https://sqlitestudio.pl (darmowe)

Powodzenia! 🚀
