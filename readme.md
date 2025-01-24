
# FrameUp.OrderService

Project to process videos


## Environment Variables

To run this project, you will need to add the following environment variables to your .env file. This file should be located inside the deploy/ directory.
Follow the example .env.example file to create your .env

`LogBee_OrganizationId`

`LogBee_ApplicationId`

`LogBee_ApiUrl`


## Deployment

To deploy this project run with docker

Then open a bash terminal in the project root directory.
```bash
docker compose -f deploy/docker-compose.yml up -d --build
```
With all services running, follow the next step.
```
  Follow these steps to create a new service in logbee:

1 - Navigate to http://localhost:44080/Auth/Login
2 - Login using the following token eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.e30.HP79qro7bvfH7BneUy5jB9Owc_5D2UavFDulRETAl9E
2 - Navigate to http://localhost:44080/Organizations/List
3 - Click on Create application to create a new application
4 - Copy the OrganizationId, ApplicationId and ApiUrl values ​​to the variables in the .env file
```

