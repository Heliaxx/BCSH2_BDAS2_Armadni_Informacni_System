﻿namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class Hodnost
    {
        public int IdHodnost { get; set; }
        public string Nazev { get; set; }
        public string Odmeny { get; set; } = "Tato hodnost nemá odměny";
        public string PotrebnyStupenVzdelani { get; set; }
        public int PotrebnyPocetLetVPraxi { get; set; }
        public int VahaHodnosti { get; set; }
        public int IdRole { get; set; }
    }
}
