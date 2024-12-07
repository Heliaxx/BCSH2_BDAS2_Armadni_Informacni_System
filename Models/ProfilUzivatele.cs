using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class ProfilUzivatele
    {
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public static class ProfilUzivateleManager
    {
        public static ProfilUzivatele CurrentUser { get; set; }
    }

}
