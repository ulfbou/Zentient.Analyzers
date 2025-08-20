; Unshipped analyzer release
; https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md
### Unshipped
ZNT0001 Title:Result types must compute success from Errors (no settable IsSuccess) Category:Zentient.ResultDoctrine Severity:Error EnabledByDefault:True Description:Zentient doctrine: IsSuccess is a derived property computed from Errors; implementations must not expose a setter.
ZNT0001A Title:Result types must expose readable Messages Category:Zentient.ResultDoctrine Severity:Error EnabledByDefault:True Description:Zentient doctrine requires a readable Messages collection on all Result types.
ZNT0001B Title:Result types must expose readable Errors Category:Zentient.ResultDoctrine Severity:Error EnabledByDefault:True Description:Zentient doctrine requires a readable Errors sequence on all Result types.
ZNT0001C Title:IResult<T> types must expose readable Value Category:Zentient.ResultDoctrine Severity:Error EnabledByDefault:True Description:Zentient doctrine requires a readable Value on generic Result types.
