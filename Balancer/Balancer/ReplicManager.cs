using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Balancer
{
    class ReplicManager
    {
        private string[] ips;
        private int index;

        public ReplicManager(string filename)
        {
            ips = File.ReadAllLines(filename);
        }

        public WebResponse GetResponse(string query, int time)
        {
            var timeForReplic = time/ips.Length + 1;
            for(; time > 0; time -= timeForReplic)
            {
                var url = "http://" + GetIp() + "/method?query=" + query;
                var task = new Task<WebResponse>(() => WebRequest.Create(url).GetResponse());
                var timer = new Task(() => Thread.Sleep(timeForReplic));
                task.Start();
                timer.Start();

                Task.WaitAny(task, timer);
                if (task.IsCompleted)
                    return task.Result;
                
                try{
                    task.Dispose();   //Если задача завершится во время перехода, возникнет исключение,
                }catch(Exception e){} //Но ждать пока система свернет этот обьект - еще хуже
            }

            return null;
        }
        
        private string GetIp()
        {
            lock (ips)
            {
                index = (index + 1)%ips.Length;
                return ips[index];
            }
        }
    }
}
