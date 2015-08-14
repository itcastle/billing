using System;

namespace GestionCommerciale.Views.Prints
{
    public class NumberToWords
    {
        private static string[] m_Unite = new string[] { "zero ", "un ", "deux ", "trois ", "quatre ", "cinq ", "six ", "sept ", "huit ", "neuf " };
        private static string[] m_FirstDizaine = new string[] { String.Empty, "onze ", "douze ", "treize ", "quatorze ", "quinze ", "seize ", "dix-sept ", "dix-huit ", "dix-neuf " };
        private static string[] m_Dizaine = new string[] { String.Empty, "dix ", "vingt ", "trente ", "quarante ", "cinquante ", "soixante ", "soixante ", "quatre-vingt ", "quatre-vingt " };
        private static string[] m_CentMille = new string[] { "cent ", "cents ", "mille ", "million ", "millions ", "milliard ", "milliards " };

        private static string m_PrefixAnd = "et ";

        private static string[] m_Devise = new string[] { "dinar", "dinars" };

        private static string[] m_Centime = new string[] { "centime", "centimes" };

        private static string IntegerToFrenchWords(int pNumber)
        {
            if (pNumber == 0)
                return m_Unite[0]; // zéro

            bool isNumberNeg = false;
            if (pNumber < 0)
            {
                isNumberNeg = true;
                pNumber = pNumber * -1;
                // PB si p_Number = -2147483648 => p_Number * -1 = -2147483648, 
                // car la valeur max d'un entier est 2147483647 et pas 2147483648
                if (pNumber == Int32.MinValue)
                    throw new ArgumentException(string.Format("Impossible de convertir en lettres le nombre {0}", pNumber), "pNumber");
            }

            // Déclarations
            var is10Or70Or90 = false;
            var intInWords = String.Empty;

            var reste = pNumber;

            for (int i = 1000000000; i >= 1; i /= 1000)
            {
                var y = reste / i;
                if (y != 0)
                {
                    var centaine = y / 100;
                    var dizaine = (y - centaine * 100) / 10;
                    var unite = y - (centaine * 100) - (dizaine * 10);

                    // Centaines
                    if (centaine == 1)
                        intInWords += m_CentMille[0];
                    else if (centaine > 1)
                    {
                        intInWords += m_Unite[centaine];
                        if ((dizaine == 0) && (unite == 0)) intInWords += m_CentMille[1];
                        else intInWords += m_CentMille[1];
                    }

                    // Convertit les dizaines et unités en lettres
                    intInWords += GetUniteAndDizaineWords(dizaine, unite, ref is10Or70Or90);

                    // Milliers, millions, milliards
                    switch (i)
                    {
                        case 1000000000:
                            if (y > 1) intInWords += m_CentMille[6];
                            else intInWords += m_CentMille[5];
                            break;
                        case 1000000:
                            if (y > 1) intInWords += m_CentMille[4];
                            else intInWords += m_CentMille[3];
                            break;
                        case 1000: // cas particulier si unité = 1 -> pas de préfix "un"
                            intInWords = unite == 1 ? m_CentMille[2] : String.Concat(intInWords, m_CentMille[2]);
                            break;
                    }
                }

                reste -= y * i;
                is10Or70Or90 = false;
            }

            if (isNumberNeg)
                intInWords = "moins " + intInWords;

            return intInWords;
        }

        public static string CurrencyNumberToFrenchWords(double pNumber)
        {
            if (pNumber.Equals(0)) 
                return m_Unite[0]; // zéro

            // Déclarations
            bool is10Or70Or90 = false;

            // Récupère la partie entière et détermine la devise au singulier/pluriel
            int partEntiere = (int)pNumber;
            string devise = partEntiere > 1 ? m_Devise[1] : m_Devise[0];

            // Récupère la partie entière
            var intInWords = IntegerToFrenchWords(partEntiere);

            // Traitement de la virgule
            var chiffreDec = (decimal)(pNumber * 100) % 100;
            var dizaine = (int)(chiffreDec) / 10;
            var unite = (int)chiffreDec - (dizaine * 10);

            // Convertit les dizaines et unités en lettres
            string decInWords = GetUniteAndDizaineWords(dizaine, unite, ref is10Or70Or90);

            if (decInWords == String.Empty)
                return String.Concat(intInWords, devise);
            if (dizaine == 0 && unite == 1)
                return String.Format("{0}{1} et {2}{3}", intInWords, devise, decInWords, m_Centime[0]);
            return String.Format("{0}{1} et {2}{3}", intInWords, devise, decInWords, m_Centime[1]);
        }

        private static string GetUniteAndDizaineWords(int pDizaine, int pUnite, ref bool pIs10Or70Or90)
        {
            string numberInWords = String.Empty;

            // dizaines
            if (pDizaine == 1) // Flag la dizaine particulière
            {
                pIs10Or70Or90 = true;
            }
            else if (pDizaine > 1)
            {
                // Concatène avec la dizaine appropriée
                numberInWords += m_Dizaine[pDizaine];

                if (pDizaine == 7 || pDizaine == 9) // Flag la dizaine particulière
                    pIs10Or70Or90 = true;
            }


            // unités
            if (pUnite == 0 && pIs10Or70Or90)
            {
                numberInWords += m_Dizaine[1]; // dix
            }
            else if (pUnite == 1)
            {
                if (pDizaine == 7) numberInWords = String.Concat(numberInWords, m_PrefixAnd, m_FirstDizaine[pUnite]);
                else
                    if (pIs10Or70Or90) numberInWords += m_FirstDizaine[pUnite];
                    else if (pDizaine == 8) numberInWords = String.Concat(numberInWords, m_Unite[pUnite]); // cas particulier 81 pas de "et"
                    else if (pDizaine > 1) numberInWords = String.Concat(numberInWords, m_PrefixAnd, m_Unite[pUnite]);
                    else numberInWords += m_Unite[1];
            }
            else if (pUnite > 1)
            {
                if (pIs10Or70Or90) numberInWords += m_FirstDizaine[pUnite];
                else numberInWords += m_Unite[pUnite];
            }

            return numberInWords;
        }
    }

}