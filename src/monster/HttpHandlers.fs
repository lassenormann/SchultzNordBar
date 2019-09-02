namespace monster

open System.Text.RegularExpressions

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open Giraffe
    
    let handleNotFound = 
        fun (next : HttpFunc) (ctx : HttpContext) ->
            printfn "%s" ctx.Request.Path.Value
            text ctx.Request.Path.Value next ctx


    type Question = Sum of (int*int) | Subtract of (int*int) | Power of (int*int)
    
    let (|TwoRegexGroup|_|) pattern input =
        let m = Regex.Match(input,pattern) 
        if (m.Success && m.Groups.Count = 3) then Some (m.Groups.[1].Value, m.Groups.[2].Value) else None  

    let parseQuestion str = 
        match str with
        | TwoRegexGroup "(\d+) plus (\d+)" (i1, i2) -> Some (Sum (i1 |> int, i2 |> int))                
        | TwoRegexGroup "(\d+) minus (\d+)" (i1, i2) -> Some (Subtract (i1 |> int, i2 |> int))                
        | TwoRegexGroup "what is (\d+) to the power of (\d+)" (i1, i2) -> Some (Power (i1 |> int, i2 |> int))                
        | _ -> None
    
    let answer = function
        | Sum (i1, i2) -> i1 + i2
        | Subtract (i1, i2) -> i1 - i2
        | Power (i1, i2) -> pown i1 i2
        
    let  handleParse =       
        fun (next : HttpFunc) (ctx : HttpContext) -> 
            let queryQuestion =
                match ctx.TryGetQueryStringValue "q" with
                | None   -> ""
                | Some q -> q
            printfn "Query question: %s" queryQuestion
            let answer =  
                match (parseQuestion queryQuestion) with
                | None -> "Does not parse as question"
                | Some question -> answer question |> string
            printfn "Answer: %s" answer
            (answer |> text) next ctx
            