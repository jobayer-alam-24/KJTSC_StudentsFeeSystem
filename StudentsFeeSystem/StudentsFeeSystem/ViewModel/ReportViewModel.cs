using StudentsFeeSystem.Models;

namespace StudentsFeeSystem.ViewModel
{
    public class ReportViewModel
    {
        public ReportSection Overall { get; set; }
        public Dictionary<int, ClassReport> ClassReports { get; set; }
    }

    public class ClassReport
    {
        public ReportSection Report { get; set; }
        public Dictionary<Department, ReportSection> DepartmentReports { get; set; }
    }

    public class ReportSection
    {
        public int StudentCount { get; set; }
        public int PaidCount { get; set; }
        public int NotPaidCount { get; set; }
        public decimal TotalCollected { get; set; }
    }

}
