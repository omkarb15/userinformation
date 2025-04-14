using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserInformation.Model;

namespace UserInformation.Controllers
{
    [Route("api/hobbies")]
    [ApiController]
    public class HobbyController : ControllerBase
    {
        public readonly UserContext _context;
        
        public HobbyController (UserContext context)
        {
            _context = context;
        }

        [HttpGet]
     public async Task<ActionResult<IEnumerable<Hobby>>> getAllhobby()
        {
            return await _context.Hobbies.ToListAsync();
        }
        



    }
}
