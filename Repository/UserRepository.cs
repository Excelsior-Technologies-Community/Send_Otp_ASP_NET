using System.Data.SqlClient;
using System.Data;
using Send_Otp_ASP_NET.Models.User;

namespace Send_Otp_ASP_NET.Repository
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<UserSave> Upsert(long EmployeeId, string Name, int Age, string City, string State, string ActionMode)
        {
            List<UserSave> result = new List<UserSave>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_Employee_Save", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", EmployeeId);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@Age", Age);
                cmd.Parameters.AddWithValue("@City", City);
                cmd.Parameters.AddWithValue("@State", State);
                cmd.Parameters.AddWithValue("@Action", ActionMode);

                conn.Open();

                int rowsAffected = cmd.ExecuteNonQuery();

                // Return success status
                if (rowsAffected > 0)
                {
                    result.Add(new UserSave
                    {
                        EmployeeId = EmployeeId.ToString(),
                        ReturnStatus = "1"
                    });
                }
                else
                {
                    result.Add(new UserSave
                    {
                        EmployeeId = EmployeeId.ToString(),
                        ReturnStatus = "0"
                    });
                }
            }

            return result;
        }


        public bool UserDelete(Int64 EmployeeId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Employee_Delete", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", EmployeeId);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
        }

        public List<UserList> UserList()
        {
            List<UserList> ulr = new List<UserList>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Employee_List", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ulr.Add(new UserList
                            {
                                EmployeeId = Convert.ToInt64(reader["EmployeeID"]),
                                Name = reader["Name"].ToString(),
                                Age = reader["Age"].ToString(),
                                City = reader["City"].ToString(),
                                State = reader["State"].ToString()

                            });

                        }
                    }
                }

            }
            return ulr;
        }

        public UserEdit UserEdit(long EmployeeId)
        {
            UserEdit emp = new UserEdit();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Employee_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("@Id", EmployeeId);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            emp.EmployeeId = Convert.ToInt64(reader["EmployeeID"]);
                            emp.Name = reader["Name"].ToString();
                            emp.Age = reader["Age"].ToString();
                            emp.City = reader["City"].ToString();
                            emp.State = reader["State"].ToString();
                        }
                    }
                }
            }

            return emp;
        }

        public bool IsNameExists(string name, long employeeId = 0)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Employee WHERE Name = @Name";

                if (employeeId > 0)
                    query += " AND EmployeeID <> @EmployeeId";

                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);

                    if (employeeId > 0)
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public void SaveOtp(string mobile, string otp, DateTime expireAt)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(@"
                INSERT INTO tbl_UserOtp (Mobile, Otp, ExpireAt)
                VALUES (@Mobile, @Otp, @ExpireAt)", con);

            cmd.Parameters.AddWithValue("@Mobile", mobile);
            cmd.Parameters.AddWithValue("@Otp", otp);
            cmd.Parameters.AddWithValue("@ExpireAt", expireAt);

            con.Open();
            cmd.ExecuteNonQuery();
        }

       
        public bool VerifyOtp(string mobile, string otp)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(@"
                SELECT COUNT(*) FROM tbl_UserOtp
                WHERE Mobile = @Mobile
                AND Otp = @Otp
                AND ExpireAt >= GETDATE()", con);

            cmd.Parameters.AddWithValue("@Mobile", mobile);
            cmd.Parameters.AddWithValue("@Otp", otp);

            con.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count > 0)
            {
                // delete OTP after successful verification
                DeleteOtp(mobile);
                return true;
            }

            return false;
        }

        private void DeleteOtp(string mobile)
        {
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(
                "DELETE FROM tbl_UserOtp WHERE Mobile = @Mobile", con);

            cmd.Parameters.AddWithValue("@Mobile", mobile);

            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
