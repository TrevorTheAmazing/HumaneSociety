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
            if (crudOperation=="create")
            {
                InsertNewEmployee(employee);
            }

            else if (crudOperation=="read")
            {
                DisplayEmployeeInformation(employee);
            }

            else if (crudOperation=="update")
            {
                //update
                //finduser
                //updateuser
                if (UpdateEmployee(employee))
                {
                    Console.WriteLine("Employee information updated.");
                }
                
            }

            else if (crudOperation == "delete")
            {
                if (DeleteEmployee(employee))
                {
                    Console.WriteLine("Employee deleted.");
                }
            }
        }

        internal static void InsertNewEmployee(Employee employee)
        {
            Employee newEmployee = new Employee();
            
            newEmployee = employee;

            newEmployee.FirstName = UserInterface.GetStringData("first name", "the new user");
            newEmployee.LastName = UserInterface.GetStringData("last name", "the new user");
            newEmployee.UserName = UserInterface.GetStringData("username", "the new user");
            newEmployee.Password = UserInterface.GetStringData("password", "the new user");

            db.Employees.InsertOnSubmit(newEmployee);
            //db.SubmitChanges();
            //UpdateEmployee(newEmployee);
            db.SubmitChanges();
            Console.Clear();
            Console.WriteLine("New employee created.");
        }

        internal static void DisplayEmployeeInformation(Employee employee)
        {
            var employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Single();
            List<string> info = new List<string>() { employeeFromDb.EmployeeId.ToString(), employeeFromDb.FirstName, employeeFromDb.LastName, employeeFromDb.Email };
            Console.ReadLine();
        }

        internal static bool UpdateEmployee(Employee employee)
        {
            bool tempResult = false;
            Employee tempEmployee = null;            

            try
            {
                var employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Single();
                tempEmployee = employeeFromDb;
            }
            catch(InvalidOperationException)
            {
                Console.WriteLine("Could not find that employee.");
                return tempResult;
            }


            db.SubmitChanges();
            Console.Clear();
            Console.WriteLine("Employee information updated.");
            return tempResult;
        }

        internal static bool DeleteEmployee(Employee employee)
        {
            bool tempResult = false;
            Employee tempEmployee = null;
            try
            {
                var employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Single();
                tempEmployee = employeeFromDb;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Could not find that employee.");
                return tempResult;
            }

            db.Employees.DeleteOnSubmit(tempEmployee);
            db.SubmitChanges();
            return tempResult;
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            try
            {
                var tempCategory = db.Categories.Where(c => c.CategoryId == animal.CategoryId).Single();
            }
            catch(Exception)
            {
                var tempCategory = db.Categories.First();
            }            

            try
            {
                var tempDietPlan = db.DietPlans.Where(d => d.DietPlanId == animal.DietPlanId).Single();
            }
            catch(Exception)
            {
                var tempDietPlan = db.DietPlans.First();
            }

            try
            {
                var tempEmployee = db.Employees.Where(e => e.EmployeeId == animal.EmployeeId).Single();
            }
            catch(Exception)
            {
                var tempEmployee = db.Employees.First();
            }
            
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal animal = db.Animals.Where(a => a.AnimalId == id).Single();
            return animal;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animalFromDb = null;            

            try
            {
                animalFromDb = db.Animals.Where(a => a.AnimalId == animalId).Single();
            }
            catch(InvalidOperationException)
            {
                Console.WriteLine("Could not find that animal.");
                return;
            }

            foreach (var update in updates)
            {
                switch (update.Key)
                {
                    case (1):
                        {
                            animalFromDb.CategoryId = Int32.Parse(update.Value);
                            break;
                        }
                    case (2):
                        {
                            animalFromDb.Name = update.Value;
                            break;
                        }
                    case (3):
                        {
                            animalFromDb.Age = Int32.Parse(update.Value);
                            break;
                        }
                    case (4):
                        {
                            animalFromDb.Demeanor = update.Value;
                            break;
                        }
                    case (5):
                        {
                            animalFromDb.KidFriendly = (update.Value == "yes");
                            break;
                        }
                    case (6):
                        {
                            animalFromDb.PetFriendly = (update.Value == "yes");
                            break;
                        }
                    case (7):
                        {
                            animalFromDb.Weight = Int32.Parse(update.Value);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            Animal tempAnimal;
            try
            {
                var animals = db.Animals.Where(a => a.AnimalId == animal.AnimalId).Single();
                tempAnimal = animals;
            }
            catch(InvalidOperationException)
            {
                Console.WriteLine("Animal not found.");
                return;
            }

            db.Animals.DeleteOnSubmit(tempAnimal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> traits)
        {
            var tempAnimals = db.Animals.ToList();
            foreach (var trait in traits)
            {
                switch (trait.Key)
                {
                    case (1):
                        {
                            tempAnimals = tempAnimals.Where(a => a.CategoryId == Int32.Parse(trait.Value)).ToList();
                            break;
                        }
                    case (2):
                        {
                            tempAnimals = tempAnimals.Where(a => a.Name == trait.Value).ToList();
                            break;
                        }
                    case (3):
                        {
                            tempAnimals = tempAnimals.Where(a => a.Age == Int32.Parse(trait.Value)).ToList();                            
                            break;
                        }
                    case (4):
                        {
                            tempAnimals = tempAnimals.Where(a => a.Age == Int32.Parse(trait.Value)).ToList();
                            break;
                        }
                    case (5):
                        {
                            bool tempValue = (trait.Value == "yes" || trait.Value == "y");
                            tempAnimals = tempAnimals.Where(a => a.KidFriendly == tempValue).ToList();
                            break;
                        }
                    case (6):
                        {
                            bool tempValue = (trait.Value == "yes" || trait.Value == "y");
                            tempAnimals = tempAnimals.Where(a => a.PetFriendly == tempValue).ToList();
                            break;
                        }
                    case (7):
                        {
                            tempAnimals = tempAnimals.Where(a => a.Weight == Int32.Parse(trait.Value)).ToList();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            return tempAnimals as IQueryable<Animal>;
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            
            int tempInt = -1;
            try
            {
                Category category = db.Categories.Where(c => c.Name == categoryName).Single();
                tempInt = category.CategoryId;
            }
            catch(Exception)
            {
                Category tempCategory = new Category();
                tempCategory.Name = categoryName;
                db.Categories.InsertOnSubmit(tempCategory);
                db.SubmitChanges();
                tempInt = tempCategory.CategoryId;
            }

            return tempInt;
        }
        
        internal static Room GetRoom(int animalId)
        {
            Room room = db.Rooms.Where(r => r.AnimalId == animalId).Single();
            return room;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            int tempDietPlan;
            try
            {
                DietPlan dietPlan = db.DietPlans.Where(d => d.Name == dietPlanName).Single();
                tempDietPlan = dietPlan.DietPlanId;
            }
            catch(Exception)
            {
                DietPlan newDietPlan = new DietPlan();
                newDietPlan.Name = dietPlanName;
                db.DietPlans.InsertOnSubmit(newDietPlan);
                db.SubmitChanges();
                tempDietPlan = newDietPlan.DietPlanId;

            }
            
            return tempDietPlan;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            var tempAdoption = db.Adoptions.Where(a => a.AnimalId == animal.AnimalId && a.ClientId == client.ClientId).Single();
            if (tempAdoption.ApprovalStatus != null)
            {
                Console.WriteLine("This customer has already adopted this animal.");
                return;
            }
            
            Adoption newAdoption = new Adoption();
            newAdoption.ClientId = client.ClientId;
            newAdoption.AnimalId = animal.AnimalId;
            newAdoption.ApprovalStatus = "pending";
            newAdoption.AdoptionFee = 100;
            newAdoption.PaymentCollected = false;

            db.Adoptions.InsertOnSubmit(newAdoption);
            db.SubmitChanges();
            Console.Clear();
            Console.WriteLine("New adoption created.");
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            List<Adoption> allAdoptions = db.Adoptions.ToList();
            var tempPendingAdoptions = allAdoptions.Where(a => a.ApprovalStatus.Contains("pending"));
            return tempPendingAdoptions as IQueryable<Adoption>;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            Adoption adoptionFromDb = null;
            try
            {
                var tempAdoption = db.Adoptions.Where(a => a.ClientId== adoption.ClientId && a.AnimalId == adoption.AnimalId).Single();
                tempAdoption = adoptionFromDb;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Adoption record for this animal does not exist.");
                return;
            }

            adoptionFromDb.ApprovalStatus = isAdopted ? "adoption approved" : "adoption denied.";
            db.SubmitChanges();
            Console.Clear();
            Console.WriteLine("Adoption updated.");
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption tempAdoption = new Adoption();
            var removeAdoption = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).Single();
                try
                {
                tempAdoption = removeAdoption;
                }
                catch(InvalidOperationException)
                {
                Console.WriteLine("Could not find that adoption.");
                return;
                }
            
            bool success = false;
            try
            {
                db.Adoptions.DeleteOnSubmit(tempAdoption);
                db.SubmitChanges();
            }
            catch//InvalidOperationException
            {
                Console.WriteLine("RemoveAdoption failed.");
            }
            finally
            {
                if (success)
                {
                    Console.WriteLine("Adoption removed.");
                }
                else
                {
                    Console.WriteLine("RemoveAdoption failed.");
                }
            }
                
            //throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var animalShots = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            return animalShots as IQueryable<AnimalShot>;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot animalShotFromDb = null;
            try
            {
                var tempShot = db.Shots.Where(s => s.Name == shotName).Single();
                animalShotFromDb = db.AnimalShots.Where(aS => aS.ShotId == tempShot.ShotId && aS.AnimalId == animal.AnimalId).Single();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Shot record for this animal does not exist.");
                return;
            }
            animalShotFromDb.DateReceived = DateTime.Today;
            db.SubmitChanges();
            Console.Clear();
            Console.WriteLine("Shot record updated.");
        }

        internal static void InsertNewCategory(string categoryName)
        {
            Category tempCategory = new Category();
            tempCategory.Name = categoryName;
            db.Categories.InsertOnSubmit(tempCategory);
            db.SubmitChanges();
        }

        internal static void InsertNewDietPlan(string dietPlanIn)
        {
            DietPlan tempDietPlan = new DietPlan();
            tempDietPlan.Name = dietPlanIn;
            tempDietPlan.FoodType = UserInterface.GetStringData("food type", "the new diet plan");
            tempDietPlan.FoodAmountInCups = Int32.Parse(UserInterface.GetStringData("food amount measured in cups", "the new diet plan"));
            db.DietPlans.InsertOnSubmit(tempDietPlan);
            db.SubmitChanges();
        }
    }
}