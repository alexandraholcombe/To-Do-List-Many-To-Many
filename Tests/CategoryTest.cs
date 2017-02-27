using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ToDoListSql
{
  public class CategoryTest : IDisposable
  {
    public CategoryTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=todo_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_CategoriesEmptyAtFirst()
    {
      //Arrange Act
      int result = Category.GetAll().Count;

      //Assert
      Assert.Equal(0,result);
    }

    public void Dispose()
    {
      Task.DeleteAll();
      Category.DeleteAll();
    }

    [Fact]
    public void Test_Delete_DeletesCategoryFromDatabase()
    {
      //Arrange
      Category testCategory1 = new Category("Home stuff");
      testCategory1.Save();
      Category testCategory2 = new Category("Work stuff");
      testCategory2.Save();

      //Act
      testCategory1.DeleteCategory();
      List<Category> resultCategories = Category.GetAll();
      List<Category> testCategoryList = new List<Category> {testCategory2};

      //Assert
      Assert.Equal(testCategoryList,resultCategories);
    }
    [Fact]
    public void Test_Equal_ReturnsTrueForSameName()
    {
      //Arrange, Act
      Category firstCategory = new Category("Household chores");
      Category secondCategory = new Category("Household chores");

      //Assert
      Assert.Equal(firstCategory, secondCategory);
    }

    [Fact]
    public void Test_Save_SavesCategoryToDatabase()
    {
      //Arrange
      Category testCategory = new Category("Household chores");
      testCategory.Save();

      //Act
      List<Category> result = Category.GetAll();
      List<Category> testList = new List<Category>{testCategory};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Save_AssignsIdToCategoryObject()
    {
      //Arrange
      Category testCategory = new Category("Household chores");
      testCategory.Save();

      //Act
      Category savedCategory = Category.GetAll()[0];

      int result = savedCategory.GetId();
      int testId = testCategory.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_Find_FindsCategoryInDatabase()
    {
      //Arrange
      Category testCategory = new Category("Household chores");
      testCategory.Save();

      //Act
      Category foundCategory = Category.Find(testCategory.GetId());

      //Assert
      Assert.Equal(testCategory, foundCategory);
    }

    [Fact]
    public void Test_GetTasks_ReturnsAllCategoryTasks()
    {
      //Arrange
      Category testCategory = new Category("Household chores");
      testCategory.Save();

      DateTime testDate = new DateTime(2017, 2, 21);

      Task testTask1= new Task("Mow the lawn", testDate);
      testTask1.Save();

      Task testTask2 = new Task("Buy plane ticket", testDate);
      testTask2.Save();

      //Act
      testCategory.AddTask(testTask1);
      testCategory.AddTask(testTask2);
      List<Task> savedTasks= testCategory.GetTasks();
      List<Task> testList = new List<Task> {testTask1, testTask2};

      //Assert
      Assert.Equal(testList, savedTasks);
    }

    // [Fact]
    // public void Test_GetTasks_RetrievesAllTasksWithCategory()
    // {
    //   Category testCategory = new Category("Household chores");
    //   testCategory.Save();
    //   DateTime testDate = new DateTime(2017, 02, 21);
    //   Task firstTask = new Task("Mow the lawn", testDate);
    //   firstTask.Save();
    //   Task secondTask = new Task("Do the dishes", testDate);
    //   secondTask.Save();
    //
    //   List<Task> testTaskList = new List<Task> {firstTask, secondTask};
    //   List<Task> resultTaskList = testCategory.GetTasks();
    //
    //   Assert.Equal(testTaskList, resultTaskList);
    // }
    // [Fact]
    // public void Test_GetTasks_OrdersAllTasksByDueDate()
    // {
    //   Category testCategory = new Category("Household chores");
    //   testCategory.Save();
    //   DateTime testDate = new DateTime(2017, 02, 21);
    //   DateTime testDate2 = new DateTime(2016, 02, 21);
    //   Task firstTask = new Task("Mow the lawn", testDate);
    //   firstTask.Save();
    //   Task secondTask = new Task("Do the dishes", testDate2);
    //   secondTask.Save();
    //
    //   List<Task> testTaskList = new List<Task> {secondTask, firstTask};
    //   List<Task> resultTaskList = testCategory.GetTasks();

      // foreach (Task task in testTaskList)
      // {
      //   Console.WriteLine("TEST: " + task.GetDescription());
      // }
      //
      // foreach (Task task in resultTaskList)
      // {
      //   Console.WriteLine("ACTUAL: " + task.GetDescription());
      // }

    //   Assert.Equal(testTaskList, resultTaskList);
    // }
  }
}
