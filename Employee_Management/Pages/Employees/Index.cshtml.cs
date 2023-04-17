using Dapper;
using Employee_Management.DataClass;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace Employee_Management.Pages.Employees
{
    public class IndexModel : PageModel
    {
        public List<EmployeeInfo> ListEmployees=new List<EmployeeInfo>();

        [BindProperty]
        public EmployeeInfo EmployeeInfo { get; set; } = new EmployeeInfo();

       public readonly IDatabaseConnection Dbc;
       public IndexModel(IDatabaseConnection dbc)
        {
            Dbc = dbc;
        }

        public async Task OnGetAsync()
        {
            try
            {
                   using var connection = Dbc.GetConnection();
                    
                    string sql = @"select LoginDetails.Id, Employees.EmployeeNo,Employees.Name,Employees.Email,LoginDetails.Username,
                     LoginDetails.Password
                     from LoginDetails INNER JOIN Employees 
                     ON LoginDetails.EmployeeNo = Employees.EmployeeNo"; 

                  ListEmployees = (List<EmployeeInfo>)(await connection.QueryAsync<EmployeeInfo>(sql));

               // ListEmployees = (await connection.QueryAsync<EmployeeInfo>("select * from LoginDetails")).ToList();


            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " +  ex.ToString());
            }
        }

        public async Task<IActionResult> OnPostUpdateEmployee()
        {
            using var connection = Dbc.GetConnection();
            string sql2 = @"UPDATE LoginDetails SET Username=@Username, Password=@Password WHERE Id=@Id";
            var a1 = await connection.ExecuteAsync(sql2, new { Username = EmployeeInfo.Username, Password = EmployeeInfo.Password, Id = EmployeeInfo.Id });

            string sql3 = @"UPDATE Employees SET Name=@Name, Email=@Email WHERE EmployeeNo=@EmployeeNo";
            var a2 = await connection.ExecuteAsync(sql3, new { Name = EmployeeInfo.Name, Email = EmployeeInfo.Email, EmployeeNo = EmployeeInfo.EmployeeNo });
            return RedirectToPage();
        }

    }

   
}
