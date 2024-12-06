using System;

public class PrehledVojaci
{
    public int? IdVojak { get; set; }
    public string Jmeno { get; set; }
    public string Prijmeni { get; set; }
    public DateTime DatumNastupu { get; set; }
    public DateTime? DatumPropusteni { get; set; }
    public string Hodnost { get; set; }
    public string Jednotka { get; set; } = "Voják zatím nepatří do žádné jednotky";
    public int HodnostId { get; set; }
    public int? JednotkaId { get; set; }  
}
