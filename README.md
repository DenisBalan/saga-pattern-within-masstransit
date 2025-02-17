# saga-pattern-within-masstransit
Getting started with Saga Pattern within MassTransit in a document generation example

<!---
![image](https://github.com/user-attachments/assets/202d2138-bd35-4e60-8d9c-052f2c7f6eaa)
-->
![image](https://github.com/user-attachments/assets/ccb2a808-dc85-4076-8cff-2966fcc4168d)
![image](https://github.com/user-attachments/assets/09874425-e7c5-44e7-bded-955256f9e759)


hands on: https://www.youtube.com/watch?v=Vsnz7np84Vc

# Context
1. We are developing a distributed document generation solution.
2. The application must ensure that each document is successfully generated, or else the entire set of documents is marked as erroneous.
3. Since data collection, generation, uploading, etc. are in different micro-services, the application must use a solution to orchestrate the entire workflow

> Bonus, using saga on sagas
# Saga on Sagas
It's possible to implement Saga on Saga via invoking child Saga initialization message from parent one, while parent is in a certain state until child will not finish.

## Objective
To build a system that is
- scalable
- always reporting on the status of document generation
- be fault tolerance
- consistent - changes must be atomic
