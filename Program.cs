using System.Net;
using System.Net.Mail;

string fromEmail = "hesenovfermayil765@gmail.com";
string fromPassword = "ycqt cgom dfsu ccng";

using SmtpClient smtp = new SmtpClient();
smtp.Host = "smtp.gmail.com";
smtp.Port = 587;
smtp.EnableSsl = true;


smtp.Credentials = new NetworkCredential(fromEmail,fromPassword);


using var listener = new HttpListener();
listener.Prefixes.Add("http://localhost:27001/");
listener.Start();

while (true)
{
    HttpListenerContext? context = await listener.GetContextAsync();

    _ = Task.Run(async () =>
    {
        HttpListenerRequest? request = context.Request;
        HttpListenerResponse? response = context.Response;

        using var writer = new StreamWriter(response.OutputStream);

        if (request.Url.LocalPath.Contains("/index") || request.Url.LocalPath.Contains("/index.html"))
        {
            var queryString = request.QueryString;

            if(queryString is null || queryString.Count == 0)
            {
                var text =await File.ReadAllTextAsync("/Users/fermayilhesenov/Projects/MailProtocols/Smtp_Http_Protocol/new/index.html");
                response.StatusCode =200;
                await writer.WriteAsync(text);
            }
            else
            {
                var emailTo = queryString["emailTo"];
                var subject = queryString["subject"];
                var body = queryString["body"];
                if (queryString.Count==3 && !(string.IsNullOrWhiteSpace(emailTo) ||
                    string.IsNullOrWhiteSpace(subject)|| string.IsNullOrWhiteSpace(body)))
                {
                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress(fromEmail);
                    msg.To.Add(emailTo);
                    msg.Subject = subject;
                    msg.Body = body;

                    smtp.Send(msg);
                    response.StatusCode = 200;
                    writer.Write("Message send successfullly");  ////response dir
                }
                else
                {
                    response.StatusCode = 404;
                    writer.Write("Mail dont send"); //response dir
                }
            }
        }
        else
        {
            response.StatusCode = 404;
            writer.Write("Folder is not found");
        }
    });
}