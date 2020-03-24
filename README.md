# backend for couchtreeapp  
## Get it running
### Pull latest images from Dockerhub:
`docker-compose pull`
### Start backend    
`docker-compose pull`
OR if you just want to start the REST-Api itself  
`docker run -p 80:5000 -d couchtreeapp/backend:latest` 
## How to use it  
When backend is started port 5000 is exposed to be used for HTTP requests.  
Keep in mind: For real server deployment, a proxy is in front of our app. So you have to use port 443 (HTTPS) for your requests

### Base URL:
https://creative-two.com/api/v1  
http://localhost:5000/api/v1  

### HTTP Headers
```Content-type: application/json```  
### API description
#### Add and update players location  
Url: ```GET  player/<player id>/location```  
Body:
```
{  
	"Lat": 1.130000,  
	"Long": 1.100002,  
	"at_home": true,  
	"tracked":  true  
}
```  
Response: 
```
{
    "nearby_players": [
        {
            "dir": 6,
            "dist": 83.23,
            "vel_nearing": 3
        }
    ]
}
```  
\<dir\>: general direction of other player Enum: [0:"n",1:"ne",2:"e",3:"se",4:"s",5:"sw",6:"w",7:"nw"]  
\<dist\>: distance in meters to a nearby player  
\<vel_nearing\>: speed in km/h with which a nearby player is closing in  
