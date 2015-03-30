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
        private Queue<int> grayList = new Queue<int>();
        private int timeLiveInGrayList = 10000;

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
                AddReplicaToGrayList();
            }

            return null;
        }

        private void AddReplicaToGrayList()
        {
            lock (grayList)
            {
                grayList.Enqueue(index);
                new Task(DeliteIpInGrayList);
            }
        }

        private void DeliteIpInGrayList()
        {
            Thread.Sleep(timeLiveInGrayList);
            lock (grayList)
            {
                if (grayList.Count > 0)
                    grayList.Dequeue();
            }
        }
        
        private string GetIp()
        {
            lock (ips)
            {
                var i = 0;
                do
                {
                    index = (index + 1)%ips.Length;
                } while (grayList.Contains(index) && i++ < ips.Length);
                
                lock (grayList)
                {
                    if (i <= ips.Length || !grayList.Any())
                        return ips[index];

                    return ips[grayList.Dequeue()];
                }
            }
        }
    }
}
