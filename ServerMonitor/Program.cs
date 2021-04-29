/******************************************************
Windows Process Monitoring Tool
- CPU/RAM/Running Status Monitoring, kill and restart process 
C# 
  
 *
 Author Shuo Ding 
 * Copyright ©2021   All Right Reserved 
 * */

 

using System;
using System.Collections.Generic;
using System.Text;
using ServerMonitor.ProcessManager;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using System.Runtime.InteropServices;
using System.Linq;
using System.Threading.Tasks;


namespace ServerMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessMonitor proc = new ProcessMonitor();
            Console.WriteLine("Process Monitoring Starts -- by Shuo Ding\n");
            proc.InitXML();
            try
            {
                proc.Monitor();
            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
            }
        }
    }
}

//Class.

namespace ServerMonitor
{
    namespace ProcessManager
    {
        class ProcessMonitor
        {
            //Private declarations.
            private string processname;
            protected PerformanceCounter cpuCounter;
            protected PerformanceCounter ramCounter;
            protected bool killEnable;
            protected float cpuThreashold;
            protected float ramThreashold;
            protected string filename;
            protected string workpath;
            protected string logfile;
            protected int intInterval;

   
            protected delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            protected static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            protected static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("user32.dll", EntryPoint = "FindWindow")]
            private static extern IntPtr FindWindow(string lp1, string lp2);

            [DllImport("user32.dll")]
            protected static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

            [DllImport("user32.dll")]
            protected static extern bool IsWindowVisible(IntPtr hWnd);
            [DllImport("user32.dll")]
            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
            static uint WM_CLOSE = 0x10;
            protected static bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
            {
                int size = GetWindowTextLength(hWnd);
                if (size++ > 0 && IsWindowVisible(hWnd))
                {
                    StringBuilder sb = new StringBuilder(size);
                    GetWindowText(hWnd, sb, size);
                    if (sb.ToString().StartsWith("exampleApp"))
                    {
                        SendMessage(hWnd, WM_CLOSE, 0, 0);
                        Console.WriteLine("Send CLOSE message to close app\n");
                    }
                }
                return true;
            }


            /// Constructor.
            /// </summary>
            public ProcessMonitor()
            {
                //default setting 
                workpath = "C:\\Temp\\";
                killEnable = true;
                cpuThreashold = 50;
                ramThreashold = 1000;
                filename = "exampleApp.exe";
                logfile = "C:\\Temp\\Tracking.txt";
                processname = "exampleApp";
                intInterval = 10000;
            }
            public string Name
            {
                get;
                set;
            }
            public int InitXML()
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("tasks.xml");
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Monitor/task");
                foreach (XmlNode node in nodes)
                {
                    processname = node.SelectSingleNode("process").InnerText;
                    Name = processname;
                    filename = node.SelectSingleNode("filename").InnerText;
                    workpath = node.SelectSingleNode("workpath").InnerText;
                    string killenabled = node.SelectSingleNode("killenable").InnerText;
                    string memory_threshold = node.SelectSingleNode("memorythreshold").InnerText;
                    string cpu_threshold = node.SelectSingleNode("cputhreshold").InnerText;
                    logfile = node.SelectSingleNode("logfile").InnerText;
                    string inverval = node.SelectSingleNode("sleepinvervalms").InnerText;
                    if (killenabled == "no")
                        killEnable = false;
                    else
                        killEnable = true;

                    cpuThreashold = Int32.Parse(cpu_threshold);
                    ramThreashold = float.Parse(memory_threshold);
                    intInterval = Int32.Parse(inverval);
                }
                Console.WriteLine("Init XML OK\n");
                return 0;
            }

            /// Determines if the process is running or NOT.
            /// </summary>

            public bool IsProcessRunning()
            {
                Process[] proc = Process.GetProcessesByName(processname);
                // Console.WriteLine(string.Format("\n Get process  {0} Length  {1} \n", processname, proc.Length));
                Console.WriteLine("Process {0} Length {1}", processname, proc.Length);
                return (proc.Length > 0 && proc != null);
            }

