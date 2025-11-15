using DateTime_Validator.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using TaskSch = Microsoft.Win32.TaskScheduler;

namespace DateTime_Validator
{
    public class Program
    {
        static async Task Main()
        {
            Console.ForegroundColor = ConsoleColor.White;

            var addTaskSchTask = AddTaskScheduler();

            const string API_DATE = "https://api.ineo-team.ir/timezone.php?action=date&zone=en";
            const string API_TIME = "https://api.ineo-team.ir/timezone.php?action=time&zone=en"; // UTC
            
            while (true)
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        var apiDateResponse = await client.GetAsync(API_DATE);
                        var apiTimeResponse = await client.GetAsync(API_TIME);
                        if (apiDateResponse.IsSuccessStatusCode && apiTimeResponse.IsSuccessStatusCode)
                        {
                            var dateJsonResponse = await apiDateResponse.Content.ReadAsStringAsync();
                            var timeJsonResponse = await apiTimeResponse.Content.ReadAsStringAsync();

                            var apiDateModel = JsonConvert.DeserializeObject<ApiDate>(dateJsonResponse);
                            var apiTimeModel = JsonConvert.DeserializeObject<ApiTime>(timeJsonResponse);

                            if (apiDateModel.Status_code == 200 && apiTimeModel.Status_code == 200)
                            {
                                DateTime dateTime = new DateTime(apiDateModel.Result.Year, apiDateModel.Result.Month, apiDateModel.Result.Day,
                                    apiTimeModel.Result.Houre, apiTimeModel.Result.Minute, apiTimeModel.Result.Second);
                                var res1 = SystemDateTimeHelper.SetWindowsSystemDateTime(dateTime);
                                var res2 = TimeZoneHelper.EnsureTimeZoneIsTehran();
                                //if (!res1 || !res2)
                                PressEnterToExit();
                                break;
                            }
                            else
                            {
                                PrintError_Api();
                                break;
                            }
                        }
                        else
                        {
                            PrintError_Api();
                            break;
                        }
                    }
                    catch
                    {
                        PrintError_Connection();
                        Console.WriteLine("[Try Again]");
                        await Task.Delay(5000);
                    }
                }
            }
            addTaskSchTask.Wait();
        }

        static void PrintError_Connection()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[Connection Error]");
        }

        static void PrintError_Api()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[Api Error]");
            PressEnterToExit();
        }

        public static void PressEnterToExit()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Press enter to exit form program...");
            //Console.ReadLine();
        }

        private async static Task AddTaskScheduler()
        {
            const string TASK_FOLDER_NAME = "QueuingSystem";
            const string TASK_NAME = "DateTime-Validator";
            await Task.Run(() =>
            {
                using (var ts = new TaskSch.TaskService())
                {
                    var rootFolder = ts.RootFolder;
                    TaskSch.TaskFolder taskFolder;

                    if (!rootFolder.SubFolders.Exists(TASK_FOLDER_NAME))
                        taskFolder = rootFolder.CreateFolder(TASK_FOLDER_NAME);

                    else if ((taskFolder = ts.GetFolder(TASK_FOLDER_NAME)).Tasks.Any(x => x.Name == TASK_NAME))
                        return;

                    TaskSch.TaskDefinition td = ts.NewTask();

                    // تنظیم Principal به SYSTEM
                    td.Principal.UserId = "NT AUTHORITY\\SYSTEM";
                    td.Principal.LogonType = TaskSch.TaskLogonType.ServiceAccount;

                    // Trigger: اجرا در هنگام راه‌اندازی سیستم (Startup)
                    td.Triggers.Add(new TaskSch.BootTrigger { Delay = TimeSpan.Zero });

                    // Action: اجرای برنامه با مسیر مشخص
                    var actionPath = Assembly.GetExecutingAssembly().Location;
                    td.Actions.Add(new TaskSch.ExecAction(actionPath, null, null));

                    // ثبت تسک
                    taskFolder.RegisterTaskDefinition(TASK_NAME, td);

                    Console.WriteLine($"[Task \"{TASK_NAME}\" created successfully]");
                }
            });
        }
    }
}
