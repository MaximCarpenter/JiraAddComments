namespace JiraUpdater

module GitCommits =
    let less (x1:int) (x2:int) =
        if x1 = -1 then x2
        elif x2 = -1 then x1
        elif x1 < x2 then x1 
        else x2

    let subStringIndexes (str:string) (pattern:string) =
        let mutable idx = str.IndexOf(pattern)
        if idx = -1 then 
            Seq.empty<string>
        else
            let mutable substring = str.[idx..] 
            let tickets = 
                [|
                    while idx >= 0
                        do       
                            let idx_top_dots = substring.IndexOf(@":")
                            let idx_top_spaces = substring.IndexOf(" ")
                            let idx_top = less idx_top_dots idx_top_spaces

                            if idx_top <> -1 then
                                yield substring.[0..idx_top - 1]
                                idx <- substring.[idx_top + 1..].IndexOf(pattern)
                                substring <- substring.[idx_top + 1 + idx..]
                            else idx <- -1            
                |]
            Seq.distinct(tickets)
        
    let getTickets path =
        let gitChanges = System.IO.File.ReadAllText path
        let tickets = subStringIndexes gitChanges "APP-"
        tickets