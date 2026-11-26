using mango_fusion.Data;
using mango_fusion.Models;
using mango_fusion.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace mango_fusion.Controllers
{
    [Route("api/menuItem")]
    [ApiController]
    public class MenuItemController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        public readonly ApiResponse _response;
        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _response = new ApiResponse();
            _env = env;
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

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm] MenuItemCreateDto menuItemCreateDto)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    if (menuItemCreateDto.File == null || menuItemCreateDto.File.Length == 0)
                    {

                        _response.IsSuccess=false;
                        _response.StatusCode= HttpStatusCode.BadRequest;
                        _response.ErrorMessages = ["file is required"];
                        return BadRequest(_response);
                    }

                    var imagesPath = Path.Combine(_env.WebRootPath, "images");
                    if(!Directory.Exists(imagesPath))
                    {
                        Directory.CreateDirectory(imagesPath);
                    }
                    var filePath = Path.Combine(imagesPath, menuItemCreateDto.File.FileName);
                    if(System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    //uploading the file

                    using(var stream = new FileStream (filePath,FileMode.Create))
                    {
                        await menuItemCreateDto.File.CopyToAsync(stream);
                    }
                    MenuItem menuItem = new()
                    {
                        Name = menuItemCreateDto.Name,
                        Description = menuItemCreateDto.Description,
                        Price = menuItemCreateDto.Price,
                        Category = menuItemCreateDto.Category,
                        SpecialTag = menuItemCreateDto.SpecialTag,
                        Image = "images/"+menuItemCreateDto.File.FileName
                    };
                    _db.MenuItems.Add(menuItem);
                    await _db.SaveChangesAsync();

                    _response.Result = menuItemCreateDto;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetMenuByItem", new { id = menuItem.Id }, _response);
                }

                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess= false;
                _response.ErrorMessages = [ex.ToString()];

            }
            return BadRequest(_response);
        }
    }
}
