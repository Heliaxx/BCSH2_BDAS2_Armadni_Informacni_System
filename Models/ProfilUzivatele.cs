using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class ProfilUzivatele
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public int IdVojak { get; set; } 
        public string Jmeno { get; set; } 
        public string Prijmeni { get; set; } 
        public string Hodnost { get; set; } 
    }

    public static class ProfilUzivateleManager
    {
        public static ProfilUzivatele CurrentUser { get; set; }
        public static ProfilUzivatele OriginalUser { get; set; }
    }
}
