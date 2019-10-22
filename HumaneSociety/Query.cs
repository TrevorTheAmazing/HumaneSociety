using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            //use a delegate here?
            throw new NotImplementedException();
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            //new animal
            //check constructor method?
            //set properties
            //insertonsubmit()
            //submitchanges()
            throw new NotImplementedException();
        }

        internal static Animal GetAnimalByID(int id)
        {
            //Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();
            Animal animal = db.Animals.Where(a => a.AnimalId == id).Single();
            return animal;
            //throw new NotImplementedException();
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {            
            //do animalFromDb = null like clientFromDb = null;
            //try/catch/return to avoid the rest of the procedure if no animal found
            //set animalFromDb from animalWithUpdates
            //any foreign key info will need to be posted prior to this animal insert(new room, new mealplan, etc)
            //insertonsubmit(), then submitchanges()
            //append new animal fk info from newly inserted FK objects
            //submitchanges()
            throw new NotImplementedException();
        }

        internal static void RemoveAnimal(Animal animal)
        {            
            //deleteonsubmit
            //submitchanges
            throw new NotImplementedException();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            //recursive?
            //use UI class here?
            throw new NotImplementedException();
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            Category category = db.Categories.Where(c => c.Name == categoryName).Single();
            return category.CategoryId;
            //throw new NotImplementedException();
        }
        
        internal static Room GetRoom(int animalId)
        {
            Room room = db.Rooms.Where(r => r.AnimalId == animalId).Single();
            return room;
            //throw new NotImplementedException();
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            DietPlan dietPlan = db.DietPlans.Where(d => d.Name == dietPlanName).Single();
            
            return dietPlan.DietPlanId;
            //throw new NotImplementedException();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            //new insert to adoptions table
            //insertonsubmit()
            //submitchanges()
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            //get multiple
            //check for 'pending' status
            //List<Client> allClients = db.Clients.ToList();
            List<Adoption> allAdoptions = db.Adoptions.ToList();

            var tempPendingAdoptions = allAdoptions.Where(a => a.ApprovalStatus.Contains("pending"));

            return tempPendingAdoptions as IQueryable<Adoption>;
            //throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            //do animalFromDb = null like clientFromDb = null;
            //try/catch/return to avoid the rest of the procedure if no animal found
            //set animalFromDb from animalWithUpdates
            //any foreign key info will need to be posted prior to this animal insert(new room, new mealplan, etc)
            //insertonsubmit(), then submitchanges()
            //append new animal fk info from newly inserted FK objects
            //submitchanges()
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            //delete
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            //get multiple
            //List<Client> allClients = db.Clients.ToList();

            //return allClients;
            
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            //do animalFromDb = null like clientFromDb = null;
            //try/catch/return to avoid the rest of the procedure if no animal found
            //set animalFromDb from animalWithUpdates
            //any foreign key info will need to be posted prior to this animal insert(new room, new mealplan, etc)
            //insertonsubmit(), then submitchanges()
            //append new animal fk info from newly inserted FK objects
            //submitchanges()
            throw new NotImplementedException();
        }
    }
}