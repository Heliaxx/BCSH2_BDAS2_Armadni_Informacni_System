using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Entities
{
    public class Vojak
    {
        public int IdVojak { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public DateTime DatumNastupu { get; set; }
        public DateTime DatumPropusteni { get; set; }

        public decimal IdHodnost { get; set; }
        public decimal IdJednotka { get; set; }
        public decimal IdZbran { get; set; }

        public string Email { get; set; }
        public string Password { get; set; } //uložený hash hesla
    }
}