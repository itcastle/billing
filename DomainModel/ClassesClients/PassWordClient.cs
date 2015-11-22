using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class PasswordClient
    {
       GcdbEntities _gestionDb;
        public String AddUser(Password pass)
        {
            try
            {

           _gestionDb = new GcdbEntities();
            bool v = IsUserExist(pass);
            if (IsUserExist(pass))
                return "Utilisateur existe déjà";
            
            _gestionDb.Passwords.Add(pass);
            _gestionDb.SaveChanges();

            }
            catch (Exception ex)
            {

                return ex.Message;
            }
            return "Utilisateur ajouter avec succes";
        }

        public String DelUser(Password pass)
        {

            _gestionDb = new GcdbEntities();
            if (!IsUserExist(pass)) return "Utilisateur n'existante pas";
            var user = _gestionDb.Passwords.Where(pw => pw.Login.StartsWith(pass.Login));
            int i = user.Count();
            if (user.Any())
            {
                foreach (Password usr in user)
                {
                    _gestionDb.Passwords.Remove(usr);
                }
            }
            _gestionDb.SaveChanges();


            return "Utilisateur supprimer avec succes";
        }

        public String MajUser(Password pass)
        {
            _gestionDb = new GcdbEntities();

            if (!IsUserExist(pass)) return "Utilisateur n'existante pas";


            Password user = pass;
                
           
            _gestionDb.SaveChanges();

            return "Utilisateur modifier avec succes";
        }

        public String ActivatUser(Password pass)
        {
            _gestionDb = new GcdbEntities();

            if (!IsUserExist(pass)) return "Utilisateur n'existante pas";


            Password user = _gestionDb.Passwords.Single(pw => pw.Login.Equals(pass.Login));
            
            user.Status = 1;
            

            _gestionDb.SaveChanges();

            return "Utilisateur activer avec succes";
        }
        
        public String DesactivatUser(Password pass)
        {
            _gestionDb = new GcdbEntities();

            if (!IsUserExist(pass)) return "Utilisateur n'existante pas";


            Password user = _gestionDb.Passwords.Single(pw => pw.Login.Equals(pass.Login));

            user.Status = 0;


            _gestionDb.SaveChanges();

            return "Utilisateur activer avec succes";
        }

        public int GetStatus(Password pass)
        {
            _gestionDb = new GcdbEntities();

            if (!IsUserExist(pass)) return -1;


            Password user = _gestionDb.Passwords.Single(pw => pw.Login.Equals(pass.Login));

            return user.Status;
        }
        
        public bool IsUserExist(Password pass)
        {

            _gestionDb = new GcdbEntities();
     
            try
            {
                var query = _gestionDb.Passwords.Where(p => p.Login.Equals(pass.Login)).ToList();

              return query.Any();
            }
            catch (Exception)
            {
                return false;
            }
           
        
        }

        public bool IsPassWordValid(Password pass) 
        {
            _gestionDb = new GcdbEntities();
            
            bool result = false;
            try
            {
                Password password = GetPassword(pass.Login);
                if (password == null)
                {
                    //traitement user n'existe pas
                    return result;   
                }
                
                if (! password.Password1.Equals(pass.Password1)) 
                {
                    // traitement user avec faux mot de passe
                    return result;
                }
              //  if (! )
            }
            catch (Exception )
            {
                
                return result;
            }

            return true;
        }
                       
        public Password GetPassword(String login)
        {

            _gestionDb = new GcdbEntities();

            try
            {
                var user = _gestionDb.Passwords.Single(p => p.Login.Equals(login));
                return user;
            }
            catch (Exception )
            {
                return null;
            }
        }

        public static void SaySomthing(string text)
        {
            Console.WriteLine(text);
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


