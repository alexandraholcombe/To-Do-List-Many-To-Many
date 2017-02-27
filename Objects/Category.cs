using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace ToDoListSql
{
  public class Category
  {
    private int _id;
    private string _name;

    public Category(string Name, int Id = 0)
    {
      _id = Id;
      _name = Name;
    }

    public override bool Equals(System.Object otherCategory)
    {
      if (!(otherCategory is Category))
      {
        return false;
      }
      else
      {
        Category newCategory = (Category) otherCategory;
        bool idEquality = this.GetId() == newCategory.GetId();
        bool nameEquality = this.GetName() == newCategory.GetName();
        return (idEquality && nameEquality);
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
    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }

    public static List<Category> GetAll()
    {
      List<Category> allCategories = new List<Category>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM categories;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int categoryId = rdr.GetInt32(0);
        string categoryName = rdr.GetString(1);
        Category newCategory = new Category(categoryName, categoryId);
        allCategories.Add(newCategory);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allCategories;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO categories (name) OUTPUT INSERTED.id VALUES (@CategoryName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@CategoryName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM categories;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public void DeleteCategory()
    {
        SqlConnection conn = DB.Connection();
        conn.Open();

        SqlCommand cmd = new SqlCommand("DELETE FROM categories WHERE id = @CategoryId;", conn);
        cmd.Parameters.Add(new SqlParameter("@CategoryId", this.GetId()));
        cmd.ExecuteNonQuery();

        if (conn != null)
        {
        conn.Close();
        }
    }

    public static Category Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM categories WHERE id = @CategoryId;", conn);
      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = id.ToString();
      cmd.Parameters.Add(categoryIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundCategoryId = 0;
      string foundCategoryDescription = null;

      while(rdr.Read())
      {
        foundCategoryId = rdr.GetInt32(0);
        foundCategoryDescription = rdr.GetString(1);
      }
      Category foundCategory = new Category(foundCategoryDescription, foundCategoryId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundCategory;
    }

    // public List<Task> GetTasks()
    // {
    //   SqlConnection conn = DB.Connection();
    //   conn.Open();
    //   SqlCommand cmd = new SqlCommand("SELECT * FROM tasks WHERE category_id = @CategoryId ORDER BY due_date;", conn);
    //   SqlParameter categoryIdParameter = new SqlParameter();
    //   categoryIdParameter.ParameterName = "@CategoryId";
    //   categoryIdParameter.Value = this.GetId();
    //   cmd.Parameters.Add(categoryIdParameter);
    //   SqlDataReader rdr = cmd.ExecuteReader();
    //
    //   List<Task> tasks = new List<Task> {};
    //   while(rdr.Read())
    //   {
    //     int taskId = rdr.GetInt32(0);
    //     string taskDescription = rdr.GetString(1);
    //     DateTime taskDueDate = rdr.GetDateTime(2);
    //     Task newTask = new Task(taskDescription, taskDueDate, taskId);
    //     tasks.Add(newTask);
    //   }
    //   if (rdr != null)
    //   {
    //     rdr.Close();
    //   }
    //   if (conn != null)
    //   {
    //     conn.Close();
    //   }
    //   return tasks;
    // }

    public void AddTask(Task newTask)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO categories_tasks (category_id, task_id) VALUES (@CategoryId, @TaskId);", conn);
      cmd.Parameters.Add(new SqlParameter("@CategoryId", this.GetId()));
      cmd.Parameters.Add(new SqlParameter("@TaskId", newTask.GetId()));

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Task> GetTasks()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT task_id FROM categories_tasks WHERE category_id = @CategoryId;", conn);
      cmd.Parameters.Add(new SqlParameter("@CategoryId", this.GetId()));

      SqlDataReader rdr = cmd.ExecuteReader();

      List<int>TaskIds = new List<int>{};

      while(rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        TaskIds.Add(taskId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<Task> tasks = new List<Task> {};
      foreach (int taskId in TaskIds)
      {
        SqlCommand newcmd = new SqlCommand("SELECT * FROM tasks WHERE id = @TaskId;", conn);
        newcmd.Parameters.Add(new SqlParameter("@TaskId",taskId));

        SqlDataReader newRdr = newcmd.ExecuteReader();

        while(newRdr.Read())
        {
          int newId = newRdr.GetInt32(0);
          string newDescription = newRdr.GetString(1);
          DateTime newDateTime = newRdr.GetDateTime(2);
          Task newTask = new Task(newDescription, newDateTime, newId);
          tasks.Add(newTask);
        }

        if (newRdr != null)
        {
          newRdr.Close();
        }
      }

      if (conn != null)
      {
        conn.Close();
      }

      return tasks;
    }
  }
}
