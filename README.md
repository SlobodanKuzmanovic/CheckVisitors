# Check active visitors

API - RabbitMQ - MongoDB

## Getting Started

Before starting You need to have installed MongoDB and Docker. 

### Prerequisites

Import data into your local MongoDB. Find files in the root file of the project (reports_data.zip).

Inside file **docker-compose.yml** you can find settings for RabbitMQ: **RABBITMQ_DEFAULT_USER** and **RABBITMQ_DEFAULT_PASS**. If you already have a RabbitMQ up and running change property values to match your settings in that **yml** file and inside **Helper.cs** file in **Common** ClassLibrary.
```csharp
public static ConnectionFactory CreateFactory()
{
    return new ConnectionFactory()
    {
        HostName = "localhost",
        UserName = "root",
        Password = "password",
        VirtualHost = "/"
    };
}
```
Run cmd (or GitBash) in the root folder of the project and run the command "**docker-compose up**" - That will start a RabbitMQ on your docker.

Change the connection string to match your MongoDB connection. Inside the file **SightingsDBAccess** in **DBAccessLayer** ClassLibrary you will find
```csharp
public SightingsDBAccess()
{
    connectionString = "mongodb://localhost:27017";
    databaseName = "reports_data_sightings";
    collectionName = "reports_data_sightings";
}
```
Change to match your connection, database name, and collection name.

Make sure that you have selected **Multiple startup projects** in properties for your solution.

Right-click on your solution and select **Configure Startup Projects**
set **ActiveVisitors.API** and **Sightings.Server** to action "Start";
![image](https://github.com/SlobodanKuzmanovic/CheckVisitors/assets/17620480/47bb2659-1a0f-40c6-b946-f004b5cf7a98)

Starting a project will automatically start Swagger where you can call an API to read data.
The required parameter is **date**, camerraIds is optional.

