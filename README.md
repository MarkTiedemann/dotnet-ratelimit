# dotnet-ratelimit

**Minimal .NET HTTP rate limiting.**

This repository is a boilerplate for as-simple-as-possible rate-limiting in a .NET HTTP server.

## Commands

```sh
# restore packages
dotnet restore

# run app locally
dotnet run

# create release build
dotnet publish -c release -o out

# build image and run container
docker build -t ratelimit .
docker run -p 5000:5000 -it --rm ratelimit
```

## Example

**Request:**

```
GET /

Authorization → Bearer xyz
```

**Response:**

```
200 OK

X-RateLimit-Remaining → 4
X-RateLimit-Reset → 11s
X-Client-ID → 1266329982
```

```
429 Too Many Requests

X-RateLimit-Remaining → 0
X-RateLimit-Reset → 9s
X-Client-ID → 1266329982
```

## License

[WTFPL](http://www.wtfpl.net/) – Do What the F*ck You Want to Public License.

Made with :heart: by [@MarkTiedemann](https://twitter.com/MarkTiedemannDE).