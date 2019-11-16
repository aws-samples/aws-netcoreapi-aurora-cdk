using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using todo_app.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;


namespace todo_app.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private TodoContext _todoContext;

        public TodoController(IOptions<Parameters> options)
        {
            Console.WriteLine("ToDo Controller!");
            _todoContext = new TodoContext(options);
        }

        // GET: api/values
        [HttpGet]
        public List<Todo> Get()
        {
            return _todoContext.GetAllTodos();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Todo todoVal)
        {
            Console.WriteLine("Entering ToDo Put - " + JsonConvert.SerializeObject(todoVal, Formatting.Indented));

            if (todoVal != null && !string.IsNullOrEmpty(todoVal.Status) &&
                    !string.IsNullOrEmpty(todoVal.Task))
            {
                Console.WriteLine("Saving DBContext! - " + todoVal.Status + "   " + todoVal.Task);
                _todoContext.SaveTodo(todoVal.Status, todoVal.Task);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
