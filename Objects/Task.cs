using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace ToDoListSql
{
  public class Task
  {
    private int _id;
    private string _description;
    private DateTime _dueDate;

    public Task(string Description, DateTime dueDate, int Id = 0)
    {
      _id = Id;
      _description = Description;
      _dueDate = dueDate;
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
        return (idEquality && descriptionEquality && dueDateEquality);
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
    public DateTime GetDueDate()
    {
      return _dueDate.Date;
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
        DateTime taskDueDate = rdr.GetDateTime(2);
        Task newTask = new Task(taskDescription, taskDueDate, taskId);
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

      SqlCommand cmd = new SqlCommand("INSERT INTO tasks (description, due_date) OUTPUT INSERTED.id VALUES (@TaskDescription, @TaskDueDate);", conn);

      SqlParameter descriptionParameter = new SqlParameter();
      descriptionParameter.ParameterName = "@TaskDescription";
      descriptionParameter.Value = this.GetDescription();
      cmd.Parameters.Add(descriptionParameter);

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
    public void DeleteTask()
    {
        SqlConnection conn = DB.Connection();
        conn.Open();
        SqlCommand cmd = new SqlCommand("DELETE FROM tasks WHERE id = @TaskId; DELETE FROM categories_tasks WHERE task_id = @TaskId;", conn);
        cmd.Parameters.Add(new SqlParameter("@TaskId", this.GetId()));
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
      DateTime foundDueDate = new DateTime(1980, 01, 01);

      while(rdr.Read())
      {
        foundTaskId = rdr.GetInt32(0);
        foundTaskDescription = rdr.GetString(1);
        foundDueDate = rdr.GetDateTime(2);
      }
      Task foundTask = new Task(foundTaskDescription, foundDueDate, foundTaskId);

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

    public void AddCategory(Category newCategory)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO categories_tasks (category_id, task_id) VALUES (@CategoryId, @TaskId);", conn);
      cmd.Parameters.Add(new SqlParameter("@CategoryId", newCategory.GetId()));
      cmd.Parameters.Add(new SqlParameter("@TaskId", this.GetId()));

      cmd.ExecuteNonQuery();

      if(conn != null)
      {
        conn.Close();
      }
    }

    public List<Category> GetCategories()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT category_id FROM categories_tasks WHERE task_id = @TaskId;", conn);
      cmd.Parameters.Add(new SqlParameter("@TaskId", this.GetId()));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<int> categoryIds = new List<int> {};

      while (rdr.Read())
      {
        int categoryId = rdr.GetInt32(0);
        categoryIds.Add(categoryId);
      }

      if(rdr != null)
      {
        rdr.Close();
      }

      List<Category> categories = new List<Category>{};

      foreach (int categoryId in categoryIds)
      {
        SqlCommand newCmd = new SqlCommand("SELECT * FROM categories WHERE id = @CategoryId;", conn);
        newCmd.Parameters.Add(new SqlParameter("@CategoryId", categoryId));
        SqlDataReader newRdr = newCmd.ExecuteReader();

        while (newRdr.Read())
        {
          int newCategoryId = newRdr.GetInt32(0);
          string newCategoryName = newRdr.GetString(1);
          Category newCategory = new Category(newCategoryName,newCategoryId);
          categories.Add(newCategory);
        }

        if (newRdr != null)
        {
          newRdr.Close();
        }
      }

      if(conn != null)
      {
        conn.Close();
      }

      return categories;
    }
  }
}
