### Get it running
#### Pull latest image from Dockerhub:
`docker pull couchtreeapp/backend:latest`
#### Start backend WebApi
`docker run -p 80:5000 -d couchtreeapp/backend:latest`
#### Build or pull latest images with docker compose (including database)
`docker-compose build`
OR
`docker-compose pull`
#### Start complete backend including database and reverse proxy
`docker-compose up`

### How to use it
When backend is started port 5000 is exposed to be used for HTTP requests.  
Keep in mind: For real server deployment, a proxy is in front of our app. So you have to use port 443 (HTTPS) for your requests

#### Base URL:
https://creative-two.com
http://localhost:5000

