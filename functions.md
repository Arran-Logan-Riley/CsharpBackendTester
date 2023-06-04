**TO RUN**

CMD: dotnet run

**To DELETE**:

http://localhost:5000/delete
Set api method to DELETE
Pass in raw **Firstname** and **Lastname**

{
    "FirstName": "John",
    "LastName": "Doe"
}

**TO POST**

http://localhost:5000/post

Set api method to POST

Put object in raw section in postman

{
    "FirstName": "Maximus",
    "LastName": "Thunderstrike",
    "Age": 35,
    "Address": {
        "Street": "456 Lightning Road",
        "City": "Thunderbolt City",
        "Country": "Stormland"
    }
}

**TO GET**

http://localhost:5000/get

Set api method to GET

will return data JSON obj as txt

**PUT**

Set api method to PUT

http://localhost:5000/put

Paste new object you want to change into the raw section

{
    "FirstName": "John",
    "LastName": "Doe",
    "Age": 35,
    "Address": {
        "Street": "789 Oak St",
        "City": "Villageland",
        "Country": "Countryland"
    }
}