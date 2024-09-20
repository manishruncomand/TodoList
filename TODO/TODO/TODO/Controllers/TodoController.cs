using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TODO.Models;
using TODO.Service;

namespace TODO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoservice;

        public TodoController(TodoService todoservice)
        {
            _todoservice = todoservice;
        }


        [HttpGet]
        public ActionResult<IEnumerable<TodoItem>> Get(int pageNumber=1,int pageSize = 10)
        {
            try
            {
                var todoItems = _todoservice.GetAllTodo();
                if (todoItems == null || !todoItems.Any())
                {
                    return NotFound(new { message = "No records found." });
                }
                var totalRecords = todoItems.Count();
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
                var paginatedItems = todoItems
                                     .Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToList();
                if (!paginatedItems.Any())
                {
                    return NotFound(new { message = "No records found for this page." });
                }
                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    Items = paginatedItems
                };
                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while getting the records.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<TodoItem> GetTodoById(long id)
        {
            try
            {
                var item = _todoservice.GetTodoById(id);
                if (item == null)
                    return NotFound();

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while retrieving the item.",
                    error = ex.Message

                });
            }

        }

        [HttpPost]
        public ActionResult Create([FromBody] TodoItem todoItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                int affectedRows = _todoservice.Create(todoItem);
                if (affectedRows > 0)
                {
                    return Ok("Item added successfully");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong while adding item!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while adding the item", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(long id, [FromBody] TodoItem todoItem)
        {
            if (id != todoItem.Id)
                return BadRequest("Todo ID mismatch");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                int affectedRows = _todoservice.Update(todoItem);
                if (affectedRows > 0)
                {
                    return Ok(new
                    {
                        message = "Item updated successfully!!"
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        message = "Todo item not found!!"
                    });
                }
            }catch(Exception ex)
            {
                return BadRequest(new { message = "An error occurred while updating the item", error = ex.Message });
            }
           
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                int deleteditem =_todoservice.Delete(id);
                if (deleteditem > 0)
                {
                    return Ok(new
                    {
                        message = "Item deleted successfully"
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        message = "Todo item not found!!"
                    });
                }
              
            }catch(Exception ex)
            {
                return BadRequest(new { message = "An error occurred while deleting the item", error = ex.Message });
            }

        }
   


    }
}
