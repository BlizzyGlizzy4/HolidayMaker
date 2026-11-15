using MySqlConnector;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Den kund operatören har valt. 0 betyder att ingen kund är vald.
int kundId = 0;
string kundNamn = "";

while (true)
{
    Console.WriteLine();
    Console.WriteLine(kundId == 0 ? "=== HOLIDAY MAKER ===" : $"=== HOLIDAY MAKER === Vald kund: {kundNamn}");
    Console.WriteLine("1. Registrera kund");
    Console.WriteLine("2. Välj kund");
    Console.WriteLine("3. Sök lediga rum och boka");
    Console.WriteLine("4. Visa kundens bokningar");
    Console.WriteLine("5. Ändra antal personer");
    Console.WriteLine("6. Avboka");
    Console.WriteLine("0. Avsluta");
    string val = Fraga("Välj");

    try
    {
        if (val == "1") RegistreraKund();
        else if (val == "2") ValjKund();
        else if (val == "3") SokOchBoka();
        else if (val == "4") VisaBokningar();
        else if (val == "5") AndraAntal();
        else if (val == "6") Avboka();
        else if (val == "0") break;
        else Console.WriteLine("Ogiltigt val.");
    }
    catch (MySqlException ex)
    {
        Console.WriteLine("Databasfel: " + ex.Message);
    }
}


// 1. Registrera kund
void RegistreraKund()
{
    string fornamn = Fraga("Förnamn");
    string efternamn = Fraga("Efternamn");
    string epost = Fraga("E-post");
    string telefon = Fraga("Telefon");

    using var conn = Db.Open();
    using var cmd = new MySqlCommand("""
        INSERT INTO Kund (Fornamn, Efternamn, Epost, Telefon)
        VALUES (@fornamn, @efternamn, @epost, @telefon);
        """, conn);

    cmd.Parameters.AddWithValue("@fornamn", fornamn);
    cmd.Parameters.AddWithValue("@efternamn", efternamn);
    cmd.Parameters.AddWithValue("@epost", epost);
    cmd.Parameters.AddWithValue("@telefon", telefon);

    try
    {
        cmd.ExecuteNonQuery();
        kundId = (int)cmd.LastInsertedId;  
        kundNamn = fornamn + " " + efternamn;
        Console.WriteLine($"Kunden har kundnummer {kundId} och är nu vald.");
    }
    catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
    {
        Console.WriteLine("E-postadressen finns redan. Välj kunden via meny 2.");
    }
}


// 2. Välj kund
void ValjKund()
{
    using var conn = Db.Open();
    using var cmd = new MySqlCommand("""
        SELECT KundId, Fornamn, Efternamn, Epost FROM Kund
        WHERE CONCAT(Fornamn, ' ', Efternamn) LIKE @sok OR Epost LIKE @sok
        ORDER BY Efternamn;
        """, conn);
    cmd.Parameters.AddWithValue("@sok", "%" + Fraga("Sök namn eller e-post (Enter = alla)", tomOk: true) + "%");

    var kunder = new Dictionary<int, string>();
    using (var r = cmd.ExecuteReader())
    {
        while (r.Read())
        {
            kunder[r.GetInt32("KundId")] = r.GetString("Fornamn") + " " + r.GetString("Efternamn");
            Console.WriteLine($"{r.GetInt32("KundId"),4}  {kunder[r.GetInt32("KundId")],-25} {r.GetString("Epost")}");
        }
    }

    // Bara en kund som visades i listan får väljas.
    int id = FragaTal("Kundnummer (0 = avbryt)", 0, int.MaxValue);
    if (!kunder.ContainsKey(id)) return;

    kundId = id;
    kundNamn = kunder[id];
    Console.WriteLine($"{kundNamn} är vald.");
}


