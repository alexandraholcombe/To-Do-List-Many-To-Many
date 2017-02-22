using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace ToDoListSql
{
  public class Task
  {
    private int _id;
    private string _description;
    private int _categoryId;
    private DateTime _dueDate;

    public Task(string Description, int categoryId, DateTime dueDate, int Id = 0)
    {
      _id = Id;
      _description = Description;
      _dueDate = dueDate;
      _categoryId = categoryId;
    }

    public override bool Equals(System.Object otherTask)
    {
      if (!(otherTask is Task))
      {
        return false;
      }
      else
      {
        Task newTask = (Task) otherTask;
        bool idEquality = (this.GetId() == newTask.GetId());
        bool descriptionEquality = (this.GetDescription() == newTask.GetDescription());
        bool dueDateEquality = (this.GetDueDate() == newTask.GetDueDate());
        bool categoryEquality = (this.GetCategoryId() == newTask.GetCategoryId());
        return (idEquality && descriptionEquality && categoryEquality && dueDateEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }

    public int GetId()
    {
      return _id;
    }
    public string GetDescription()
    {
      return _description;
    }
    public void SetDescription(string newDescription)
    {
      _description = newDescription;
    }
    public int GetCategoryId()
    {
      return _categoryId;
    }
    public DateTime GetDueDate()
    {
      return _dueDate.Date;
    }
    public void SetCategoryId(int newCategoryId)
    {
      _categoryId = newCategoryId;
    }
    public static List<Task> GetAll()
    {
      List<Task> allTasks = new List<Task>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks ORDER BY due_date;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        string taskDescription = rdr.GetString(1);
        int taskCategoryId = rdr.GetInt32(2);
        DateTime taskDueDate = rdr.GetDateTime(3);
        Task newTask = new Task(taskDescription, taskCategoryId, taskDueDate, taskId);
        allTasks.Add(newTask);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allTasks;
    }
    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO tasks (description, category_id, due_date) OUTPUT INSERTED.id VALUES (@TaskDescription, @TaskCategoryId, @TaskDueDate);", conn);

      SqlParameter descriptionParameter = new SqlParameter();
      descriptionParameter.ParameterName = "@TaskDescription";
      descriptionParameter.Value = this.GetDescription();
      cmd.Parameters.Add(descriptionParameter);

      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@TaskCategoryId";
      categoryIdParameter.Value = this.GetCategoryId();
      cmd.Parameters.Add(categoryIdParameter);

      SqlParameter dueDateParameter = new SqlParameter();
      dueDateParameter.ParameterName = "@TaskDueDate";
      dueDateParameter.Value = this.GetDueDate();
      cmd.Parameters.Add(dueDateParameter);

      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }
    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM tasks;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }
    public static void DeleteTask()
    {
        SqlConnection conn = DB.Connection();
        conn.Open();
        SqlCommand cmd = new SqlCommand("DELETE FROM tasks WHERE id = @TaskId;", conn);
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    public static Task Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks WHERE id = @TaskId;", conn);
      SqlParameter taskIdParameter = new SqlParameter();
      taskIdParameter.ParameterName = "@TaskId";
      taskIdParameter.Value = id.ToString();
      cmd.Parameters.Add(taskIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundTaskId = 0;
      string foundTaskDescription = null;
      int foundTaskCategoryId = 0;
      DateTime foundDueDate = new DateTime(1980, 01, 01);

      while(rdr.Read())
      {
        foundTaskId = rdr.GetInt32(0);
        foundTaskDescription = rdr.GetString(1);
        foundTaskCategoryId = rdr.GetInt32(2);
        foundDueDate = rdr.GetDateTime(3);
      }
      Task foundTask = new Task(foundTaskDescription, foundTaskCategoryId, foundDueDate, foundTaskId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return foundTask;
    }
  }
}
