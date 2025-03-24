# Rent A Car API

## Overview

The Rent A Car API is a RESTful web service designed to facilitate the management of car rentals. It provides endpoints for user authentication, vehicle management, rental processing, and coupon handling. The API is built using C# and ASP.NET Core, ensuring a robust and scalable solution for car rental services.

## Features
- User authentication and authorization using JWT
- Vehicle management (CRUD operations)
- Rental processing (creating and managing rentals)
- Coupon creation and redemption
- Error handling and standardized API responses
- IP-Based rate limiting
- Caching most used endpoints

## Technologies Used
- **C#**: The primary programming language used for the API.
- **ASP.NET Core**: The framework used to build the web service.
- **Entity Framework Core**: For database interactions and ORM (Object-Relational Mapping).
- **JWT (JSON Web Tokens)**: For user authentication and authorization.
- **Docker**: For development environment containerization.
- **PostgreSQL**: As the database management system.
- **Redis**: For caching frequently accessed data.
- **Swagger**: For API documentation and testing.


## Getting Started

To get started with the Rent A Car API, you'll need to set up your development environment using Docker. Follow the steps below to get everything up and running.

### Prerequisites

Before you begin, ensure you have the following installed on your machine:

- [Git](https://git-scm.com/downloads)
- [Docker](https://www.docker.com/get-started)

### Step 1: Clone the Repository

First, clone the Rent A Car API repository from GitHub:

```bash
git clone https://github.com/steveljko/rent-a-car-api.git
cd rent-a-car-api
```

### Step 2. Build and Run the Docker Containers

```bash
docker-compose up --build
```

### Step 3. Access the API

After the containers are up and running, you can access the Rent A Car API at the following URL:
```
http://localhost:8080/api/v1
```

You can also access the Swagger documentation for the API at:
```
http://localhost:8080
```
