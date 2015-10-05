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
            string sql = @"SELECT u.*,
                              CASE WHEN r.position = 2
                                 THEN 1
                                 ELSE 0
                              END AS user_is_team_leader, 
	                          CASE WHEN r.position = 3
                                 THEN 1
                                 ELSE 0
                              END AS user_is_project_manager
                           FROM[USER] u
                           LEFT JOIN[SEI_Galileo].[dbo].[ROLE] r ON r.student_id = u.user_id
                           WHERE user_id = @userId";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var user = connection.Query<User>(sql, new { userId });
                return user.FirstOrDefault();
            }
        }

        public List<Course> GetCourses(string userId)
        {
            string sql = @"SELECT c.*, SUM(e.entry_total_time) as course_total_time
                           FROM [COURSE]     c
                           JOIN [MEMBER]     m ON m.member_course_id  = c.course_id
                           JOIN [PROJECT]    p ON p.project_course_id = c.course_id
                           JOIN [USER]       u ON m.member_user_id    = u.user_id
                           LEFT JOIN [ENTRY] e ON e.entry_project_id  = p.project_id
                           WHERE u.user_id = @userId
                           AND  UPPER(m.member_position) = UPPER('Teacher')
                           GROUP BY c.course_id, c.course_name, c.course_submit_day, c.course_date_created, c.course_begin_date, c.course_end_date, c.course_is_enabled, 
                                    c.course_ref_grade, c.course_ref_hours;";
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var courses = connection.Query<Course>(sql, new { userId });
                return courses.AsList();
            }
        }

        public List<Project> GetProjects(int course_id)
        {
            string sql = @"SELECT p.*, SUM(e.entry_total_time) as project_total_time,
                              CASE WHEN EXISTS(SELECT * 
                                               FROM [SEI_Galileo].[dbo].[ROLE] AS r
                                               WHERE p.project_id  = r.team_id) 
                                 THEN 1
                                 ELSE 0
                              END AS project_is_team
                           FROM PROJECT p
                           JOIN [ENTRY] e ON e.entry_project_id = p.project_id
                           WHERE p.project_course_id            = @course_id
                           GROUP BY p.project_begin_date, p.project_course_id, p.project_created_by, p.project_date_created, p.project_description, p.project_end_date, 
                                    p.project_id, p.project_id, p.project_is_enabled, p.project_name;";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var projects = connection.Query<Project>(sql, new { course_id });
                return projects.AsList();
            }
        }

        public List<Project> GetLeaderProjects(string leaderId)
        {
            string sql = @"SELECT p.*, SUM(e.entry_total_time) AS project_total_time
                           FROM [SEI_TimeMachine2].[dbo].[PROJECT] p 
                           JOIN [SEI_TimeMachine2].[dbo].[ENTRY]   e ON e.entry_project_id = p.project_id 
                           JOIN [SEI_Galileo].[dbo].[ROLE]         r ON r.team_id          = p.project_id 
                           JOIN [SEI_TimeMachine2].[dbo].[USER]    u ON r.student_id       = u.user_id 
                           WHERE user_id = @leaderId AND r.position > 1
                           GROUP BY p.project_begin_date, p.project_course_id, p.project_created_by, p.project_date_created, p.project_description, p.project_end_date, 
                                    p.project_id, p.project_id, p.project_is_enabled, p.project_name";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var projects = connection.Query<Project>(sql, new { leaderId });
                return projects.AsList();
            }
        }

        public List<User> GetUsersInCourse(int course_id)
        {
            string sql = @"SELECT u.user_id, u.user_first_name, u.user_last_name, sum(e.entry_total_time) as user_total_time, c.course_begin_date as user_begin_date, 
                                  c.course_end_date as user_end_date
                           FROM [USER]  u
                           JOIN ENTRY   e ON e.entry_user_id    = u.user_id
                           JOIN PROJECT p ON e.entry_project_id = p.project_id
                           JOIN COURSE  c ON c.course_id        = p.project_course_id
                           WHERE c.course_id = @course_id
                           GROUP BY u.user_id, u.user_first_name, u.user_last_name, c.course_begin_date, c.course_end_date
                           ORDER BY user_id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var users = connection.Query<User>(sql, new { course_id });
                return users.AsList();
            }
        }

        public List<User> GetUsersInProject(int projectId)
        {
            string sql = @" SELECT user_id, user_first_name, user_last_name, SUM(entry_total_time) AS user_total_time, p.project_begin_date AS user_begin_date, 
                                   p.project_end_date AS user_end_date
                            FROM [SEI_TimeMachine2].[dbo].[USER]    u
                            JOIN [SEI_TimeMachine2].[dbo].[entry]   e ON user_id = entry_user_id
                            JOIN [SEI_TimeMachine2].[dbo].[project] p ON project_id = entry_project_id
                            WHERE project_id = @projectId
                            GROUP BY user_id, user_first_name, user_last_name, p.project_begin_date, p.project_end_date
                            ORDER BY user_total_time";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var users = connection.Query<User>(sql, new { projectId });
                return users.AsList();
            }
        }

        public List<User> GetUsersInTeam(int teamId)
        {
            string sql = @"SELECT user_id, r.team_id, user_first_name, user_last_name, ISNULL(SUM(entry_total_time), 0) AS user_total_time, 
                                  p.project_begin_date AS user_begin_date, p.project_end_date as user_end_date,
		                      CASE WHEN r.position = 2
		                         THEN 1
		                         ELSE 0
	                          END AS user_is_team_leader, 
		                      CASE WHEN r.position = 3
		                         THEN 1
		                         ELSE 0
	                          END AS user_is_project_manager
                           FROM [SEI_Galileo].[dbo].[ROLE]            r
                           JOIN [SEI_TimeMachine2].[dbo].[USER]         ON user_id = r.student_id
                           JOIN [SEI_TimeMachine2].[dbo].[project]    p ON r.team_id = p.project_id 
                           LEFT JOIN [SEI_TimeMachine2].[dbo].[entry] e ON p.project_id = entry_project_id AND e.entry_user_id = r.student_id
                           WHERE project_id = @teamId
                           GROUP BY user_id, user_first_name, user_last_name, r.team_id, position, p.project_begin_date, p.project_end_date
                           ORDER BY user_total_time";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var users = connection.Query<User>(sql, new { teamId });
                return users.AsList();
            }
        }

        public List<User> GetUsersWithoutTeams(int courseId)
        {
            string sql = @"SELECT DISTINCT(u.user_id), u.user_first_name, u.user_last_name
                           FROM [USER]  u
                           JOIN MEMBER  m on m.member_user_id    = u.user_id
                           JOIN COURSE  c on c.course_id         = m.member_course_id
                           JOIN PROJECT p on p.project_course_id = c.course_id
                           LEFT JOIN [SEI_Galileo].[dbo].ROLE r  ON r.team_id     = p.project_id and r.student_id = u.user_id
                           WHERE c.course_id = @courseId AND u.user_id not in (SELECT distinct(r.student_id) from [PROJECT] p
																		join [SEI_Galileo].[DBO].[ROLE] r on p.project_id = r.team_id
																		where p.project_course_id = @courseId)
						   AND UPPER(m.member_position) != UPPER('Teacher')";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var users = connection.Query<User>(sql, new { courseId });
                return users.AsList();
            }
        }

        public List<User> GetUsersWithTeams(int courseId)
        {
            string sql = @"SELECT DISTINCT(u.user_id), u.user_first_name, u.user_last_name, project_id, r.student_id,
                               CASE WHEN r.position = 2
                               THEN 1
                               ELSE 0
                            END AS user_is_team_leader, 
                               CASE WHEN r.position = 3
                               THEN 1
                               ELSE 0
                            END AS user_is_project_manager
                        FROM [USER]  u
                        JOIN MEMBER  m ON m.member_user_id    = u.user_id
                        JOIN COURSE  c ON c.course_id         = m.member_course_id
                        JOIN PROJECT p on p.project_course_id = c.course_id
                        LEFT JOIN [SEI_Galileo].[dbo].ROLE r  ON r.team_id     = p.project_id AND r.student_id = u.user_id
                        WHERE c.course_id = @courseId AND r.student_id IS NOT NULL AND UPPER(m.member_position) != UPPER('Teacher')";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var users = connection.Query<User>(sql, new { courseId });
                return users.AsList();
            }
        }

        public List<User> GetAllUsers()
        {
            string sql = @"SELECT USER_ID, user_first_name, user_last_name 
                           FROM [user]";
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var users = connection.Query<User>(sql);
                return users.AsList();
            }
        }

        public List<Entry> GetUserEntries(string userId)
        {
            string sql = @"SELECT e.*, u.user_first_name, u.user_last_name, p.project_name, c.course_name, SUM(e.entry_total_time)/60 AS entry_total_hours 
                           FROM [entry] e
                           JOIN [PROJECT] p ON e.entry_project_id  = p.project_id
                           JOIN [COURSE]  c ON p.project_course_id = c.course_id
                           JOIN [user]    u ON e.entry_user_id     = u.user_id
                           WHERE e.entry_user_id = @userId
                           GROUP BY u.user_first_name, u.user_last_name, p.project_name, c.course_name, entry_id, entry_begin_time, entry_end_time, entry_total_time, 
                                    entry_work_accomplished, entry_comment, entry_user_id, entry_project_id, entry_location_id, entry_category_id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var entries = connection.Query<Entry>(sql, new { userId });
                return entries.AsList();
            }
        }


        public void DeleteAllMembers(int[] projectIds)
        {
            string sql = @"DELETE FROM [SEI_Galileo].[dbo].[ROLE] 
                           WHERE team_id IN @projectIds";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Query(sql, new { projectIds });
            }
        }

        public void InsertProjectManager(string userId, int[] projectIds)
        {
            string sql = @"INSERT INTO [SEI_Galileo].[dbo].[ROLE] (student_id, team_id, position) VALUES ";
            var values = new List<string>();

            foreach (int projectId in projectIds)
                values.Add("(" + userId + ", " + projectId + ", 3)");

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
                            values.Add("(" + userIds[userIndex] + ", " + team.projectId + ", 2)");
                        else
                            values.Add("(" + userIds[userIndex] + ", " + team.projectId + ", 1)");
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
            string sql = @"SELECT e.*, c.course_name, u.user_first_name, u.user_last_name, p.project_name, SUM(e.entry_total_time)/60 AS entry_total_hours
                           FROM[ENTRY]   e
                           JOIN[PROJECT] p ON entry_project_id    = project_id
                           JOIN[COURSE]  c ON p.project_course_id = c.course_id
                           JOIN[USER]    u ON e.entry_user_id     = u.user_id
                           WHERE c.course_id = @courseId
                           GROUP BY u.user_first_name, u.user_last_name, p.project_name, c.course_name, entry_id, entry_begin_time, entry_end_time, entry_total_time, 
                                    entry_work_accomplished, entry_comment, entry_user_id, entry_project_id, entry_location_id, entry_category_id
                           ORDER BY entry_user_id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var entries = connection.Query<Entry>(sql, new { courseId });
                return entries.AsList();
            }
        }

        public int CreateComment(string commenter_id, string comment, bool hidden)
        {
            DateTime timestamp = DateTime.Now;
            string sql = @"INSERT INTO [SEI_Galileo].[dbo].[COMMENT] (commenter_id, comment_text, hidden, created_at) VALUES (@commenter_id, @comment, @hidden, @timestamp);
                           SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var id = connection.Query<int>(sql, new { commenter_id, comment, hidden, timestamp }).Single();
                return id;
            }
        }

        public void LinkComment(int commentId, string[] recipients)
        {
            string sql = @"INSERT INTO [SEI_Galileo].[dbo].[RECIPIENTS] (comment_id, recipient_id) VALUES ";
            var values = new List<string>();

            foreach (string recipient in recipients)
                values.Add("(" + commentId + ", '" + recipient + "')");

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

        public List<Comment> GetComments(string userId)
        {
            string sql = @"SELECT commenter.id, commenter.created_at, commenter.comment_text, commenter.hidden, commenter.commenter_id, commenter.commenter_first_name, 
                                  commenter.commenter_last_name, recipient.recipient_id, recipient.recipient_first_name, recipient.recipient_last_name,
                              CASE WHEN commenter_id =  @userId
	                             THEN 1
	                             ELSE 0
                              END AS user_is_commenter
                           FROM (SELECT id, comment_text, commenter_id, created_at, hidden, u.user_first_name AS commenter_first_name, u.user_last_name AS commenter_last_name
                                 FROM [SEI_Galileo].[dbo].[COMMENT] c 
                                 JOIN [SEI_TimeMachine2].[dbo].[USER] u ON c.commenter_id = u.user_id) AS commenter
                           JOIN (SELECT comment_id, recipient_id, u.user_first_name as recipient_first_name, u.user_last_name as recipient_last_name
	                             FROM [SEI_Galileo].[dbo].[RECIPIENTS] r
	                             JOIN [SEI_TimeMachine2].[dbo].[USER]  u ON r.recipient_id = u.user_id) 
                               AS recipient
                               ON commenter.id = recipient.comment_id 
                           WHERE commenter_id = @userId OR (recipient_id = @userId and hidden = 0)
                           ORDER BY created_at desc";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var comments = connection.Query<Comment>(sql, new { userId });
                return comments.AsList();
            }
        }

        public void DeleteComment(int comment_id)
        {
            string sql = @"DELETE FROM [SEI_Galileo].[dbo].[COMMENT] 
                           WHERE id = @comment_id;
                           DELETE FROM [SEI_Galileo].[dbo].[RECIPIENTS]
                           WHERE comment_id = @comment_id;";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Query(sql, new { comment_id });
            }
        }

        public Comment GetComment(int comment_id)
        {
            string sql = @"SELECT * FROM [SEI_Galileo].[dbo].[COMMENT] WHERE id = @comment_id";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var comment = connection.Query<Comment>(sql, new { comment_id });
                return comment.First();
            }

        }

        public List<User> GetMinions(string userId, bool isTeacher)
        {
            string sql;
            if (isTeacher)
                sql = @"SELECT user_id, user_first_name, user_last_name 
                        FROM [USER]";
            else
            {
                sql = @"SELECT distinct user_id, user_first_name, user_last_name, position 
                        FROM [user] u
                        LEFT JOIN [SEI_Galileo].[DBO].[ROLE] r ON r.student_id = u.user_id
                        WHERE r.team_id IN (SELECT team_id 
                                            FROM[SEI_Galileo].[DBO].[ROLE]
                                            WHERE student_id = @userId)
                                        AND (SELECT distinct position 
                                             FROM [SEI_Galileo].[DBO].[ROLE] 
                                             WHERE student_id = @userId) > r.position
                                        OR user_id = @userId";
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var users = connection.Query<User>(sql, new { userId });
                return users.AsList();
            }
        }
    }
}