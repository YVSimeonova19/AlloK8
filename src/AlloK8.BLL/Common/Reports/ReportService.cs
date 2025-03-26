using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Tasks;

namespace AlloK8.BLL.Common.Invoices;

internal class ReportService : IReportService
{
    private readonly ITaskService taskService;

    public ReportService(ITaskService taskService)
    {
        this.taskService = taskService;
    }

    public async Task<DataTable> GetProjectProgressAsync(int projectId)
    {
        var tasks = await this.taskService.GetAllTasksByProjectIdAsync(projectId);

        // Create DataTable with appropriate columns
        DataTable dataTable = new DataTable("ProjectProgress");

        // Add task columns
        dataTable.Columns.Add("TaskName", typeof(string));
        dataTable.Columns.Add("TaskDescription", typeof(string));
        dataTable.Columns.Add("IsPriority", typeof(bool));
        dataTable.Columns.Add("Progress", typeof(string));
        dataTable.Columns.Add("StartDate", typeof(DateTime));
        dataTable.Columns.Add("DueDate", typeof(DateTime));
        dataTable.Columns.Add("Assignees", typeof(string));
        dataTable.Columns.Add("Labels", typeof(string));

        var users = new List<string>();
        var labels = new List<string>();

        foreach (var task in tasks)
        {
            var taskRecord = await this.taskService.GetTaskByIdAsync(task.Id);

            users.AddRange(taskRecord.Assignees
                .Select(a => a.ApplicationUser!.Email!));

            labels.AddRange(taskRecord.Labels
                .Select(l => l.Title!));

            var progress = task.ColumnId switch
            {
                1 => "To Do",
                2 => "In Progress",
                3 => "Done",
                _ => "Undefined",
            };

            // Create a new row for the DataTable
            DataRow row = dataTable.NewRow();

            // Add task info
            row["TaskName"] = task.Title;
            row["TaskDescription"] = task.Description;
            row["IsPriority"] = task.IsPriority;
            row["Progress"] = progress;
            row["StartDate"] = task.StartDate;
            row["DueDate"] = task.DueDate;
            row["Assignees"] = string.Join(", ", users);
            row["Labels"] = string.Join(", ", labels);

            // Add the row to the DataTable
            dataTable.Rows.Add(row);

            users.Clear();
            labels.Clear();
        }

        return dataTable;
    }
}