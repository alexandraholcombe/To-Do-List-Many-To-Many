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

      Task firstTask = new Task("Mow the lawn", 1, testDate);
      Task secondTask = new Task("Mow the lawn", 1, testDate);

      //Assert
      Assert.Equal(firstTask, secondTask);
    }
    [Fact]
    public void Test_Save_SavesToDatabase()
    {
      //Arrange
      DateTime testDate = new DateTime(2017, 2, 21);

      Task testTask = new Task("Mow the lawn", 1, testDate);

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
      Task testTask = new Task("Mow the lawn", 1, testDate);

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
      Task testTask = new Task("Mow the lawn", 1, testDate);
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
      Task firstTask = new Task("Mow the lawn", 1, testDate);
      Task secondTask = new Task("Mow the lawn", 1, testDate);

      //Assert
      Assert.Equal(firstTask, secondTask);
    }

    [Fact]
    public void Test_GetTasks_OrdersAllTasksByDueDate()
    {
      DateTime firstTestDate = new DateTime(2017, 02, 21);
      DateTime secondTestDate = new DateTime(2017, 02, 10);
      Task firstTask = new Task("Mow the lawn", 1, firstTestDate);
      firstTask.Save();
      Task secondTask = new Task("Do the dishes", 1, secondTestDate);
      secondTask.Save();

      List<Task> testTaskList = new List<Task> {secondTask, firstTask};
      List<Task> resultTaskList = Task.GetAll();

      foreach (Task task in testTaskList)
      {
        Console.WriteLine("TEST: " + task.GetDescription());
      }

      foreach (Task task in resultTaskList)
      {
        Console.WriteLine("ACTUAL: " + task.GetDescription());
      }

      Assert.Equal(testTaskList, resultTaskList);
    }
  }
}
