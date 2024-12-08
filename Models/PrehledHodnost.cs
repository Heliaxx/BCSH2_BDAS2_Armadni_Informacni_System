namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    internal class PrehledHodnost
    {
        public int IdHodnost { get; set; }
        public string Nazev { get; set; }
        public string Odmeny { get; set; }
        public string PotrebnyStupenVzdelani { get; set; }
        public int PotrebnyPocetLetVPraxi { get; set; }
        public decimal VahaHodnosti { get; set; }
        public int IdRole { get; set; }
        public string NazevRole { get; set; }
    }
}
