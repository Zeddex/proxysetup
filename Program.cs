using System;
using System.Threading;
using Renci.SshNet;

namespace ssh_proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "8.8.8.8";
            string user = "root";
            string password = "root_password";

            using (var client = new SshClient(host, 22, user, password))          // Connect to server
            {
                try
                {
                    client.Connect();
                    Console.WriteLine("Connected!");

                    //Console.WriteLine("update");
                    var cmd = client.RunCommand("apt-get update -y");

                    //Console.WriteLine("install git");
                    cmd = client.RunCommand("apt-get install git -y");

                    //Console.WriteLine("clone repository");
                    cmd = client.RunCommand("git clone https://github.com/Zeddex/proxy.git");

                    //Console.WriteLine("install main script");
                    cmd = client.RunCommand("cd proxy && chmod +x proxy.sh && bash proxy.sh");

                    Console.WriteLine("reboot...");
                    try
                    {
                        cmd = client.CreateCommand("reboot");
                        cmd.Execute();
                    }
                    catch { }

                    Thread.Sleep(30000);

                    while (!client.IsConnected)                     // check server is online again
                    {
                        Thread.Sleep(1000);
                        try
                        {
                            client.Connect();
                        }
                        catch {}
                    }

                    Console.WriteLine("Connected!");
                    Console.WriteLine(client.ConnectionInfo.ServerVersion);

                    client.Disconnect();
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
            }
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
