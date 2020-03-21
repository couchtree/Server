#### Pull latest image from Dockerhub:
`docker pull fabian986/stayathome:draft`

#### Start backend WebApi
`docker run -p 80:80 -d fabian986/stayathome:draft`

#### Build or pull latest images with docker compose (including database)
`docker-compose build`
OR
`docker-compose pull`

#### Start complete backend
`docker-compose up`