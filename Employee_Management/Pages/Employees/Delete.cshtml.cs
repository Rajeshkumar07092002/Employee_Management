using Dapper;
using Employee_Management.DataClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Employee_Management.Pages.Employees
{
    public class DeleteModel : PageModel
    {
        public EmployeeInfo employeeinfo = new EmployeeInfo();
        public readonly IDatabaseConnection Dbc;
        public string errorMessage = "";
        public string successMessage = "";
        public DeleteModel(IDatabaseConnection dbc)
        {
            Dbc = dbc;
        }
        public async Task OnGetAsync()
        {
            string Id = Request.Query["Id"];

            try
            {
                using var connection = Dbc.GetConnection();
                var sql1 = @"select LoginDetails.Id,Employees.EmployeeNo,Employees.Name,Employees.Email,
                         LoginDetails.Username,LoginDetails.Password 
                        from LoginDetails Join Employees ON LoginDetails.EmployeeNo = Employees.EmployeeNo 
                       where Id=@Id";
                employeeinfo = await connection.QuerySingleAsync<EmployeeInfo>(sql1, new { Id = Id });

                var sql2 = @"delete from LoginDetails where Id=@Id";
                var sql3 = @"delete from Employees where EmployeeNo=@EmployeeNo";
                var a1 = await connection.ExecuteAsync(sql2, new { Id = employeeinfo.Id });
                var a2 = await connection.ExecuteAsync(sql3, new { EmployeeNo = employeeinfo.EmployeeNo });
            }
            catch (Exception ex)
            { 
                errorMessage=ex.Message;
                return;
            }
            successMessage = "Employee Deleted successfully";
            Response.Redirect("/Employees/Index");
        }
        public void OnPost()
        {
            
        }
    }
}
