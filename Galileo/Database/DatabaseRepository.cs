using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Galileo.Models;
using System.Collections.Generic;

namespace Galileo.Database
{
    public class DatabaseRepository
    {
        private readonly string _connectionString;

        public DatabaseRepository()
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
WHERE u.user_id = @userId
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

        public List<User> GetUsersInCourse(int course_id)
        {
            string sql = @"
SELECT e.entry_user_id as user_id, u.user_first_name, u.user_last_name, sum(e.entry_total_time)/60 as user_total_hours from [USER] u
join ENTRY e on e.entry_user_id = u.user_id
join PROJECT p on e.entry_project_id = p.project_id
where p.project_course_id = @course_id 
group by u.user_first_name, u.user_last_name, e.entry_user_id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var users = connection.Query<User>(sql, new { course_id });
                return users.AsList();
            }
        }

        public List<User> GetUsersInProject(int projectId)
        {
            string sql = @"
SELECT user_id, user_first_name, user_last_name, SUM(entry_total_time)/60 as user_total_hours
  FROM [SEI_TimeMachine2].[dbo].[USER]
  join [SEI_TimeMachine2].[dbo].[entry] on user_id = entry_user_id
  join [SEI_TimeMachine2].[dbo].[project] on project_id = entry_project_id
  where project_id = @projectId
  group by user_id, user_first_name, user_last_name
  order by user_total_hours";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var users = connection.Query<User>(sql, new { projectId });
                return users.AsList();
            }
        }

        public List<Entry> GetUserEntries(string userId)
        {
            string sql = @"
select e.*, u.user_first_name, u.user_last_name, p.project_name, c.course_name, sum(e.entry_total_time)/60 as entry_total_hours from [entry] e
JOIN [PROJECT] p ON e.entry_project_id    = p.project_id
JOIN [COURSE]  c ON p.project_course_id = c.course_id
JOIN [user] u ON e.entry_user_id = u.user_id
where e.entry_user_id = @userId
group by u.user_first_name, u.user_last_name, p.project_name, c.course_name, entry_id, entry_begin_time, entry_end_time, entry_total_time, entry_work_accomplished, entry_comment, entry_user_id, entry_project_id, entry_location_id, entry_category_id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var entries = connection.Query<Entry>(sql, new { userId });
                return entries.AsList();
            }
        }

        public void InsertTeamMembers(List<Team> teams)
        {
            bool addComma = false;
            string sql = @"INSERT INTO [SEI_Galileo].[dbo].[ROLE] (student_id, team_id, position) VALUES ";

            for (int teamIndex = 0; teamIndex < teams.Count; teamIndex++)
            {
                string[] userIds = null;
                if (teams[teamIndex].userIds != null)
                    userIds = teams[teamIndex].userIds.Split(',');
                for (int userIndex = 0; userIndex < userIds.Length; userIndex++)
                {
                    if (addComma)
                        sql += ", ";
                    else
                        addComma = true;

                    if (teamIndex == 0)
                    {
                        if (userIndex == 0)
                            sql += "(" + userIds[userIndex] + ", " + teams[teamIndex].projectId + ", 'PROJECT_MANAGER')";
                    }
                    else if (userIndex == 0)
                        sql += "(" + userIds[userIndex] + ", " + teams[teamIndex].projectId + ", 'TEAM_LEADER')";
                    else
                        sql += "(" + userIds[userIndex] + ", " + teams[teamIndex].projectId + ", 'MEMBER')";
                }
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var results = connection.Query(sql);
                return;
            }
        }

        public List<Entry> GetEntries(int courseId)
        {
            string sql = @"SELECT e.*" +
      ", c.course_name"+
      ",u.user_id"+
      ",u.user_first_name" +
      ",u.user_last_name" +
      ",p.project_name" +
"FROM[ENTRY]   e" +
"JOIN[PROJECT] p ON entry_project_id = project_id" +
"JOIN[COURSE]  c ON p.project_course_id = @courseId" +
"JOIN[USER]    u ON e.entry_user_id = u.user_id" +
"where c.course_id = 25" +
"order by entry_user_id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var entries = connection.Query<Entry>(sql, new { courseId });
                return entries.AsList();
            }
        }
    }
}