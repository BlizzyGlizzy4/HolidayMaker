## Innehåll
"docs/er-diagram.png" är ER diagrammet. 
"docs/Holidaymaker reflektkion.pdf" är min reflektion. 
"code" innehåller all kod.
"sql" innehåller alla sql filer. 

## 1. Starta MySQL

Om du kör MySQL i Docker:

```bash
docker run --name mysql-kurs -e MYSQL_ROOT_PASSWORD=kurslosenord -p 127.0.0.1:3306:3306 -v mysql-kurs-data:/var/lib/mysql -d mysql:8.4
```

Finns containern redan räcker `docker start mysql-kurs`.

## 2. Skapa databasen

Kör skripten i ordning, antingen i DBeaver/DataGrip (öppna filen och kör hela skriptet) eller från kommandoraden:

```bash
mysql -h 127.0.0.1 -u root -p < sql/Skapa_databas.sql
mysql -h 127.0.0.1 -u root -p < sql/Testdata.sql
```

`Skapa_databas.sql` tar bort och skapar om databasen `holidaymaker`, så skripten kan köras om för att återställa testdatan.

## 3. Ställ in anslutningen

Appen läser anslutningssträngen från miljövariabeln `HOLIDAYMAKER_DB`. Finns den inte används

```
Server=127.0.0.1;Port=3306;Database=holidaymaker;User ID=root;Password=kurslosenord;
```

Har du ett annat lösenord, sätt miljövariabeln innan du kör:

```powershell
# Windows PowerShell
$env:HOLIDAYMAKER_DB = "Server=127.0.0.1;Port=3306;Database=holidaymaker;User ID=root;Password=DITT_LÖSENORD;"
```

```bash
# macOS / Linux
export HOLIDAYMAKER_DB="Server=127.0.0.1;Port=3306;Database=holidaymaker;User ID=root;Password=DITT_LÖSENORD;"
```

## 4. Kör programmet

```bash
cd src/HolidayMaker
dotnet run
```