            /* 
            Call this method every time you need to get 
            the amount of the available RAM in Mb 
            */

            /// <summary>

            /// Monitors the running program.

            /// </summary>

            public void Monitor()
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(logfile, true))
                    while (true)
                    {
                        if (IsProcessRunning())
                        {
                            Console.WriteLine("Monitoring {0} for CPU & RAM usage at {1} \n", processname, DateTime.Now.ToString());
                            Process[] runningNow = Process.GetProcessesByName(processname);
                            foreach (Process process in runningNow)
                            {
                                cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);
                                ramCounter = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName);
                                if (process.ProcessName == processname)
                                {
                                    cpuCounter.NextValue();
                                    System.Threading.Thread.Sleep(1000);
                                    Console.WriteLine("-----Process:{0} CPU {1}% Warning threshold {2}%", process.ProcessName,
                                       cpuCounter.NextValue(), cpuThreashold);
                                    ramCounter.NextValue();
                                    System.Threading.Thread.Sleep(1000);
                                    Console.WriteLine("-----Process:{0} RAM {1} Mb Warning threshold {2} Mb", process.ProcessName,
                                        ramCounter.NextValue() / (1024 * 1024), ramThreashold);

                                    if ((DateTime.Now.Minute == 20 || DateTime.Now.Minute == 40 || DateTime.Now.Minute == 59) && DateTime.Now.Second > 40)
                                    {   //log to file 
                                        file.WriteLine(string.Format("--Running {0} Monitor at {1} CPU: {2}% RAM: {3} Mb\n", processname, DateTime.Now.ToString(), cpuCounter.NextValue(), ramCounter.NextValue() / (1024 * 1024)));
                                    }
                                    if (IsUsingToLessResources())
                                    {
                                        Console.WriteLine(string.Format("Use Too less process: Killing {0} at {1} CPU: {2}% RAM: {3} Mb\n", processname, DateTime.Now.ToString(), cpuCounter.NextValue(), ramCounter.NextValue() / (1024 * 1024)));
                                        file.WriteLine(string.Format("Use Too less process: Killing {0} at {1} CPU: {2}% RAM: {3} Mb\n", processname, DateTime.Now.ToString(), cpuCounter.NextValue(), ramCounter.NextValue() / (1024 * 1024)));
                                        if (killEnable)
                                        {
                                            try
                                            {
                                                Console.WriteLine("Try to Close window box too low process \n");
                                                file.WriteLine("Try to Close window box too low process \n");
                                                // process.Kill(); //Kills the running process.                             
                                                //process.WaitForExit();  
                                                // process.Dispose();
                                                // process.Close();
                                                // process.CloseMainWindow();
                                                // Closebox();

                                                if (EnumWindows(EnumTheWindows, IntPtr.Zero))
                                                {
                                                    Console.WriteLine("Close Crashed App Success \n");
                                                    file.WriteLine("Closed Crashed App Success\n");
                                                    Thread.Sleep(intInterval); //ms 
                                                    //now restart 
                                                    using (Process myProcess = new Process())
                                                    {
                                                        //myProcess.StartInfo.UseShellExecute = false;
                                                        myProcess.StartInfo.FileName = filename;
                                                        myProcess.StartInfo.WorkingDirectory = workpath;
                                                        // myProcess.StartInfo.CreateNoWindow = true;
                                                        if (myProcess.Start())
                                                        {
                                                            Console.Write("{0} RE-Started. file {1} path {2} \n", processname, filename, workpath);
                                                            file.WriteLine(string.Format("Re-Started {0} at {1}\n", processname, DateTime.Now.ToString()));
                                                        }
                                                        else
                                                        {
                                                            Console.Write("{0} Restart Fail {1} path {2} \n", processname, filename, workpath);
                                                            file.WriteLine(string.Format("Re-Start {0} Fail at {1}\n", processname, DateTime.Now.ToString()));
                                                        }
                                                        file.Flush();
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Close window box Fail \n");
                                                    file.WriteLine("Closed crash app box Fail\n");
                                                }

                                                /*
                                                   string TaskKiller = "taskkill /f /im " + processname;
                                                  ProcessStartInfo info = new ProcessStartInfo("cmd.exe", "/c " + TaskKiller);
                                                  info.RedirectStandardError = true;
                                                  info.RedirectStandardInput = true;
                                                  info.RedirectStandardOutput = true;
                                                  info.UseShellExecute = false;
                                                  info.CreateNoWindow = true;
                                                  Process killprocess = new Process();
                                                  killprocess.StartInfo = info;
                                                  killprocess.Start();
                                                  killprocess.StandardOutput.ReadToEnd(); */
                                            }
                                            catch (Exception exception)
                                            {
                                                Console.Write(exception.Message);
                                                file.WriteLine(exception.Message);
                                            };
                                        }
                                        else
                                        {
                                            Console.WriteLine("However，Kill is NOT enabled\n");
                                            file.WriteLine("However, Kill is NOT enabled\n");
                                        }
                                        file.Flush();
                                    }
                                    if (IsUsingToMuchResources())
                                    {
                                        Console.WriteLine(string.Format("Killing Too Much {0} at {1} CPU: {2}% RAM: {3} Mb\n", processname, DateTime.Now.ToString(), cpuCounter.NextValue(), ramCounter.NextValue() / (1024 * 1024)));
                                        file.WriteLine(string.Format("Killing Too Much {0} at {1} CPU: {2}% RAM: {3} Mb\n", processname, DateTime.Now.ToString(), cpuCounter.NextValue(), ramCounter.NextValue() / (1024 * 1024)));
                                        if (killEnable)
                                        {
                                            try
                                            {
                                                process.Kill(); //Kills the running process.
                                            }
                                            catch (Exception exception)
                                            {
                                                Console.Write(exception.Message);
                                                file.WriteLine(exception.Message);
                                            };
                                        }
                                        else
                                        {
                                            Console.WriteLine("However，Kill is NOT enabled\n");
                                            file.WriteLine("However, Kill is NOT enabled\n");
                                        }
                                        file.Flush();
                                    }
                                }//end of if process is name                                 
                            }
                        }
                        else
                        {
                            Console.Write("**** {0} is NOT running--Start Now! Path: {1} file: {2} at {3} \n", processname, workpath, filename, DateTime.Now.ToString());
                            try
                            {
                                using (Process myProcess = new Process())
                                {
                                    //myProcess.StartInfo.UseShellExecute = false;
                                    myProcess.StartInfo.FileName = filename;
                                    myProcess.StartInfo.WorkingDirectory = workpath;
                                    // myProcess.StartInfo.CreateNoWindow = true;
                                    if (myProcess.Start())
                                    {
                                        Console.Write("{0} Started. file {1} path {2} \n", processname, filename, workpath);
                                        file.WriteLine(string.Format("Start new process {0} at {1}\n", processname, DateTime.Now.ToString()));
                                    }
                                    else
                                    {
                                        Console.Write("{0} start Fail {1} path {2} \n", processname, filename, workpath);
                                        file.WriteLine(string.Format("Start new process {0} Fail at {1}\n", processname, DateTime.Now.ToString()));
                                    }
                                    file.Flush();
                                }
                            }
                            catch (Exception exception)
                            {
                                Console.Write(exception.Message);
                                file.WriteLine(exception.Message);
                            };
                        }
                        // Sleep till the next loop
                        Thread.Sleep(intInterval); //ms   
                    }//while       
            }

            private bool IsUsingToMuchResources()
            {
                bool res = (ramCounter.NextValue() / (1024 * 1024)) > ramThreashold || cpuCounter.NextValue() > cpuThreashold;
                return res;
            }
            private bool IsUsingToLessResources()
            {
                bool res = (ramCounter.NextValue() / (1024 * 1024)) < 0.08;
                return res;
            }

            /// <summary>
            /// Kills the running process, selects the process from the function input
            /// parameter.
            /// </summary>
            /// <param name="program">The running process name.</param> 
        }

    }

}