using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();

        Console.WriteLine("Listening for requests...");

        //Define a list of people 
        var people = new List<Person>();


        // Add multiple people with different addresses
        people.Add(new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30,
            Address = new Address
            {
                Street = "123 Main St",
                City = "Cityville",
                Country = "Countryland"
            }
        });

        people.Add(new Person
        {
            FirstName = "Alice",
            LastName = "Smith",
            Age = 25,
            Address = new Address
            {
                Street = "456 Elm St",
                City = "Townsville",
                Country = "Countryland"
            }
        });

        while (true)
        {
            var context = await listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            //GET
            if (request.Url.AbsolutePath == "/get" && request.HttpMethod == "GET")
            {
                var responseObj = new
                {
                    People = people
                };

                var jsonResponse = JsonSerializer.Serialize(responseObj);
                var buffer = Encoding.UTF8.GetBytes(jsonResponse);

                response.ContentType = "application/json";
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            //POST method
            else if(request.Url.AbsolutePath == "/post" && request.HttpMethod == "POST"){
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding)){
                    var requestBody = await reader.ReadToEndAsync();
                    var newPerson = JsonSerializer.Deserialize<Person>(requestBody);

                    if(newPerson != null){
                        people.Add(newPerson);
                        response.StatusCode = (int)HttpStatusCode.Created;
                    }
                }
                //DELETE method
            }else if(request.Url.AbsolutePath == "/delete" && request.HttpMethod == "DELETE"){
                Console.WriteLine("Delete activated");
                //Init the stream reader
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding)){
                    //set the request body so an input can be taken.
                    var requestBody = await reader.ReadToEndAsync();
                    //Deserialize the input into an identifier to use to delete a person
                    var personToDelete = JsonSerializer.Deserialize<Person>(requestBody);

                    if(personToDelete != null){
                        var removed = people.RemoveAll(p => p.FirstName == personToDelete.FirstName && p.LastName == personToDelete.LastName);

                        if(removed > 0){
                            response.StatusCode = (int)HttpStatusCode.NoContent;
                        }else{
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }
                }
            }else if(request.Url.AbsolutePath == "/put" && request.HttpMethod == "PUT"){
                Console.WriteLine("PUT has been activated");
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding)){
                    var requestBody = await reader.ReadToEndAsync();
                    //Take in the person to replace as a JSON 
                    var personToReplace = JsonSerializer.Deserialize<Person>(requestBody);
                    if(personToReplace != null){
                        var existingPerson = people.FirstOrDefault(p => p.FirstName == personToReplace.FirstName && p.LastName == personToReplace.LastName);
                        //Check if existingPerson was successful
                        if(existingPerson != null){
                            existingPerson.FirstName = personToReplace.FirstName;
                            existingPerson.LastName = personToReplace.LastName;
                            existingPerson.Age = personToReplace.Age;
                            existingPerson.Address = personToReplace.Address;
                            response.StatusCode = (int)HttpStatusCode.OK;
                        }
                    }
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            response.Close();
        }
    }
}

class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }
}

class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}
