using Dapper;
using Employee_Management.DataClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Employee_Management.Pages.Employees
{
    public class CreateModel : PageModel
    {
        public EmployeeInfo employeeInfo = new EmployeeInfo();
        public List<EmployeeInfo> EmployeesList = new List<EmployeeInfo>();

        public readonly IDatabaseConnection Dbc;

        public string errorMessage = "";
        public string successMessage = "";

        public string passwordStrength = "";
        public string temp_pass_strength = "";
        public CreateModel(IDatabaseConnection dbc)
        {
            Dbc= dbc;
        }
       
        public void OnGet()
        {
           
        }
        public async Task OnPost()
        {


            try
            {
                using var connection = Dbc.GetConnection();

                string sql = @"select LoginDetails.Id, Employees.EmployeeNo,Employees.Name,Employees.Email,LoginDetails.Username,
                     LoginDetails.Password
                     from LoginDetails INNER JOIN Employees 
                     ON LoginDetails.EmployeeNo = Employees.EmployeeNo";

                EmployeesList = (List<EmployeeInfo>)(await connection.QueryAsync<EmployeeInfo>(sql));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }


            employeeInfo.EmployeeNo = Request.Form["employeeno"];
            employeeInfo.Name = Request.Form["name"];
            employeeInfo.Email = Request.Form["email"];
            employeeInfo.Username = Request.Form["username"];
            employeeInfo.Password = Request.Form["password"];
            passwordStrength = Request.Form["passwordStrength"];

           // Console.WriteLine("Line 56 ", passwordStrength);
            if(employeeInfo.Username.Length==0 || employeeInfo.Password.Length==0 ||
                employeeInfo.EmployeeNo==null || employeeInfo.Name.Length == 0 || employeeInfo.Email.Length == 0)
            {
                errorMessage = "All fields are required";
                return;
            }

            // save this employee to database

            try
            {
                try
                {

                    foreach (var Employee in EmployeesList)
                    {
                        if (Employee.EmployeeNo == employeeInfo.EmployeeNo)
                        {
                            errorMessage = "This EmployeeNo already exist";
                            return;
                        }
                        if (Employee.Email == employeeInfo.Email)
                        {
                            errorMessage = "This Email already exist";
                            return;
                        }
                        if (Employee.Username == employeeInfo.Username)
                        {
                            errorMessage = "This Username already exist";
                            return;
                        }
                        if (Employee.Password == employeeInfo.Password)
                        {
                            errorMessage = "Password  should be unique";
                            return;
                        }
                    }
                   
                        string temp_password = employeeInfo.Password;
                        
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
                        else if ((hasLower || hasUpper) & specialChar && (n >= 7))
                        {
                            temp_pass_strength = "2";
                        }
                        else if(n>=5)
                        {
                            temp_pass_strength = "1";
                        }  
                        else
                        {
                            errorMessage = "Password Strength must be atleast of 5 length";
                            return;
                        }

                    if (passwordStrength == "1")
                    {
                        if(temp_pass_strength == "1" || temp_pass_strength=="2" || temp_pass_strength == "3")
                        {

                        }
                        else
                        {
                            errorMessage = @"Low Password Strength must be atleast of 5 length";
                            return;
                        }
                    }
                    else if(passwordStrength == "2")
                    {
                        if (temp_pass_strength == "2" || temp_pass_strength == "3")
                        {

                        }
                        else
                        {
                            errorMessage = @" Medium Password Strength must be atleast of 7 length and has atleast 
                                one upperCase or lowerCase and special Character";
                            return;
                        }
                    }
                    else if(passwordStrength == "3")
                    {
                        if (temp_pass_strength == "3")
                        {

                        }
                        else
                        {
                            errorMessage = @"High Password Strength must be atleast of 10 length and has atleast 
                                one upperCase and lowerCase and special Character";
                            return;
                        }
                    }
                    else
                    {
                        errorMessage = "Select PasswordStrength";
                        return;
                    }

                    using var connection = Dbc.GetConnection();
                    string sql1 = @"SET IDENTITY_INSERT Employees ON
                                         INSERT into Employees
                                           (EmployeeNo,Name,Email) Values
                                          (@EmployeeNo,@Name,@Email)
                                           SET IDENTITY_INSERT Employees OFF ";
                    await connection.ExecuteAsync(sql1, new { EmployeeNo = employeeInfo.EmployeeNo, Name = employeeInfo.Name, Email = employeeInfo.Email });

                    string sql = @"INSERT into LoginDetails
                                           (EmployeeNo,Username,Password) Values
                                          (@EmployeeNo,@Username,@Password)";
                    await connection.ExecuteAsync(sql, new {EmployeeNo = employeeInfo.EmployeeNo,Username= employeeInfo.Username,Password = employeeInfo.Password });

                }
                catch (Exception ex)
                {
                    errorMessage=ex.Message;
                    return;
                }

               
               
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }


            employeeInfo.EmployeeNo = ""; employeeInfo.Username = ""; employeeInfo.Password = "";
            successMessage = "New employee added correctly";
            Response.Redirect("/Employees/Index");
        }
    }
}
