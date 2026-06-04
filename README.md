# Aplikacja Harmonogram Pracy - ASP.NET MVC

Aplikacja webowa do tworzenia i zarządzania harmonogramami pracy pracowników.

## Funkcjonalności

✅ **Zarządzanie pracownikami**
- Dodawanie nowych pracowników
- Edycja danych pracownika
- Usuwanie pracowników
- Śledzenie normy pracy (godzin i dni)

✅ **Tworzenie harmonogramów**
- Widok tygodniowy (poniedziałek - niedziela)
- Nawigacja między tygodniami
- Typy dni: Normalny, Wolne, Święto, Urlop
- Godziny pracy (od-do)
- Uwagi do każdego dnia

✅ **Baza danych**
- SQLite (baza w jednym pliku, BEZ konfiguracji)
- Entity Framework 6
- Automatyczne tworzenie przy pierwszym uruchomieniu

## Wymagania systemowe

- .NET Framework 4.7.2+
- Visual Studio 2019+

**Baza danych SQLite jest wbudowana - żadna dodatkowa instalacja nie jest wymagana!**

## Instalacja

1. **Otwórz projekt w Visual Studio**
   ```
   C:\Users\karol\source\repos\ScheduleApp
   ```

2. **Przywróć pakiety NuGet**
   - Tools → NuGet Package Manager → Package Manager Console
   ```
   Update-Package -Reinstall
   ```
ruchom aplikację**
   - Baza danych zostanie utworzona **AUTOMATYCZNIE** przy pierwszym uruchomieniu
   - Naciśnij F5 lub Debug → Start Debugging
   - Aplikacja otworzy się w przeglądarce
   - Aplikacja otworzy się w przeglądarce na http://localhost:...

## Struktura projektu

```
ScheduleApp/
├── Controllers/           # Kontrolery MVC
│   ├── HomeController.cs
│   ├── EmployeesController.cs
│   └── SchedulesController.cs
├── Models/               # Modele danych
│   ├── Employee.cs
│   ├── ScheduleEntry.cs
│   └── ScheduleViewModel.cs
├── Views/               # Widoki Razor
│   ├── Employees/
│   ├── Schedules/
│   └── Shared/
├── Data/                # Kontekst bazy danych
│   └── ScheduleContext.cs
└── Web.config           # Konfiguracja
```

## Jak używać

### Dodawanie pracownika
1. Wejdź na stronę "Pracownicy"
2. Kliknij "Dodaj nowego pracownika"
3. Wypełnij formularz (imię, nazwisko, normę godzin i dni)
4. Kliknij "Zapisz"

### Tworzenie harmonogramu
1. Na stronie "Harmonogram" kliknij na dowolną komórkę
2. Wybierz typ dnia (Normalny, Wolne, Święto, Urlop)
3. Jeśli normalny dzień - wpisz godziny (od-do)
4. Dodaj opcjonalne uwagi
5. Kliknij "Zapisz"

### Navigacja między tygodniami
- "← Poprzedni tydzień" - przechodzi do poprzedniego tygodnia
- "Dzisiaj" - wraca do bieżącego tygodnia
- "Następny tydzień →" - przechodzi do następnego tygodnia

## Konfiguracja bazy danych

Baza danych łączy się poprzez `ScheduleConnection` zdefiniowany w `Web.config`:

```xml
<connectionStrings>
    <add name="ScheduleConnection" 
         connectionString="Data Source=(LocalDB)\mssqllocaldb;AttachDbFilename=|DataDirectory|\ScheduleApp.mdf;..." />
</connectionStrings>
```

Pliki bazy danych będą tworzone w folderze `App_Data/`

## Troubleshooting

### Błąd: "Update-Database: No migrations found"
```
Enable-Migrations
Add-Migration InitialCreate
Update-Database
```

### Błąd: "The name 'ISOWeek' does not exist"
Upewnij się, że używasz .NET Framework 4.7.2+

### Aplikacja nie uruchamia się
1. Sprawdź czy wszystkie pakiety NuGet są zainstalowane
2. Przywróć pakiety: `Update-Package -Reinstall`
3. Przebuduj projekt: Ctrl+Shift+B

## Autor
Stworzono w ramach ASP.NET MVC

## Licencja
Projekt do użytku personalnego
