This is experimental project including folowing features:
- Blazor Client/WASM and Blazor Server (Capable to swith between these runitmes)
- API-less integration (mediator/CQRS pattern)
- Redux integration (Fluxer nuget package)
- Local storage persistence in browser
- Authentication (Cookies or JWT)
- Configuration (with appsetting.json stored on server)
- Logging
- Localization (with resx files)
- Fluent validator (shared validator on server and client)

Not yet fully implemented features
- Bootstrap integration
- Client side Debugging


# VS Code usage
## Start application in watch mode (Server Side Execution)
`cd App.Server/ && dotnet watch run -c Debug_SSE`

## Start application in watch mode (Client Side Execution / WASM)
`cd App.Server/ && dotnet watch run -c Debug_CSE`

## Build CSS and watch for changes
Install VS codee ExtensionLive Sass Compiler

Press F1 and type `Live Sass: Watch Sass`
