using mango_fusion.Data;
using mango_fusion.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace mango_fusion.Controllers
{
    [Route("api/menuItem")]
    [ApiController]
    public class MenuItemController : Controller
    {

        private readonly ApplicationDbContext _db;
        public readonly ApiResponse _response;
        public MenuItemController(ApplicationDbContext db)
        {
            _db = db;
            _response = new ApiResponse();

        }
        [HttpGet]
        public IActionResult GetMenuItems()
        {
            _response.Result = _db.MenuItems.ToList();
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{id:int}",Name ="GetMenuByItem")]
        public IActionResult GetMenuById(int id)
        {
            if(id == 0)
            {
                _response.StatusCode= HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            MenuItem? menuItem = _db.MenuItems.FirstOrDefault(x => x.Id == id);
            _response.Result = menuItem;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
