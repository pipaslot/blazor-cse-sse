This is experimental project including folowing features:
- Blazor Client/WASM
- API-less integration (mediator/CQRS pattern)
- Redux integration (Fluxer nuget package)
- Local storage persistence in browser
- Authentication via JWT
- Configuration (with appsetting.json stored on server)
- Logging
- Localization (with resx files)
- Fluent validator (shared validator on server and client)

Not yet fully implemented features
- Bootstrap integration


# VS Code usage

## Start application in watch mode 
`cd App.Server/ && dotnet watch run -c Debug_CSE`

## Build CSS and watch for changes
Install VS codee ExtensionLive Sass Compiler

Press F1 and type `Live Sass: Watch Sass`
