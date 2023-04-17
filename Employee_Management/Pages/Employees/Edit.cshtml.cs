using Dapper;
using Employee_Management.DataClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Employee_Management.Pages.Employees
{
    public class EditModel : PageModel
    {
        public EmployeeInfo employeeInfo = new EmployeeInfo();
        public EmployeeInfo employeeInfop = new EmployeeInfo();
        public List<EmployeeInfo> EmployeesList = new List<EmployeeInfo>();

        public string errorMessage = "";
        public string successMessage = "";

        public string passwordStrength = "";
        public string temp_pass_strength = "";


        public readonly IDatabaseConnection Dbc;

        public EditModel(IDatabaseConnection dbc)
        {
            Dbc = dbc;
        }
        public async Task OnGetAsync()
        {
            string Id= Request.Query["Id"];
            try
            {
                using var connection = Dbc.GetConnection();

                var sql1 = @"select LoginDetails.Id,Employees.EmployeeNo,Employees.Name,Employees.Email,
                         LoginDetails.Username,LoginDetails.Password 
                        from LoginDetails Join Employees ON LoginDetails.EmployeeNo = Employees.EmployeeNo 
                       where Id=@Id";
                employeeInfo = await connection.QuerySingleAsync<EmployeeInfo>(sql1, new { Id = Id });

            }
            catch (Exception ex) 
            {
                errorMessage= ex.Message;
                return;
            }
        }
        public async Task OnPostAsync() 
        {
            using var connection = Dbc.GetConnection();


            employeeInfop.Id = Request.Form["Id"];
            employeeInfop.Name = Request.Form["Name"];
            employeeInfop.Email = Request.Form["Email"];
            employeeInfop.Username = Request.Form["Username"];
            employeeInfop.Password = Request.Form["Password"];
            employeeInfop.EmployeeNo = Request.Form["EmployeeNo"]; ;
            passwordStrength = Request.Form["passwordStrength"];

            if (employeeInfop.Id==null|| employeeInfop.Name.Length == 0 || employeeInfop.Email.Length == 0||
                employeeInfop.Password.Length == 0 || employeeInfop.Username.Length==0)
            {
                errorMessage = "All fields are required";
                return;
            }

            string sql = @"select LoginDetails.Id, Employees.EmployeeNo,Employees.Name,Employees.Email,LoginDetails.Username,
                     LoginDetails.Password
                     from LoginDetails INNER JOIN Employees 
                     ON LoginDetails.EmployeeNo = Employees.EmployeeNo where Id!=@Id";

            EmployeesList = (List<EmployeeInfo>)(await connection.QueryAsync<EmployeeInfo>(sql, new { Id = employeeInfop.Id }));

            try
            {

                string temp_password = employeeInfop.Password;

                int n = temp_password.Length;
                bool hasLower = false, hasUpper = false,
                        hasDigit = false, specialChar = false;
                HashSet<char> set = new HashSet<char>(
                    new char[] { '!', '@', '#', '$', '%', '^', '&',
                          '*', '(', ')', '-', '+' });
                foreach (char i in temp_password.ToCharArray())
                {
                    if (char.IsLower(i))
                        hasLower = true;
                    if (char.IsUpper(i))
                        hasUpper = true;
                    if (char.IsDigit(i))
                        hasDigit = true;
                    if (set.Contains(i))
                        specialChar = true;
                }

                // Strength of password
                if (hasDigit && hasLower && hasUpper && specialChar && (n >= 10))
                {
                    temp_pass_strength = "3";
                }
                else if ((hasLower || hasUpper ) & specialChar && (n >= 7))
                {
                    temp_pass_strength = "2";
                }
                else if (n >= 5)
                {
                    temp_pass_strength = "1";
                }
                else
                {
                    errorMessage = "Password Strength must be atleast of 5 length";
                    employeeInfo = employeeInfop;
                    return;
                }

                if (passwordStrength == "1")
                {
                    if (temp_pass_strength == "1" || temp_pass_strength == "2" || temp_pass_strength == "3")
                    {

                    }
                    else
                    {
                        errorMessage = @"Low Password Strength must be atleast of 5 length";
                        employeeInfo= employeeInfop;
                        return;
                    }
                }
                else if (passwordStrength == "2")
                {
                    if (temp_pass_strength == "2" || temp_pass_strength == "3")
                    {

                    }
                    else
                    {
                        errorMessage = @" Medium Password Strength must be atleast of 7 length and has atleast 
                                one upperCase or lowerCase and special Character";
                        employeeInfo   = employeeInfop;
                        return;
                    }
                }
                else if (passwordStrength == "3")
                {
                    if (temp_pass_strength == "3")
                    {

                    }
                    else
                    {
                        errorMessage = @"High Password Strength must be atleast of 10 length and has atleast 
                                one upperCase and lowerCase and special Character";
                        employeeInfo = employeeInfop;
                        return;
                    }
                }
                else
                {
                    errorMessage = "Select PasswordStrength";
                    employeeInfo = employeeInfop;
                    return;
                }


                foreach (var Employee in EmployeesList)
                {
                    if (Employee.EmployeeNo == employeeInfop.EmployeeNo)
                    {
                        errorMessage = "This EmployeeNo already exist";
                        employeeInfo = employeeInfop;
                        return;
                    }
                    if (Employee.Email == employeeInfop.Email)
                    {
                        errorMessage = "This Email already exist";
                        employeeInfo = employeeInfop;
                        return;
                    }
                    if (Employee.Username == employeeInfop.Username)
                    {
                        errorMessage = "This Username already exist";
                        employeeInfo = employeeInfop;
                        return;
                    }
                    if (Employee.Password == employeeInfop.Password)
                    {
                        errorMessage = "Password  should be unique";
                        employeeInfo = employeeInfop;
                        return;
                    }
                }


                string sql2 = @"UPDATE LoginDetails SET Username=@Username, Password=@Password WHERE Id=@Id";
              var a1 =  await connection.ExecuteAsync(sql2, new { Username = employeeInfop.Username, Password = employeeInfop.Password,Id= employeeInfop.Id});

                string sql3= @"UPDATE Employees SET Name=@Name, Email=@Email WHERE EmployeeNo=@EmployeeNo";
               var a2 = await connection.ExecuteAsync(sql3, new { Name = employeeInfop.Name, Email = employeeInfop.Email, EmployeeNo = employeeInfop.EmployeeNo});

               /* Console.WriteLine(a1);
                Console.WriteLine(a2);*/
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            employeeInfo.EmployeeNo = ""; employeeInfo.Username = ""; employeeInfo.Password = "";
            successMessage = "Employee Updated successfully";
            Response.Redirect("/Employees/Index");
        }
    }
}
