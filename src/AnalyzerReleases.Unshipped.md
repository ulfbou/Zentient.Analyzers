; Unshipped analyzer release
; https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md
### Unshipped
ZNT0001A Title:Result types must compute success from Errors (no settable IsSuccess) Category:Zentient.ResultDoctrine Severity:Error EnabledByDefault:True Description:Zentient doctrine: IsSuccess is a derived property computed from Errors; implementations must not expose a setter.
ZNT0001B Title:Result types must compute success from Errors (no settable Messages) Category:Zentient.ResultDoctrine Severity:Error EnabledByDefault:True Description:Zentient doctrine: Messages is a derived property computed from Errors; implementations must not expose a setter.
ZNT0001C Title:Result types must compute success from Errors (no settable Errors) Category:Zentient.ResultDoctrine Severity:Error EnabledByDefault:True Description:Zentient doctrine: Errors is a derived property computed from Errors; implementations must not expose a setter.
ZNT0001D Title:Result types must compute success from ErrorMessage (no settable ErrorMessage) Category:Zentient.ResultDoctrine Severity:Error EnabledByDefault:True Description:Zentient doctrine: ErrorMessage is a derived property computed from ErrorMessage; implementations must not expose a setter.
ZNT0001E Title:Result types must expose readable Messages Category:Zentient.ResultDoctrine Severity:Error EnabledByDefault:True Description:Zentient doctrine requires a readable Messages collection on all Result types.
ZNT0001F Title:Result types must expose readable Errors Category:Zentient.ResultDoctrine Severity:Error EnabledByDefault:True Description:Zentient doctrine requires a readable Errors sequence on all Result types.
ZNT0001G Title:IResult<T> types must expose readable Value Category:Zentient.ResultDoctrine Severity:Error EnabledByDefault:True Description:Zentient doctrine requires a readable Value on generic Result types.
