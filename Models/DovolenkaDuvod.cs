using System;

public class DuvodDovolenky
{
    public string Duvod { get; set; }
    public DateTime DatumOd { get; set; }
    public DateTime DatumDo { get; set; }

    public string DisplayValue => $"{Duvod} (od {DatumOd.ToShortDateString()} do {DatumDo.ToShortDateString()})";
}
