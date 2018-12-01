namespace JiraUpdater

module Runner =
    let readAndPost(url, token, version, jenkinsPath) =
        let tickets = (GitCommits.getTickets jenkinsPath)
        let comment = "Please test in APP version " + version
        for ticket in tickets do
            JiraSender.postComment(url, token, ticket, comment)
        0

    let run (argv:string[]) = 
        let url = argv.[0]
        let token = argv.[1]
        let version = argv.[2]
        let jenkinsPath = argv.[3]
        readAndPost(url, token, version, jenkinsPath)
        0