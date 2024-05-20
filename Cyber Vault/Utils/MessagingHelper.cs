using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;

namespace Cyber_Vault.Utils;

internal class MessagingHelper
{
    private static HttpListener? _listener;

    public static void StartListening()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:3000/");
        _listener.Start();
        _listener.BeginGetContext(new AsyncCallback(ProcessRequest), null);

        Console.ReadLine();
    }

    public static void ProcessRequest(IAsyncResult result)
    {
        var context = _listener!.EndGetContext(result);
        var request = context.Request;

        //Answer getCommand/get post data/do whatever

        string postData;
        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            postData = reader.ReadToEnd();
            var json = JsonConvert.DeserializeObject(postData);
            Debug.WriteLine(json);
        }

        _listener.BeginGetContext(new AsyncCallback(ProcessRequest), null);
    }

}
