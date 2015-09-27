using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Galileo.Models;
using System.Collections.Generic;
using System;

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
select p.*, sum(e.entry_total_time)/60 as project_total_hours,
CASE WHEN EXISTS(SELECT * 
                 FROM [SEI_Galileo].[dbo].[ROLE] AS r
                 WHERE p.project_id = r.team_id) 
             THEN 1
             ELSE 0
       END AS project_is_team
from PROJECT p
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

        public List<User> GetUsersInTeam(int teamId)
        {
            string sql = @"
    SELECT user_id, r.team_id, user_first_name, user_last_name, ISNULL(SUM(entry_total_time)/60, 0) as user_total_hours,
		CASE WHEN r.position = 'TEAM_LEADER'
		THEN 1
		ELSE 0
	END AS user_is_team_leader, 
		CASE WHEN r.position = 'PROJECT_MANAGER'
		THEN 1
		ELSE 0
	END AS user_is_project_manager
	
  FROM [SEI_Galileo].[dbo].[ROLE] r
  join [SEI_TimeMachine2].[dbo].[USER] on user_id = r.student_id
  join [SEI_TimeMachine2].[dbo].[project] on r.team_id = project_id 
  left join [SEI_TimeMachine2].[dbo].[entry] e on project_id = entry_project_id and e.entry_user_id = r.student_id
  where project_id = @teamId
  group by user_id, user_first_name, user_last_name, r.team_id, position
  order by user_total_hours";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var users = connection.Query<User>(sql, new { teamId });
                return users.AsList();
            }
        }

        public List<User> GetAllUsers()
        {
            string sql = @"select USER_ID, user_first_name, user_last_name from [user]";
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var users = connection.Query<User>(sql);
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

        public void InsertProjectManager(string userId, int[] projectIds)
        {
            string sql = @"INSERT INTO [SEI_Galileo].[dbo].[ROLE] (student_id, team_id, position) VALUES ";
            var values = new List<string>();

            foreach (int projectId in projectIds)
                values.Add("(" + userId + ", " + projectId + ", 'PROJECT_MANAGER')");

            if (values.Any())
            {
                sql += string.Join(",", values);
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    connection.Query(sql);
                }
            }
        }

        public void InsertTeamMembers(List<Team> teams)
        {
            string sql = @"INSERT INTO [SEI_Galileo].[dbo].[ROLE] (student_id, team_id, position) VALUES ";
            var values = new List<string>();

            foreach (var team in teams)
            {
                string[] userIds = null;
                if (team.userIds != null)
                {
                    userIds = team.userIds.Split(',');
                    for (int userIndex = 0; userIndex < userIds.Length; userIndex++)
                    {
                        if (userIndex == 0)
                            values.Add("(" + userIds[userIndex] + ", " + team.projectId + ", 'TEAM_LEADER')");
                        else
                            values.Add("(" + userIds[userIndex] + ", " + team.projectId + ", 'MEMBER')");
                    }
                }
            }

            if (values.Any())
            {
                sql += string.Join(",", values);
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    connection.Query(sql);
                }
            }
        }

        public List<Entry> GetEntriesForCourse(int courseId)
        {
            string sql = @"
SELECT e.*, c.course_name, u.user_first_name, u.user_last_name, p.project_name, sum(e.entry_total_time)/60 as entry_total_hours
FROM[ENTRY]   e
JOIN[PROJECT] p ON entry_project_id = project_id
JOIN[COURSE]  c ON p.project_course_id = c.course_id
JOIN[USER]    u ON e.entry_user_id = u.user_id
where c.course_id = @courseId
group by u.user_first_name, u.user_last_name, p.project_name, c.course_name, entry_id, entry_begin_time, entry_end_time, entry_total_time, entry_work_accomplished, entry_comment, entry_user_id, entry_project_id, entry_location_id, entry_category_id
order by entry_user_id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var entries = connection.Query<Entry>(sql, new { courseId });
                return entries.AsList();
            }
        }

        public int CreateComment (string commenter_id, string comment)
        {
            DateTime timestamp = DateTime.Now;
            string sql = @"
INSERT INTO [SEI_Galileo].[dbo].[COMMENT] (commenter_id, comment_text, created_at) VALUES (@commenter_id, @comment, @timestamp);
SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var id = connection.Query<int>(sql, new { commenter_id, comment, timestamp }).Single();
                return id;
            }
        }

        public void LinkComment (int commentId, string[] recipients)
        {
            string sql = @"INSERT INTO [SEI_Galileo].[dbo].[RECIPIENTS] (comment_id, recipient_id) VALUES ";
            var values = new List<string>();

            foreach (string recipient in recipients)
                values.Add("(" + commentId + ", " + recipient + ")");

            if (values.Any())
            {
                sql += string.Join(",", values);
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    connection.Query(sql);
                }
            }
        }

        public List<Comment> GetComments (string userId)
        {
            string sql = @"SELECT commenter.id
, commenter.created_at
, commenter.comment_text
, commenter.commenter_id
, commenter.commenter_first_name
, commenter.commenter_last_name
, recipient.recipient_id
, recipient.recipient_first_name
, recipient.recipient_last_name
, CASE WHEN commenter_id =  @userId
	Then 1
	Else 0
  END AS user_is_commenter

 FROM
(select id, comment_text, commenter_id, created_at, u.user_first_name as commenter_first_name, 
	u.user_last_name as commenter_last_name
from [SEI_Galileo].[dbo].[COMMENT] c join
	 [SEI_TimeMachine2].[dbo].[USER] u on c.commenter_id = u.user_id) AS commenter

JOIN

(select comment_id, recipient_id, u.user_first_name as recipient_first_name, 
	u.user_last_name as recipient_last_name
	from [SEI_Galileo].[dbo].[RECIPIENTS] r
	join
	 [SEI_TimeMachine2].[dbo].[USER] u on r.recipient_id = u.user_id) AS recipient

on commenter.id = recipient.comment_id 

where commenter_id = @userId or recipient_id = @userId";
            
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var comments = connection.Query<Comment>(sql, new { userId });
                return comments.AsList();
            }
        }
    }
}