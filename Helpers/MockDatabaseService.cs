using BCSH2_BDAS2_Armadni_Informacni_System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Helpers
{
    public class MockDatabaseService
    {
        private List<Vojak> mockVojaci;

        public MockDatabaseService()
        {
            // Inicializujeme mock data pro vojáky
            mockVojaci = new List<Vojak>
            {
                new Vojak { IdVojak = 41, Jmeno = "Jan", Prijmeni = "Novák", DatumNastupu = new DateTime(2021, 3, 1), Email = "honza.novak@vojak.com", Heslo = "4630e6add64f2b4b206e13fafec30c9e217a52b0e342df7ea77777f046957de2", IdHodnost = 1, IdJednotka = 1 },
                new Vojak { IdVojak = 42, Jmeno = "Petr", Prijmeni = "Kučera", DatumNastupu = new DateTime(2018, 5, 1), Email = "petr.kucera@vojak.com", Heslo = "4630e6add64f2b4b206e13fafec30c9e217a52b0e342df7ea77777f046957de2", IdHodnost = 2, IdJednotka = 5 },
                // Další vojáci podle vaší tabulky
            };
        }

        // CRUD operace

        // CREATE: Přidání nového vojáka
        public void AddVojak(Vojak newVojak)
        {
            newVojak.IdVojak = mockVojaci.Any() ? mockVojaci.Max(v => v.IdVojak) + 1 : 1;
            mockVojaci.Add(newVojak);
        }

        // READ: Získání všech vojáků
        public List<Vojak> GetAllVojaci()
        {
            return mockVojaci;
        }

        // READ: Získání vojáka podle emailu
        public Vojak GetVojakByEmail(string email)
        {
            return mockVojaci.FirstOrDefault(v => v.Email == email);
        }

        // READ: Získání vojáka podle ID
        public Vojak GetVojakById(int id)
        {
            return mockVojaci.FirstOrDefault(v => v.IdVojak == id);
        }

        // UPDATE: Aktualizace informací o vojákovi
        public void UpdateVojak(Vojak updatedVojak)
        {
            var vojak = mockVojaci.FirstOrDefault(v => v.IdVojak == updatedVojak.IdVojak);
            if (vojak != null)
            {
                vojak.Jmeno = updatedVojak.Jmeno;
                vojak.Prijmeni = updatedVojak.Prijmeni;
                vojak.Email = updatedVojak.Email;
                vojak.DatumNastupu = updatedVojak.DatumNastupu;
                vojak.DatumPropusteni = updatedVojak.DatumPropusteni;
            }
        }

        // DELETE: Odstranění vojáka podle ID
        public bool DeleteVojak(int id)
        {
            var vojak = GetVojakById(id);
            if (vojak != null)
            {
                mockVojaci.Remove(vojak);
                return true;
            }
            return false;
        }

        // Verifikace hesla pro přihlášení
        public bool VerifyPassword(string email, string hashedPassword)
        {
            var vojak = GetVojakByEmail(email);
            return vojak != null && vojak.Heslo == hashedPassword;
        }
    }
}