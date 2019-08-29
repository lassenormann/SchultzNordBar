namespace monster

open System.Text.RegularExpressions

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Giraffe
    open monster.Models

    let handleGetHello' =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = "Hello world, from Giraffe!"
                }
                return! json response next ctx
            }
    
    let handleGetHello =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            let response = {
                Text = "Hello world, from Giraffe!"
            }
            json response next ctx

    let (|FirstRegexGroup|_|) pattern input =
        let m = Regex.Match(input,pattern) 
        if (m.Success) then Some m.Groups.[1].Value else None  

    let testRegex str = 
        match str with
        | FirstRegexGroup "http://(.*?)/(.*)" host -> 
               sprintf "The value is a url and the host is %s" host
        | FirstRegexGroup ".*?@(.*)" host -> 
               sprintf "The value is an email and the host is %s" host
        | _ -> sprintf "The value '%s' is something else" str
    
    let  handleParse str = 
        fun (next : HttpFunc) (ctx : HttpContext) -> 
            (testRegex str |> text) next ctx
            