// 3. Sök lediga rum och boka
void SokOchBoka()
{
    using var conn = Db.Open();

    using (var cmd = new MySqlCommand("SELECT OrtId, Namn FROM Ort ORDER BY Namn;", conn))
    using (var r = cmd.ExecuteReader())
    {
        while (r.Read()) Console.WriteLine($"{r.GetInt32("OrtId")}. {r.GetString("Namn")}");
    }

    int ortId = FragaTal("Välj ort", 1, int.MaxValue);
    DateTime incheckning = FragaDatum("Incheckning", new DateTime(2026, 12, 1), new DateTime(2026, 12, 31));
    DateTime utcheckning = FragaDatum("Utcheckning", incheckning.AddDays(1), new DateTime(2027, 1, 1));
    int antal = FragaTal("Antal personer", 1, 20);

    Console.WriteLine("Valfria filter, Enter hoppar över.");
    bool pool = Fraga("Måste ha pool? (j/n)", tomOk: true).ToLower() == "j";
    bool restaurang = Fraga("Måste ha restaurang? (j/n)", tomOk: true).ToLower() == "j";
    int? maxStrand = FragaValfrittTal("Max meter till strand", 0, 100000);
    int? maxCentrum = FragaValfrittTal("Max meter till centrum", 0, 100000);

    string orderBy = FragaTal("Sortera 1 = pris lägst först, 2 = omdöme högst först", 1, 2) == 2
        ? "b.Omdome DESC, r.PrisPerNatt"
        : "r.PrisPerNatt, b.Omdome DESC";

    
    using var sok = new MySqlCommand($"""
        SELECT r.RumId, b.Namn AS Boende, o.Namn AS Ort, r.Rumsnummer, r.MaxPersoner, r.PrisPerNatt, b.Omdome
        FROM Rum r
        JOIN Boende b ON b.BoendeId = r.BoendeId
        JOIN Ort o ON o.OrtId = b.OrtId
        WHERE b.OrtId = @ortId
          AND r.MaxPersoner >= @antal
          AND (@pool = FALSE OR b.HarPool = TRUE)
          AND (@restaurang = FALSE OR b.HarRestaurang = TRUE)
          AND (@maxStrand IS NULL OR b.AvstandStrandMeter <= @maxStrand)
          AND (@maxCentrum IS NULL OR b.AvstandCentrumMeter <= @maxCentrum)
          AND NOT EXISTS (
              SELECT 1 FROM Bokning bk
              WHERE bk.RumId = r.RumId AND bk.Incheckning < @ut AND bk.Utcheckning > @in)
        ORDER BY {orderBy};
        """, conn);
    sok.Parameters.AddWithValue("@ortId", ortId);
    sok.Parameters.AddWithValue("@antal", antal);
    sok.Parameters.AddWithValue("@pool", pool);
    sok.Parameters.AddWithValue("@restaurang", restaurang);
    sok.Parameters.AddWithValue("@maxStrand", maxStrand);
    sok.Parameters.AddWithValue("@maxCentrum", maxCentrum);
    sok.Parameters.AddWithValue("@in", incheckning);
    sok.Parameters.AddWithValue("@ut", utcheckning);

    var ledigaRum = new List<int>();
    using (var r = sok.ExecuteReader())
    {
        while (r.Read())
        {
            ledigaRum.Add(r.GetInt32("RumId"));
            Console.WriteLine(
                $"RumId {r.GetInt32("RumId"),-3} {r.GetString("Boende"),-22} {r.GetString("Ort"),-8} " +
                $"rum {r.GetString("Rumsnummer"),-4} {r.GetInt32("MaxPersoner")} pers  " +
                $"{r.GetDecimal("PrisPerNatt"),6:N0} kr/natt  omdöme {r.GetInt32("Omdome")}");
        }
    }

    if (ledigaRum.Count == 0) { Console.WriteLine("Inga lediga rum hittades."); return; }
    if (kundId == 0) { Console.WriteLine("Välj en kund först för att kunna boka."); return; }

    int rumId = FragaTal("RumId att boka (0 = boka inte)", 0, int.MaxValue);
    if (ledigaRum.Contains(rumId)) BokaRum(conn, rumId, incheckning, utcheckning, antal);
}

void BokaRum(MySqlConnection conn, int rumId, DateTime incheckning, DateTime utcheckning, int antal)
{
    using var tx = conn.BeginTransaction();

    // FOR UPDATE låser rummet tills transaktionen är klar.
    using (var las = new MySqlCommand("SELECT RumId FROM Rum WHERE RumId = @rumId FOR UPDATE;", conn, tx))
    {
        las.Parameters.AddWithValue("@rumId", rumId);
        las.ExecuteScalar();
    }

    using var kontroll = new MySqlCommand("""
        SELECT COUNT(*) FROM Bokning
        WHERE RumId = @rumId AND Incheckning < @ut AND Utcheckning > @in;
        """, conn, tx);
    kontroll.Parameters.AddWithValue("@rumId", rumId);
    kontroll.Parameters.AddWithValue("@in", incheckning);
    kontroll.Parameters.AddWithValue("@ut", utcheckning);

    if (Convert.ToInt32(kontroll.ExecuteScalar()) > 0)
    {
        Console.WriteLine("Rummet hann bli bokat. Gör en ny sökning.");
        return;
    }

    using var spara = new MySqlCommand("""
        INSERT INTO Bokning (KundId, RumId, Incheckning, Utcheckning, AntalPersoner)
        VALUES (@kundId, @rumId, @in, @ut, @antal);
        """, conn, tx);
    spara.Parameters.AddWithValue("@kundId", kundId);
    spara.Parameters.AddWithValue("@rumId", rumId);
    spara.Parameters.AddWithValue("@in", incheckning);
    spara.Parameters.AddWithValue("@ut", utcheckning);
    spara.Parameters.AddWithValue("@antal", antal);
    spara.ExecuteNonQuery();
    int bokningId = (int)spara.LastInsertedId;

    tx.Commit();

    Console.WriteLine($"Bokning klar! Bokningsnummer {bokningId}.");
    VisaBokningar(bokningId);
}


