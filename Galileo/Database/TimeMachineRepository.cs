using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Galileo.Models;
using System.Collections.Generic;

namespace Galileo.Database
{
    public class TimeMachineRepository
    {
        private readonly string _connectionString;

        public TimeMachineRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["TimeMachineConnectionString"].ConnectionString;
        }

        public User GetUser(string userId)
        {
            string sql = "SELECT * FROM [USER] WHERE user_id = @userId";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var user = connection.Query<User>(sql, new { userId });
                return user.FirstOrDefault();
            }
        }

        public List<Course> GetCourses(string userId)
        {
            string sql = @"
SELECT c.*, sum(e.entry_total_time)/60 as course_total_hours
FROM [COURSE] c
JOIN [MEMBER] m ON m.member_course_id = c.course_id
JOIN [PROJECT] p ON p.project_course_id = c.course_id
JOIN [USER] u ON m.member_user_id = u.user_id
LEFT JOIN [ENTRY] e ON e.entry_project_id = p.project_id
WHERE u.user_id = 'mgeary'
AND   UPPER(m.member_position) = UPPER('Teacher')
GROUP BY c.course_id, c.course_name, c.course_submit_day, c.course_date_created, c.course_begin_date, c.course_end_date, c.course_is_enabled, c.course_ref_grade, c.course_ref_hours;";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var courses = connection.Query<Course>(sql, new { userId });
                return courses.AsList();
            }
        }

        public List<Project> GetProjects(int course_id)
        {
            string sql = @"
select p.*, sum(e.entry_total_time)/60 as project_total_hours from [PROJECT] p 
JOIN [ENTRY] e ON e.entry_project_id = p.project_id
where p.project_course_id = @course_id
group by p.project_begin_date, p.project_course_id, p.project_created_by, p.project_date_created, p.project_description, p.project_end_date, p.project_id, p.project_id, p.project_is_enabled, p.project_name";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var projects = connection.Query<Project>(sql, new { course_id });
                return projects.AsList();
            }
        }
    }
}