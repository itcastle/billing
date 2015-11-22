using System;
using System.Collections.Generic;
using System.Linq;
using GestionCommerciale.DomainModel.Entities;

namespace GestionCommerciale.DomainModel.ClassesClients
{
    public class EmployeesManager
    {
        private GcdbEntities _gestionDb;
        public List<Employee> GetEmployees()
        {
            try
            {
                _gestionDb = new GcdbEntities();

                return _gestionDb.Employees.Where(c => c.Status == 0).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public String DesactivateEmployee(Employee emp)
        {

            _gestionDb = new GcdbEntities();

            emp.Status = 1;
            MajEmployee(emp);

            return "Employee désactiver avec succes";
        }


        public Employee GetEmployeeByName(String name)  // s'il ya plusieurs nom alors il faut modifier la valeur de retour par list et signle par where
        {
            try
            {
                _gestionDb = new GcdbEntities();
                return _gestionDb.Employees.Single(c => c.EmployeeFirstname.Equals(name));
            }
            catch (Exception)
            {
                return null;
            }

        }

        public Employee GetEmployeeById(int id)
        {
            try
            {
                _gestionDb = new GcdbEntities();
                return _gestionDb.Employees.Single(c => c.EmployeeID == id);
            }
            catch (Exception)
            {

                return null;
            }
        }

        public Employee GetEmployeeByUserId(Password password)
        {
            try
            {
                _gestionDb = new GcdbEntities();
                Employee emp =  _gestionDb.Employees.Single(c => c.Password.Login == password.Login);
                return emp;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public String AddEmployee(Employee emp)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                if (IsEmployeeExist(emp)) return "Employee existe déjà";

                _gestionDb.Employees.Add(emp);
                _gestionDb.SaveChanges();

                return "Employee ajouter avec succes";
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public String DelEmployee(Employee emp)
        {

            try
            {
                _gestionDb = new GcdbEntities();
                if (!IsEmployeeExist(emp)) return "Employee n'existante pas";
            
                var employee = _gestionDb.Employees.Single(em => em.EmployeeID.Equals(emp.EmployeeID));
                _gestionDb.Employees.Remove(employee);
                _gestionDb.SaveChanges();


                return "Employee supprimer avec succes";
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public String MajEmployee(Employee emp)
        {
            _gestionDb = new GcdbEntities();

            if (!IsEmployeeExist(emp)) return "Employee n'existante pas";

            Employee employee = _gestionDb.Employees.Single(e => e.EmployeeID == emp.EmployeeID);
            try
            {
                employee = emp;  // si ca va pas marcher alors attache dettache sinon remplissage champ par champ
                _gestionDb.SaveChanges();
            }
            catch (Exception )
            {
                return "erreur";
            }

            return "Employee modifier avec succes";
        }

        public static bool IsEmployeeExist(Employee emp)
        {

            GcdbEntities gestionDb = new GcdbEntities();

           
            try
            {
                var user = gestionDb.Employees.Where
                    (p =>
                         (p.EmployeeFirstname.ToLower().Equals(emp.EmployeeFirstname))
                     ).ToList();

                return user.Any();
            }
            catch (Exception )
            {

                return false;
            }


         
        }

        public static void saySomthing(string text)
        {
            Console.WriteLine(text);
        }

    }

}


