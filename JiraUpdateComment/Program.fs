open System.Net
open System.Text

   
module JiraSender =
    let authorization (token) = 
        "Basic " + token

    let url (jira, issue) = 
        jira + "/issue/" + issue + "/comment"

    let body (comment) = 
        "{\"body\": \"" + comment + "\"} "
    
    let postComment(jiraUrl:string, token, issue, comment) = 
        System.Net.ServicePointManager.ServerCertificateValidationCallback <- (fun sender certificate chain sslPolicyErrors -> true)
        ServicePointManager.SecurityProtocol <- SecurityProtocolType.Tls12//(SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3)
        let httpWebRequest = WebRequest.Create(url(jiraUrl, issue)) :?> HttpWebRequest
        httpWebRequest.Method <- "POST"
        httpWebRequest.ContentType <- "application/json"
        httpWebRequest.Accept <- "application/json"
        httpWebRequest.Headers.Add("Authorization", authorization(token))
        
        use streamWriter = httpWebRequest.GetRequestStream()
        let encoding = new ASCIIEncoding()
        let body = encoding.GetBytes(body comment)
        streamWriter.Write(body, 0, body.Length)
        streamWriter.Flush()
        streamWriter.Close()
        
        let httpResponse = httpWebRequest.GetResponse()
        //use streamReader = new StreamReader(httpResponse.GetResponseStream())      
        //streamReader.ReadToEnd()
        0        

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

module Runner =
    let run(url, token, version, jenkinsPath) =
        let tickets = (GitCommits.getTickets jenkinsPath)
        let comment = "Please test in APP version " + version
        for ticket in tickets do
            JiraSender.postComment(url, token, ticket, comment)
        0

    let silentRun (argv:string[]) = 
        let url = argv.[0]
        let token = argv.[1]
        let version = argv.[2]
        let jenkinsPath = argv.[3]
        run(url, token, version, jenkinsPath)
        0

[<EntryPoint>]
let main argv =  
    Runner.silentRun(argv)
    0
