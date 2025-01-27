# FrameUp.OrderService
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Team-One-Pos-Tech_FrameUp.OrderService&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Team-One-Pos-Tech_FrameUp.OrderService)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Team-One-Pos-Tech_FrameUp.OrderService&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Team-One-Pos-Tech_FrameUp.OrderService)

## Overview
FrameUp.OrderService is a comprehensive project designed to handle the processing of video orders efficiently and effectively. The project leverages the latest technologies and adheres to best practices to ensure high performance, scalability, and maintainability.

## Getting Started

### Technologies

- .NET 9.0
- PostgresSQL
- SpecFlow
- NUnit
- RabbitMQ
- MinIO
- LogBee

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/Team-One-Pos-Tech/FrameUp.OrderService.git
    ```

2. Install dependencies:
    ```sh
    dotnet restore
    ```

### Running just the Application

To run the application locally, use the following command:
```sh
cd /src/FrameUp.OrderService.Api
dotnet run
```

### Running with Docker Compose - Recommended
Docker-Compose with all services running, follow the next step.

Open a bash terminal in the project root directory.

```bash
docker compose -f deploy/docker-compose.yml up -d --build
```

### Running Tests

To run the tests, use the following command:
Ensure you are running Docker locally.
```sh
dotnet test
```

Note: For running integration tests ensure your local docker is running. 

# LogBee Integration

## Environment Variables

To run this project, you will need to add the following environment variables to your .env file. This file should be located inside the deploy/ directory.
Follow the example .env.example file to create your .env

`LogBee_OrganizationId`

`LogBee_ApplicationId`

`LogBee_ApiUrl`


## Deployment

With all services running, follow the next step.
```
  Follow these steps to create a new service in logbee:

1 - Navigate to http://localhost:44080/Auth/Login
2 - Login using the following token eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.e30.HP79qro7bvfH7BneUy5jB9Owc_5D2UavFDulRETAl9E
2 - Navigate to http://localhost:44080/Organizations/List
3 - Click on Create application to create a new application
4 - Copy the OrganizationId, ApplicationId and ApiUrl values ​​to the variables in the .env file
```

---

# Minio Local Development Integration
Used to simulate the S3 bucket locally

1. Access the minio dashboard at https://play.min.io
2. Login with username: `minioadmin`, password: `minioadmin`
3. Create Access key.
4. Fill the docker compose app variables, Example:
```
   Settings__MinIO__AccessKey: 8F2QSn6rpRAjkZOneNBS
   Settings__MinIO__SecretKey: F3EH8vrVCeNteBMheqmB8bICzujhPczrrrBusLi1
```


---
## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.
