using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace Balancer
{
    class Program
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("Balanser#Suhoy");
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static readonly int port = 20002;
        
        private static readonly ReplicManager manager = new ReplicManager("ips.txt");
        
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            try
            {
                var listener = new Listener(port, "method", CatchRequest);
                listener.Start();

                log.InfoFormat("Balancer started on port: {0}!", port);
                new ManualResetEvent(false).WaitOne();
            }
            catch (Exception e)
            {
                log.Fatal(e);
                throw;
            }
        }

        private static async Task CatchRequest(HttpListenerContext context)
        {
            var requestId = Guid.NewGuid();
            var query = context.Request.QueryString["query"];
            var remoteEndPoint = context.Request.RemoteEndPoint;
            log.InfoFormat("{0}: received {1} from {2}", requestId, query, remoteEndPoint);
            context.Request.InputStream.Close();


            var response = manager.GetResponse(query, 10000);
            if (response != null)
            {
                if (ResponeMightBeDeflate(context.Request.Headers))
                {
                    context.Response.Headers.Add(HttpResponseHeader.ContentEncoding, "deflate");
                    CopyToWithDeflate(context.Response.OutputStream, response.GetResponseStream());
                }
                else
                    await response.GetResponseStream().CopyToAsync(context.Response.OutputStream);
            }
            else
                context.Response.StatusCode = 500;

            context.Response.OutputStream.Close();
            log.InfoFormat("{0}: response sent back to {1} with code {2}",
                            requestId, remoteEndPoint, context.Response.StatusCode);
        }

        private static bool ResponeMightBeDeflate(NameValueCollection headers)
        {
            return headers.AllKeys.Contains("Accept-Encoding") && headers["Accept-Encoding"].IndexOf("deflate") >= 0;
        }

        private static async Task CopyToWithDeflate(Stream responseStream, Stream dataStream)
        {
            using (var compressionStream = new DeflateStream(responseStream, CompressionMode.Compress))
            {
                await dataStream.CopyToAsync(compressionStream);
            }
        }
    }
}
