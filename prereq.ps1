docker run -p 15672:15672 -p 5672:5672 -v ./data-rabbit:/data rabbitmq:3-management
docker run -p 27017:27017 -v ./dataMongo:/data/db mongo
docker run --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest