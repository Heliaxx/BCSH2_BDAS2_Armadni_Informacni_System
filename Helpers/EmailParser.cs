using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Helpers
{
    public class EmailParser
    {
        public static string GenerateEmail(string jmeno, string prijmeni)
        {
            if (string.IsNullOrWhiteSpace(jmeno) || string.IsNullOrWhiteSpace(prijmeni))
            {
                throw new ArgumentException("Jméno a příjmení nesmí být prázdné.");
            }

            // Převod na malá písmena a odstranění diakritiky
            var normalizedJmeno = Normalize(jmeno);
            var normalizedPrijmeni = Normalize(prijmeni);

            // Vytvoření emailu
            return $"{normalizedJmeno}.{normalizedPrijmeni}@vojak.com";
        }

        private static string Normalize(string input)
        {
            var normalized = input.Trim().ToLower();
            return string.Concat(normalized.Normalize(System.Text.NormalizationForm.FormD)
                                          .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
                                                      System.Globalization.UnicodeCategory.NonSpacingMark));
        }
    }
}
