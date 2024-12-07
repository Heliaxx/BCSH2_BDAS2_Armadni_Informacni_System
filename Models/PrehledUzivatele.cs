using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class PrehledUzivatele
    {
        public int id_vojak { get; set; } 
        public string jmeno { get; set; }
        public string prijmeni { get; set; }
        public string email { get; set; }
        public string nazev_hodnosti { get; set; }
        public string nazev_role { get; set; }
    }
}
