﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Tasks;
using AlloK8.Common;
using AlloK8.Common.Models.Report;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AlloK8.BLL.Common.Reports;

internal class ReportService : IReportService
{
    private readonly ITaskService taskService;

    public ReportService(ITaskService taskService)
    {
        this.taskService = taskService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<List<ReportVM>> GetProjectProgressAsync(int projectId)
    {
        var tasks = await this.taskService.GetAllTasksByProjectIdAsync(projectId);

        var users = new List<string>();
        var labels = new List<string>();

        var reportVMs = new List<ReportVM>();

        foreach (var task in tasks)
        {
            var taskRecord = await this.taskService.GetTaskByIdAsync(task.Id);

            users.AddRange(taskRecord.Assignees
                .Select(a => a.ApplicationUser!.Email!));

            labels.AddRange(taskRecord.Labels
                .Select(l => l.Title!));

            var progress = task.ColumnId switch
            {
                1 => @T.ToDoColumnLabel,
                2 => @T.DoingColumnLabel,
                3 => @T.DoneColumnLabel,
                _ => "-",
            };

            var reportVM = new ReportVM
            {
                TaskName = task.Title,
                TaskDescription = task.Description,
                IsPriority = task.IsPriority,
                Progress = progress,
                StartDate = task.StartDate,
                DueDate = task.DueDate,
                Assignees = users.ToList(),
                Labels = labels.ToList(),
            };

            reportVMs.Add(reportVM);

            users.Clear();
            labels.Clear();
        }

        return reportVMs;
    }

    public async Task<byte[]> GenerateProjectReportPdfAsync(int projectId, string projectName)
    {
        var reportData = await this.GetProjectProgressAsync(projectId);

        var document = new ProjectReportDocument(reportData, projectName);

        byte[] pdfBytes;
        using (var stream = new MemoryStream())
        {
            document.GeneratePdf(stream);
            pdfBytes = stream.ToArray();
        }

        return pdfBytes;
    }

    private class ProjectReportDocument : IDocument
    {
        public ProjectReportDocument(List<ReportVM> reportData, string projectName)
        {
            this.ReportData = reportData;
            this.ProjectName = projectName;
            this.GeneratedDate = DateTime.Now;
        }

        private List<ReportVM> ReportData { get; }
        private string ProjectName { get; }
        private DateTime GeneratedDate { get; }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(50);

                    page.DefaultTextStyle(style => style.FontFamily("NotoSans"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text(@T.ReportPDFTitleText).FontSize(18).Bold();
                            column.Item().Text($"{@T.ProjectPDFTitleText}: {this.ProjectName}").FontSize(14);
                            column.Item().Text($"{@T.GeneratedPDFTitleText}: {this.GeneratedDate:dd/MM/yyyy}").FontSize(10);
                        });
                    });
                    page.Content().PaddingVertical(10).Column(column =>
            {
                column.Item().Text(@T.TasksPDFTitleText).FontSize(16).Bold();
                column.Spacing(5);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(4);
                        columns.ConstantColumn(60);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).Text("#").FontSize(10);
                        header.Cell().DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).Text(@T.TaskColumnTitlePDFText).FontSize(10);
                        header.Cell().DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).Text(@T.DescriptionModalEditTaskText).FontSize(10);
                        header.Cell().DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).Text(@T.PriorityColumnTitlePDFText).FontSize(10);
                        header.Cell().DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).Text(@T.ProgressColumnTitlePDFText).FontSize(10);
                        header.Cell().DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).Text(@T.StartDatePDFText).FontSize(10);
                        header.Cell().DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).Text(@T.DueDatePDFText).FontSize(10);
                        header.Cell().DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).Text(@T.AssigneesColumnTitlePDFText).FontSize(10);
                        header.Cell().DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).Text(@T.LabelsViewTitle).FontSize(10);
                    });

                    for (int i = 0; i < this.ReportData.Count; i++)
                    {
                        var item = this.ReportData[i];
                        var rowNumber = i + 1;

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(rowNumber.ToString()).FontSize(10);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(item.TaskName).FontSize(10);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(this.TextTruncate(item.TaskDescription!, 100)).FontSize(10);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(item.IsPriority ? @T.HighTitlePDFText : @T.NormalTitlePDFText).FontSize(10);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(item.Progress).FontSize(10);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(((DateTime)item.StartDate!).ToString("dd/MM/yyyy")).FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(((DateTime)item.DueDate!).ToString("dd/MM/yyyy")).FontSize(8);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(string.Join(", ", item.Assignees)).FontSize(10);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(string.Join(", ", item.Labels)).FontSize(10);
                    }
                });

                column.Spacing(10);
            });
                    page.Footer().Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().AlignCenter().Text(text =>
                        {
                            text.Span("AlloK8").Bold().FontSize(8);
                            text.Span($" - {@T.PagePDFText} ").NormalWeight().FontSize(8);
                            text.CurrentPageNumber().FontSize(8);
                            text.Span($" {@T.OfPDFText} ").NormalWeight().FontSize(8);
                            text.TotalPages().FontSize(8);
                        });
                    });
                });
            });
        }

        private string TextTruncate(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return text.Length <= maxLength
                ? text
                : text.Substring(0, maxLength) + "...";
        }
    }
}