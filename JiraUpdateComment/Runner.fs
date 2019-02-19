namespace JiraUpdater

module Runner =
    open System

    let readAndPost(url, token, version, jenkinsPath)   =
        let tickets = (GitCommits.getTickets jenkinsPath)
        let comment = "Please test in APP version " + version
        let mutable currentTicket = "";
        try
            for ticket in tickets do
                currentTicket <- ticket
                JiraSender.postComment(url, token, ticket, comment)
                Console.WriteLine(String.Format("Succesfully set comment to ticket: {0}", currentTicket));
            0
        with
        | :? System.Net.WebException -> Console.WriteLine(String.Format("Timeout. Failed to set comment to ticket: {0}", currentTicket)); 0 // hide exception, because I fed up with timeouts        

    let run (argv:string[]) = 
        let url = argv.[0]
        let token = argv.[1]
        let version = argv.[2]
        let jenkinsPath = argv.[3]
        readAndPost(url, token, version, jenkinsPath)
        0