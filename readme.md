# saywhat-echo-api

A simple API that echos back headers & querystring or parses a JWT in the `Authorization` header.

`https://saywhat.azurewebsites.net/headers` - this accepts GET or POST and returns the headers of the request

`https://saywhat.azurewebsites.net/who` - this accepts GET and returns the claims of a JWT found in the `Authorization` header, if properly formatted.

This does not validate tokens, merely parses and reads them.
