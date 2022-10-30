<Query Kind="Statements">
  <Connection>
    <ID>c5c58f2a-b12e-45e9-89a1-88c4a3cc4714</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\MSSQLServer1</Server>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

//Friday, September 23 Problem  //good for Q5 using multiple queries to find the solution
//One needs to have processed information from a collection to use against the same collection 
// We will need a solution to this type of problem is to use multiple queries 
// 		the early quer(ies) will produce the needed information/ criteria to execute against 
// 		the same collection in a later quer(ies)
//basically we will need to do some pre-processing 

//query 1 will generate data/information that will be used in the next query (query 2)

//Display the employees that have the most customers to support 
//Display the employee name and number of customers that employee supports 
//What is NOT wanted is a list of all employees sorted by number of customers supported 

//One could create a list of all employees, with the customer support count, ordered 
// descending by support count. BUT, this is NOT what is requested. 

//What information do I need 
// 	a. I need to know the maximum number of customers that any particular employee is supporting 
//  b. I need to take that piece of data and compare to all employees 


//a. Get a list of employees and the count of the customers each supports 
//b. From that list, I can obtain the largest number 
//c. Using the number, review all the employees and their counts, reporting ONLY the busiest employees 

var preprocessEmployeeList = Employees
								.Select(x => new
								{ 
									Name = x.FirstName + " " + x.LastName,
									CustomerCount = x.SupportRepCustomers.Count //creates list of support reps and a list of how many customers they have 	
								})
								//.Dump()
								;
var highcount = preprocessEmployeeList //use PreprocessEmploymentList because it now has the data you will need 
				.Max(x => x.CustomerCount)
				.Dump()	//value is 20
				;
var BusyEmployees = preprocessEmployeeList
				.Where (x => x.CustomerCount == highcount)
				.Dump()
				;

var busyEmployees = preprocessEmployeeList
				.Where(x => x.CustomerCount == preprocessEmployeeList.Max(x => x.CustomerCount)) //to shorten the code 
				.Dump()
				;




