# CityGuide API

ASP.NET Framework 4.8 Web API for the bulletin board assignment.
Data is saved in json files under App_Data.

To run: open CityGuide.slnx in VS and run

Users for testing:
username : shaul / password : admin
username : carmel / password : 123456

Login returns a JWT token. Create / update / delete / upload image need the token
(Authorization: Bearer ...).

Nearby ads - GetNearbyAdvertisements?latitude=..&longitude=.. returns ads in 3 km.

Login wasn't required in the assignment, I added it so users can only edit/delete their own ads.
Passwords are just base64 encoded, not hashed - kept it simple since it's only for ownership.
