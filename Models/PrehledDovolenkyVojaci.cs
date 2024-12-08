using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class PrehledDovolenkyVojaci
    {
        public int IdDovolenka { get; set; }  
        public DateTime DatumOd { get; set; }  
        public DateTime DatumDo { get; set; }  
        public string Duvod { get; set; }    
        public int IdVojak { get; set; }     
        public string Jmeno { get; set; }   
        public string Prijmeni { get; set; }  
    }
}
