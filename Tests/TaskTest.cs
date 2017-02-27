using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ToDoListSql
{
  public class ToDoTest : IDisposable
  {
    public ToDoTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=todo_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Task.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }
    [Fact]
    public void Test_Equal_ReturnsTrueIfDescriptionsAreTheSame()
    {
      //Arrange, Act
      DateTime testDate = new DateTime(2017, 2, 21);

      Task firstTask = new Task("Mow the lawn", testDate);
      Task secondTask = new Task("Mow the lawn", testDate);

      //Assert
      Assert.Equal(firstTask, secondTask);
    }
    [Fact]
    public void Test_Save_SavesToDatabase()
    {
      //Arrange
      DateTime testDate = new DateTime(2017, 2, 21);

      Task testTask = new Task("Mow the lawn", testDate);

      //Act
      testTask.Save();
      List<Task> result = Task.GetAll();
      List<Task> testList = new List<Task>{testTask};

      //Assert
      Assert.Equal(testList, result);
    }
    public void Dispose()
    {
      Task.DeleteAll();
    }
    [Fact]
    public void Test_Save_AssignsIdToObject()
    {
      //Arrange
      DateTime testDate = new DateTime(2017, 2, 21);
      Task testTask = new Task("Mow the lawn", testDate);

      //Act
      testTask.Save();
      Task savedTask = Task.GetAll()[0];

      int result = savedTask.GetId();
      int testId = testTask.GetId();

      //Assert
      Assert.Equal(testId, result);
    }
    [Fact]
    public void Test_Find_FindsTaskInDatabase()
    {
      //Arrange
      DateTime testDate = new DateTime(2017, 2, 21);
      Task testTask = new Task("Mow the lawn", testDate);
      testTask.Save();

      //Act
      Task foundTask = Task.Find(testTask.GetId());

      //Assert
      Assert.Equal(testTask, foundTask);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameDescription()
    {
      //Arrange, Act
      DateTime testDate = new DateTime(2017, 2, 21);
      Task firstTask = new Task("Mow the lawn", testDate);
      Task secondTask = new Task("Mow the lawn", testDate);

      //Assert
      Assert.Equal(firstTask, secondTask);
    }

    [Fact]
    public void Test_GetAll_OrdersAllTasksByDueDate()
    {
      DateTime firstTestDate = new DateTime(2017, 02, 21);
      DateTime secondTestDate = new DateTime(2017, 02, 10);
      Task firstTask = new Task("Mow the lawn", firstTestDate);
      firstTask.Save();
      Task secondTask = new Task("Do the dishes", secondTestDate);
      secondTask.Save();

      List<Task> testTaskList = new List<Task> {secondTask, firstTask};
      List<Task> resultTaskList = Task.GetAll();

      // foreach (Task task in testTaskList)
      // {
      //   Console.WriteLine("TEST: " + task.GetDescription());
      // }
      //
      // foreach (Task task in resultTaskList)
      // {
      //   Console.WriteLine("ACTUAL: " + task.GetDescription());
      // }

      Assert.Equal(testTaskList, resultTaskList);
    }

    [Fact]
    public void Test_AddCategory_AddsCategoryToTask()
    {
      //Arrange
      DateTime secondTestDate = new DateTime(2017, 02, 10);

      Task testTask = new Task("Mow the Lawn", secondTestDate);
      testTask.Save();

      Category testCategory = new Category("Home Stuff");
      testCategory.Save();

      //Act
      testTask.AddCategory(testCategory);

      List<Category> result = testTask.GetCategories();
      List<Category> testList = new List<Category>{testCategory};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetCategories_ReturnsAllTaskCategories()
    {
      //Arrange
      DateTime testDate = new DateTime(2017, 2, 21);

      Task testTask = new Task("Mow the lawn", testDate);
      testTask.Save();

      Category testCategory1 = new Category("Home stuff");
      testCategory1.Save();

      Category testCategory2 = new Category("Work stuff");
      testCategory2.Save();

      //Act
      testTask.AddCategory(testCategory1);
      List<Category> result = testTask.GetCategories();
      List<Category> testList = new List<Category> {testCategory1};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Delete_DeletesTaskAssociationsFromDatabase()
    {
      //Arrange
      DateTime testDate = new DateTime(2017, 2, 21);
      Task testTask1 = new Task("Mow the lawn", testDate);
      testTask1.Save();

      Task testTask2 = new Task("Sweep the floor", testDate);
      testTask2.Save();

      Category testCategory = new Category("Home stuff");
      testCategory.Save();

      // Act
      testTask1.AddCategory(testCategory);
      testTask2.AddCategory(testCategory);
      testTask1.DeleteTask();

      List<Task> resultCategoryTask = testCategory.GetTasks();
      List<Task> testCategoryTask = new List<Task>{testTask2};

      //Assert
      Assert.Equal(resultCategoryTask,testCategoryTask);
    }
  }
}
