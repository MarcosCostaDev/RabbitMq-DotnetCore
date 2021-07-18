# Rabbitmq - DotnetCore

An application that uses rabbitmq message broker in aspnet core.
You will find:
- Creating of Exchange
- Retrieve message from message broker
- Remote procedure calls 


## Tecnologies used
- Docker (docker compose) - Please install [docker](https://docs.docker.com/engine/install/) in you environment for run this project.
- [Rabbitmq](https://www.rabbitmq.com/download.html)
- Swagger
- .net 5.0


# How to run
After install docker in your PC, you just need to run the solution in visual studio pressing Start (let the magic happen!). 

Rabbitmq management can be access through http://localhost:15672. 

Login: guest
password: guest


# Behaviorment
When press start in visual studio, 5 projects will be run:
- 4 Console applications 
 - PaymentCardConsumer (A listener of Queue card payment endpoint)
 - PurchaseOrderConsumer (A listener of Queue order purchase endpoint)
 - AccountsAuditConsumer (A listener of all moviment in application)
 - DirectPaymentCardConsumer (A validator of transaction that can be happened. This is called after DirectPayment Endpoint)

- 1 Web api (https://localhost:54478/swagger/index.html)




