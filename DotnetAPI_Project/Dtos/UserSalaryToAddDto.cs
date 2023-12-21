namespace DotnetAPI_Project.Dtos
{
    public partial class UserSalaryToAddDto
    {
        public int UserId { get; set; }
        public decimal Salary { get; set; }
        public decimal AvgSalary { get; set; }
    }
}
