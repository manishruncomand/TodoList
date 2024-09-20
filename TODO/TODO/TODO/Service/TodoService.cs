using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TODO.Models;


namespace TODO.Service
{
    public class TodoService
    {
        private readonly string _connectionString;
        public TodoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConnectionString");
        }

        #region GetAll Items
        public IEnumerable<TodoItem> GetAllTodo()
        {
            var todoItems = new List<TodoItem>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, Title, Description, IsCompleted FROM TodoItems", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        long id = 0;
                        bool isCompleted = false;
                        long.TryParse(reader["Id"].ToString(), out id);
                        bool.TryParse(reader["IsCompleted"].ToString(), out isCompleted);
                        todoItems.Add(new TodoItem
                        {
                            Id = id,
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            IsCompleted = isCompleted
                        });
                    }
                }
            }

            return todoItems;
        }
        #endregion

        #region GetItem by Id
        public TodoItem GetTodoById(long id)
        {
            TodoItem todoItem = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, Title, Description, IsCompleted FROM TodoItems WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {

                    if (reader.Read())
                    {
                        long itemId = 0;
                        bool isCompleted = false;
                        long.TryParse(reader["Id"].ToString(), out itemId);
                        bool.TryParse(reader["IsCompleted"].ToString(), out isCompleted);
                        todoItem = new TodoItem
                        {
                            Id = itemId,
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            IsCompleted = isCompleted
                        };
                    }
                }
            }

            return todoItem;
        }
        #endregion

        #region Add Items
        public int Create(TodoItem todoItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var getMaxIdCommand = new SqlCommand("SELECT ISNULL(MAX(Id),0) +1 from TodoItems", connection);
                var maxIdResult = getMaxIdCommand.ExecuteScalar() as long?;
                todoItem.Id = maxIdResult ?? 1;
                var command = new SqlCommand(
                    "INSERT INTO TodoItems (Id,Title, Description, IsCompleted) VALUES (@Id,@Title, @Description, @IsCompleted)",
                    connection);
                command.Parameters.AddWithValue("@Id", todoItem.Id);
                command.Parameters.AddWithValue("@Title", todoItem.Title);
                command.Parameters.AddWithValue("@Description", todoItem.Description);
                command.Parameters.AddWithValue("@IsCompleted", todoItem.IsCompleted);

                int affectedRows = command.ExecuteNonQuery();
                return affectedRows;
            }
        }
        #endregion

        #region Update item
        public int Update(TodoItem todoItem)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "UPDATE TodoItems SET Title = @Title, Description = @Description, IsCompleted = @IsCompleted WHERE Id = @Id",
                    connection);
                command.Parameters.AddWithValue("@Id", todoItem.Id);
                command.Parameters.AddWithValue("@Title", todoItem.Title);
                command.Parameters.AddWithValue("@Description", todoItem.Description);
                command.Parameters.AddWithValue("@IsCompleted", todoItem.IsCompleted);
                int affectedRows = command.ExecuteNonQuery();
                return affectedRows;
            }
        }
        #endregion

        #region Delete item
        public int Delete(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM TodoItems WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                int affectedRows = command.ExecuteNonQuery();
                return affectedRows;
            }
        }
        #endregion

    }
}

