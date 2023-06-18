# Introduction
This repo is to showcase how one can use Redis. Below I will talk briefly about what Redis is, and how it is implemented within this repo.

# What Is Redis?
> Redis is an open source (BSD licensed), in-memory data structure store used as a database, cache, message broker, and streaming engine. - The [official Redis docs](https://redis.io/docs/about/)

This really sums it up quite nicely and certainly better than I could. In this repo, I will show you how you can use it as a cache.
I will not be exploring any of the other functionality of Redis, but it is certainly worth reading about!
Before we start talking about caching and how we leverage it in this repo, I think it prudent to visit a famous funny quote.
> There are only two hard things in Computer Science: cache invalidation and naming things - Phil Karlton

For our basic example, we're not actually going to be invalidating any cache (at least not _really_), but I couldn't pass up on the opportunity to use the quote!

# Caching Introduction
In this repo, we're simulating a very basic real-world scenario of a use case where Redis can help you out.
The scenario that we're simulating here is actually quite common and if you've ever done any fullstack or frontend work at all, you've almost certainly done this... populating a dropdown.
There are many different scenarios where caching makes sense, I just chose this one since it's common and fairly straightforward.

Dropdowns can come in many shapes and sizes, but really they all ultimately work in the same basic way - a list of key/value pairs. You probably have a table in your database that stores
this list of data. These types of tables are generally called "reference tables" or "lookup tables". These tables hold static data that rarely changes.
A super common one that you'll see is a list of countries and states/provinces. If you're working in a CRM, you may have a dropdown of employees or customers.
The point is, there are so many times that you use these and the data in them is almost always fairly static.

Now, when you see the words "fairly static data" your brain is hopefully jumping to caching. After all, it's a great candidate!
Things that are frequently read and rarely written are ideal things to cache. But why is that? Well, cache invalidation is hard! (see above)
Ok sometimes it isn't, but that doesn't mean you don't need to make sure you've done it!
If you have frequent writes to the data that is cached, then you constantly have to invalidate (refresh/delete/etc) that cached data.
And that really kind of undermines the entire point of the cache.
You could easily wind up spending far more processing time invalidating the cache than you would gain from having the cache to begin with!

There is a fairly large caveat that I want to get out of the way here before we dive in.
If you aren't load balancing your service, and you're not in a distributed systems architecture, and you're only using Redis for basic caching, then you don't need Redis!
In that scenario, you could very easily just use the built-in memory caching in .NET.
However, if you have your service replicated behind a load balancer, or you have many services that need access to that same data quickly, then Redis is a great candidate.

Now that we've talked a little bit about what caching is and a good scenario to use it, let's talk about what this repo does.

# Repo Demonstration
I tried to keep this repo pretty slim while still being able to articulate the benefit of Redis in a real-world scenario.
What we have here is a pretty standard setup where an ASP.NET Core WebApi has some endpoints that access some data in a database.
Some of this data is pretty static, and some of it is not.
Let's assume that this API has been scaled horizontally and is behind a load balancer.

## Database
The database that we're using in this repo has two tables in it - Persons and PersonTypes.
### Persons Table
The Persons table holds person records. This data is not very static. New people are added frequently, and it's actually quite rare that we retrieve a list of these people.

### PersonTypes Table
The PersonTypes table is a reference table that holds the types that a person record can be. In this case, I used two values - Student and Teacher.
This data is very static (rarely written) and will be used to populate a dropdown in our frontend (frequently read) so I have used Redis to cache this data.

## API
For these two tables, I have created two API controllers - PersonController and PersonTypeController.

### PersonController
The [PersonController](Demo.Redis.WebApi/Controllers/PersonController.cs) is simply for adding new person records and retrieving them. It's not the focus of the demo, so you can largely ignore it.

### PersonTypeController
The [PersonTypeController](Demo.Redis.WebApi/Controllers/PersonTypeController.cs) is more interesting for this demo.
In that controller, we have one endpoint to fetch the list of types from Redis, one endpoint to fetch the list of types from the DB, and one endpoint to add a new type.
I added two endpoints for fetching the list of data so that you can run some performance comparisons between them.
In this exact hosting scenario, there isn't much performance difference because frankly it's all running on your machine. However, the Redis endpoint is still a little bit faster.
**_In a real-world scenario, the performance difference will likely be much greater than what you see here._**
In the endpoint for creating a new type, you'll see that we first insert that new type into the database, and then we update the Redis cache with the new data.
Updating the Redis cache is paramount here since our endpoint for fetching the list from Redis (the one we would be using in production) relies solely on that.
I could have implemented a TTL (time to live) on the cached data but I wanted to keep things simple and show a way to have a "permanent cache".

# Running The Project
## Docker Compose
A Docker Compose file is included in this repo and should make things very easy to get up and running.
First make sure that you have [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed, or install Docker and Docker Compose however you'd like.

After Docker and Docker Compose are installed, ensure they are working correctly by executing the following commands:

For Docker: `docker --version`

For Docker Compose: `docker compose version`

After you have verified that Docker and Docker Compose have been installed correctly, type `docker compose up -d` to start the required services.

The API should be accessible at http://localhost:5109

## Database Setup
There is nothing that you need to do for your database setup. This project uses EF Core Migrations so your required DB objects will be created for you, if needed, when the API starts.

## Debugging
If you'd like to debug the code, start the services from above using the Docker Compose instructions.
After all services have started, run `docker compose stop api` to stop the API while keeping Postgres and Redis running.
You can then run the API in your IDE of choice and use it as you would any other API.