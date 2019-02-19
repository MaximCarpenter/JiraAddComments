namespace JiraUpdater

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
        ServicePointManager.SecurityProtocol <- (SecurityProtocolType.Tls ||| SecurityProtocolType.Tls11 ||| SecurityProtocolType.Tls12 ||| SecurityProtocolType.Ssl3)
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


