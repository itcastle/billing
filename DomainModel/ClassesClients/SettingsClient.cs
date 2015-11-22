using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class SettingsClient
    {
        GcdbEntities _gestionDb ;
        public static void InitSettings ()
        {
            var settings = new Setting
            {
                CompanyName = "entreprise",
                Adresse = "adresse",
                PostalCode = "35000",
                RC = "11111111111",
                NF = "11111111111",
                NIS = "1111111111",
                AI = "11111111111",
                City = "Wilaya",
                Phone = "021234567",
                MobPhone = "0550123456",
                Email = "email@entreprise.dz",
                FirstTime = true,
                FactureNumber = "1",
                FactureProNumber = "1",
                Licence = "",
                PassWordApp = "demo",
             
            };


            String s = MajSettings(settings);
            
            PasswordClient passwordClient = new PasswordClient();
            Password admin = new Password
            {
                Login = "admin",
                Password1 = PasswordClient.CryptePassword("admin"),
                Status = 2
            };


            if (passwordClient.IsUserExist(admin))passwordClient.MajUser(admin);
            else passwordClient.AddUser(admin);
            
            admin = passwordClient.GetPassword("admin");
            var empc = new EmployeesManager();
            var emp = new Employee
            {
                EmployeeFirstname = "admin",
                EmployeeLastname = "admin",
                Status = 1,
                Password = admin
            };
            if (!EmployeesManager.IsEmployeeExist(emp)) empc.AddEmployee(emp);
        }

        public Setting GetSetting()
        {
             _gestionDb = new GcdbEntities();
            
            try
            {
                Setting getSettings = _gestionDb.Settings.Where(set => set.SettingsID != -1).ToList().First();
                return getSettings;
            }
            catch (Exception )
            {
                return null;
            }
        }

        public static String MajSettings(Setting settings)
        {

            try
            {

                var gestionDb = new GcdbEntities();
                var requete = from t in gestionDb.Settings
                              select t;

                if (!requete.Any())
                {

                    gestionDb.Settings.Add(settings);
                    gestionDb.SaveChanges();
                    return "Paramètres Ajouter avec succes";
                
                }
                requete.First().AI = settings.AI;// to be added fields one by one!
                gestionDb.SaveChanges();
                return "Paramètres modifier avec succes";
            }
            catch (Exception e)
            {
                return "";
            }
            
        }


        public static string ImageToString(string path)
        {

            if (path == null)

                throw new ArgumentNullException("path");

            Image im = Image.FromFile(path);

            MemoryStream ms = new MemoryStream();

            im.Save(ms, im.RawFormat);

            byte[] array = ms.ToArray();

            return Convert.ToBase64String(array);

        }

        public static Image StringToImage(string imageString)
        {

            if (imageString == null) throw new ArgumentNullException("imageString");

            byte[] array = Convert.FromBase64String(imageString);

            Image image = Image.FromStream(new MemoryStream(array));

            return image;

        }

        public static void SaySomthing(string text)
        {
            Console.WriteLine(text);
        }

        public static bool IsNotLegal()
        {
            //String CleClient = HHD_MAC_Settings.GetHDDSerialNumber("");

            //CleClient = CryptePassword(CleClient);
            //CleClient = CleClient + "ItCastle";

            //String Licence = CryptePassword(CleClient);

            //SettingsClient sc = new SettingsClient();
            //Settings settings = sc.getSetting();

            //if (Licence.Equals(settings.Licence)) return false;
            //if (IsDemo())return false;
            return false;
        }

        private static bool IsDemo()
        {
            SettingsClient settingsClient = new SettingsClient();
            Setting s = settingsClient.GetSetting();

            if (!s.Licence.Equals("demo")) return false;
            if (CalculeOrdre() < 0) return false;
            if (CalculeTemps() < 0 || CalculeTemps() > 30) return false;
            return true;
        }

        private static int CalculeTemps()
        {
            SettingsClient settingsClient = new SettingsClient();
            Setting s = settingsClient.GetSetting();
            if (s.DateFirstTime == null) return 0;
            DateTime debut = (DateTime)s.DateFirstTime;
            DateTime fin = DateTime.Now;

            TimeSpan difference = fin - debut;

            return 30 - difference.Days;
        }

        private static int CalculeOrdre()
        {
            OrderClient oc = new OrderClient();
            return 20 - oc.GetCountOrder();
        }
        public static string CryptePassword(string password)
        {

            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