List<int> VisaBokningar(int bara = 0)
{
    var nummer = new List<int>();
    if (kundId == 0) { Console.WriteLine("Välj en kund först."); return nummer; }

    using var conn = Db.Open();

    using var cmd = new MySqlCommand("""
        SELECT bk.BokningId, b.Namn AS Boende, r.Rumsnummer, bk.Incheckning, bk.Utcheckning, bk.AntalPersoner,
               DATEDIFF(bk.Utcheckning, bk.Incheckning) AS Natter,
               DATEDIFF(bk.Utcheckning, bk.Incheckning) * r.PrisPerNatt AS Totalpris
        FROM Bokning bk
        JOIN Rum r ON r.RumId = bk.RumId
        JOIN Boende b ON b.BoendeId = r.BoendeId
        WHERE bk.KundId = @kundId AND (@bara = 0 OR bk.BokningId = @bara)
        ORDER BY bk.Incheckning;
        """, conn);
    cmd.Parameters.AddWithValue("@kundId", kundId);
    cmd.Parameters.AddWithValue("@bara", bara);

    using var r = cmd.ExecuteReader();
    while (r.Read())
    {
        nummer.Add(r.GetInt32("BokningId"));
        Console.WriteLine(
            $"Nr {r.GetInt32("BokningId"),-3} {r.GetString("Boende"),-22} rum {r.GetString("Rumsnummer"),-4} " +
            $"{r.GetDateTime("Incheckning"):yyyy-MM-dd} till {r.GetDateTime("Utcheckning"):yyyy-MM-dd}  " +
            $"{r.GetInt32("AntalPersoner")} pers  {r.GetInt32("Natter")} nätter  {r.GetDecimal("Totalpris"):N0} kr");
    }

    if (nummer.Count == 0) Console.WriteLine("Kunden har inga bokningar.");
    return nummer;
}


// 5. Ändra antal personer
void AndraAntal()
{
    var bokningar = VisaBokningar();
    int bokningId = FragaTal("Bokningsnummer (0 = avbryt)", 0, int.MaxValue);
    if (!bokningar.Contains(bokningId)) return;

    using var conn = Db.Open();

    // Villkoret MaxPersoner >= @antal gör att raden bara uppdateras om alla ryms.
    using var cmd = new MySqlCommand("""
        UPDATE Bokning bk JOIN Rum r ON r.RumId = bk.RumId
        SET bk.AntalPersoner = @antal
        WHERE bk.BokningId = @bokningId AND bk.KundId = @kundId AND r.MaxPersoner >= @antal;
        """, conn);
    cmd.Parameters.AddWithValue("@antal", FragaTal("Nytt antal personer", 1, 20));
    cmd.Parameters.AddWithValue("@bokningId", bokningId);
    cmd.Parameters.AddWithValue("@kundId", kundId);

    Console.WriteLine(cmd.ExecuteNonQuery() > 0
        ? "Antalet personer är ändrat."
        : "Så många personer ryms inte i rummet. Bokningen är oförändrad.");
}


// 6. Avboka
void Avboka()
{
    var bokningar = VisaBokningar();
    int bokningId = FragaTal("Bokningsnummer att avboka (0 = avbryt)", 0, int.MaxValue);
    if (!bokningar.Contains(bokningId)) return;

    using var conn = Db.Open();

    using var cmd = new MySqlCommand(
        "DELETE FROM Bokning WHERE BokningId = @bokningId AND KundId = @kundId;", conn);
    cmd.Parameters.AddWithValue("@bokningId", bokningId);
    cmd.Parameters.AddWithValue("@kundId", kundId);
    cmd.ExecuteNonQuery();

    Console.WriteLine($"Bokning {bokningId} är avbokad och rummet är ledigt igen.");
}


string Fraga(string fraga, bool tomOk = false)
{
    while (true)
    {
        Console.Write(fraga + ": ");
        string? svar = Console.ReadLine();
        if (svar == null) Environment.Exit(0);   
        svar = svar.Trim();
        if (svar != "" || tomOk) return svar;
        Console.WriteLine("Fältet får inte vara tomt.");
    }
}

// Enter betyder "inget värde" och då returneras null.
int? FragaValfrittTal(string fraga, int min, int max)
{
    while (true)
    {
        string svar = Fraga(fraga, tomOk: true);
        if (svar == "") return null;
        if (int.TryParse(svar, out int tal) && tal >= min && tal <= max) return tal;
        Console.WriteLine($"Ange ett heltal mellan {min} och {max}.");
    }
}

int FragaTal(string fraga, int min, int max)
{
    while (true)
    {
        if (int.TryParse(Fraga(fraga), out int tal) && tal >= min && tal <= max) return tal;
        Console.WriteLine($"Ange ett heltal mellan {min} och {max}.");
    }
}

DateTime FragaDatum(string fraga, DateTime tidigast, DateTime senast)
{
    while (true)
    {
        if (DateTime.TryParse(Fraga(fraga + " (ÅÅÅÅ-MM-DD)"), out DateTime datum)
            && datum >= tidigast && datum <= senast)
            return datum;
        Console.WriteLine($"Ange ett datum mellan {tidigast:yyyy-MM-dd} och {senast:yyyy-MM-dd}.");
    }
}