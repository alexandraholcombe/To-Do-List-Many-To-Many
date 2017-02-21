using Nancy;
using System;
using System.Collections.Generic;
using Nancy.ViewEngines.Razor;

namespace ToDoListSql
{
  public class HomeModule: NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => {
        List<Category> AllCategories = Category.GetAll();
        return View["index.cshtml", AllCategories];
      };
    }
  }
}
