### Build Docker Image
This builds an image called `openhistorian` from the local [`Dockerfile`](./Dockerfile) script:
```sh
docker build --no-cache -t openhistorian .
```

### Deploy and Run Docker Image to a Container
This deploys and runs docker image `openhistorian` to a container called `openhistorian-test` and exposes container ports to local machine:
```sh
docker run -d --name openhistorian-test -p 8180:8180 -p 7175:7175 -p 6175:6175 openhistorian
```

### Tag Docker Image
This tags the `openhistorian` docker image as `gridprotectionalliance/openhistorian:v2.8.421`, creating a copy of the image that can be pushed to [dockerhub](https://hub.docker.com/repository/docker/gridprotectionalliance/openhistorian/general):
```sh
docker tag openhistorian gridprotectionalliance/openhistorian:v2.8.421
```

### Push Tagged Docker Image to dockerhub
This pushes the tagged image `gridprotectionalliance/openhistorian:v2.8.421` to dockerhub:
```sh
docker push gridprotectionalliance/openhistorian:v2.8.421
```

### Open Shell Session Into Running Container
This connects a shell session, like SSH, to to the running container called `openhistorian-test`:
```sh
docker exec -it openhistorian-test /bin/bash
```

### Run openHistorian Console from Container Shell Session
This runs the openHistorian Console application from the container shell session, credentials will need to be provided (see below):
```sh
mono openHistorianConsole.exe
```

### Open Web Interface
The following URL will open the openHistorian web management interface for a container listening on 8180, credentials will need to be provided (see below):

http://localhost:8180/


### Default Credentials
* username: `.\admin`
* password: `admin`

Note: On web page, enter `.\admin` as user name, the `.` means use local domain. When authenticating in console, you can just use `admin` (local domain assumed).
