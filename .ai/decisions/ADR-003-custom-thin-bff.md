# ADR-003: Custom Thin BFF with YARP

## Status: Accepted

## Context
The Angular SPA must talk only to a BFF, and the BFF must communicate with the API over
HTTP. The assessment requires a clear frontend/backend boundary without shared backend
project references from the frontend side.

## Decision
Implement a custom ASP.NET Core BFF that uses YARP to proxy `/api/*` requests to the API.
The BFF will also own antiforgery setup, development CORS, health checks, auth extension
points, and production static file serving for the Angular build.

## Rationale
- Keeps Angular isolated from the API host and port
- Avoids a third-party BFF framework while preserving the BFF pattern
- Keeps auth changes concentrated in `Program.cs` when authentication is added later
- Uses standard ASP.NET Core middleware and YARP only
- Maintains the required HTTP boundary between frontend and backend projects

## Trade-offs Accepted
- The BFF initially contains little business-specific code
- Local development needs three running processes: API, BFF, and Angular
- Production static file serving needs the Angular build output to be copied into the BFF host

## Future Path
When authentication is required, add cookie/OIDC authentication, expose login/logout/user
endpoints, enable authorization middleware, and require authorization on the YARP route.
No backend API or Angular service boundary changes should be needed.
