using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using UserInformation.Model;

namespace UserInformation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;


        public UsersController(UserContext context , IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAll()
        {
            var result = await _context.Assets
                 .OrderByDescending(asset => asset.Id)
                .Select(asset => new Asset
                {
                    Id = asset.Id,
                    FirstName = asset.FirstName,
                    SurName = asset.SurName,
                    DOB = asset.DOB,
                    Gender = asset.Gender,
                    EmialId = asset.EmialId,
                    UserName = asset.UserName,
                    PassWord = asset.PassWord,
                    ProfilImage = asset.ProfilImage,
                    HobbyId =
                    string.Join(", ", _context.AssetHobbies
                    .Where(ah => ah.AssetId == asset.Id)
                    .Select(ah => ah.HobbyId)
                    .ToList()  // result store in memory
                    ),

                    HobbyName = string.Join(", ",
                        _context.AssetHobbies
                            .Where(ah => ah.AssetId == asset.Id)
                            .Join(_context.Hobbies,
                                  ah => ah.HobbyId,
                                  h => h.Id,
                                  (ah, h) => h.HobbyName)
                            .ToList()
                    )

                })
                .ToListAsync();

            return Ok(result);
        }
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Asset>>> GetAll()
        //{
        //    var result = await _context.Assets
        //        .FromSqlRaw("EXEC sp_GetAllAssetsWithHobbies")
        //        .ToListAsync();

        //    return Ok(result);
        //}






        [HttpPost]
        public async Task<ActionResult<Asset>> AddUser([FromForm] Asset asset, [FromForm] IFormFile? file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                    asset.ProfilImage = imageUrl;
                }

           


                await _context.Assets.AddAsync(asset);
                await _context.SaveChangesAsync();

                List<int> hobbyIdList = asset.HobbyId.Split(',').Select(int.Parse).ToList();

                foreach (var id in hobbyIdList)
                {
                    AssetHobby assetHobby = new AssetHobby
                    {
                        AssetId = asset.Id,  
                        HobbyId = id
                    };

                    _context.AssetHobbies.Add(assetHobby);  
                }

                await _context.SaveChangesAsync(); 


                return Ok(asset);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }





        [HttpPut("{id}")]
        public async Task<ActionResult<Asset>> EditUser([FromForm] Asset asset, IFormFile? file)
        {
            var user = await _context.Assets.FindAsync(asset.Id);
            if (user == null)
            {
                return NotFound();
            }

            
            user.FirstName = asset.FirstName;
            user.SurName = asset.SurName;
            user.DOB = asset.DOB;
            user.Gender = asset.Gender;
            user.EmialId = asset.EmialId;
            user.UserName = asset.UserName;
            user.PassWord = asset.PassWord;

            
            if (file != null && file.Length > 0)
            {
                
                if (!string.IsNullOrEmpty(user.ProfilImage)) // To delete Previous Image 
                {
                    var previousImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", Path.GetFileName(user.ProfilImage));
                    if (System.IO.File.Exists(previousImagePath))
                    {
                        System.IO.File.Delete(previousImagePath);
                    }
                }

               
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filepath = Path.Combine(uploadsFolder, filename);

                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{filename}";
                user.ProfilImage = imageUrl;
            }

            
            if (!string.IsNullOrEmpty(asset.HobbyId))
            {
                var hobbyIds = asset.HobbyId.Split(',').Select(int.Parse).ToList();

                
                var existingHobbies = _context.AssetHobbies.Where(ah => ah.AssetId == asset.Id); //DELETE previous hobby
                _context.AssetHobbies.RemoveRange(existingHobbies);

            
                foreach (var hobbyId in hobbyIds) // ADD NEW HOBBIES
                {
                    _context.AssetHobbies.Add(new AssetHobby { AssetId = asset.Id, HobbyId = hobbyId });
                }
            }

            _context.Assets.Update(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _context.Assets.FindAsync(id);
            if (user == null)
            {
                return NotFound("User is not found");
            }

            //  Delete related UserAnswers
            var userAnswers = await _context.UserAnswers
                                    .Where(ua => ua.UserId == id)
                                    .ToListAsync();

            if (userAnswers.Any())
            {
                _context.UserAnswers.RemoveRange(userAnswers);
                await _context.SaveChangesAsync(); //  Save here to avoid FK conflict
            }

            //  Delete related AssetHobbies
            var existingHobbies = await _context.AssetHobbies
                                        .Where(ah => ah.AssetId == id)
                                        .ToListAsync();

            if (existingHobbies.Any())
            {
                _context.AssetHobbies.RemoveRange(existingHobbies);
                await _context.SaveChangesAsync(); //  Save again to make sure all dependencies are cleared
            }

            // Delete profile image (optional cleanup)
            if (!string.IsNullOrEmpty(user.ProfilImage))
            {
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", Path.GetFileName(user.ProfilImage));
                var fileinfo = new FileInfo(filepath);
                if (fileinfo.Exists)
                {
                    fileinfo.Delete();
                }
            }

            // Finally delete the user
            _context.Assets.Remove(user);
            await _context.SaveChangesAsync(); //  Now safe to delete user

            return NoContent();
        }

        [HttpPost("DeleteUsers")]
        public IActionResult DeleteUsers([FromBody] List<int?> userIds)
        {
            var usersToDelete = _context.Assets.Where(u => userIds.Contains(u.Id)).ToList();
            _context.Assets.RemoveRange(usersToDelete);
            _context.SaveChanges();

            return Ok(new { message = "Users deleted successfully" });
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] Asset loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.PassWord))
            {
                return BadRequest(new { message = "userName and Password are required" });
            }

            var user = await _context.Assets.FirstOrDefaultAsync(u => u.UserName == loginRequest.UserName);
            if (user == null || user.PassWord != loginRequest.PassWord)
            {
                return Unauthorized(new { message = "Invalid Username or password" });
            }

            var secretKey = _configuration["Jwt:SecretKey"];                                                   //Get the secret key from configuration
            if (string.IsNullOrEmpty(secretKey))
            {
                return StatusCode(500, new { message = "JWT Secret Key is not configured" });
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));                //SymmetricSecurityKey means both the issuer and the consumer use the same key to sign and verify the JWT.   This converts the secretKey string into a byte array. Cryptographic algorithms generally work with byte data rather than plain text.
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //var dynamicIssuer = "Issuer-" + user.UserName;                                                    // or based on some other logic


            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),            //Here, you're saying: "The token is issued for this username".
             new Claim("userId", user.Id?.ToString() ?? ""),  
             new Claim("email", user.EmialId ?? ""),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())     //"jti" = JWT ID — a unique identifier for the token. generate unique identifier  
             };

            var token = new JwtSecurityToken(                      //this create jwt token here ,This line creates a JWT (JSON Web Token) using the JwtSecurityToken class. 
                issuer:      _configuration["Jwt:Issuer"],   //is retrieving the issuer value from the appsettings.json 
                audience: _configuration["Jwt:Audience"],       //gets the audience value from the configuration.
                claims: claims,
                  expires: DateTime.UtcNow.AddMinutes(50),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);       //   This line converts the JWT object (token) into a compact serialized string format — this is the actual token
            var issuerValue = token.Issuer;                                          // The WriteToken() method takes your JWT object (JwtSecurityToken) and turns it into a string. 
            Console.WriteLine("Token Issuer: " + issuerValue);


            return Ok(new
            {
                message = "Login Successful!",
                token = tokenString,
                issuer = issuerValue,
                userId = user.Id,
                username = user.UserName,
                firstName = user.FirstName,
                surName = user.SurName,
                gender = user.Gender
            });
        }



        [HttpGet("GetQuestions/{gender}")]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions(String gender)
            {
            var Questions = await _context.Questions.Where(q => q.Gender == gender).ToListAsync();
            if (!Questions.Any())
            {
                return NotFound("No Questions For This gender");
            }
            return Ok(Questions);

        }


        [HttpPost("SubmitAnswer")]
        public async Task<ActionResult> SubmitAns([FromBody] List<UserAnswer> userAnswers)
         {
            if (userAnswers == null || !userAnswers.Any())
            {
                return BadRequest("No answers provided");
            }

            int userId = userAnswers.First().UserId; // taking userId from first one

            var existingUser = _context.UserAnswers.Where(a => a.UserId == userId);
            _context.UserAnswers.RemoveRange(existingUser);
            await _context.SaveChangesAsync();

            _context.UserAnswers.AddRange(userAnswers);
            await _context.SaveChangesAsync();

                   // Get the total number of answers submitted by this user
            int answeredCount = await _context.UserAnswers
                                  .Where(a => a.UserId == userId)
                                  .CountAsync();

            return Ok(new { message = "Form Submitted Successfully!", totalAnswered = answeredCount });
        }
        [HttpGet("GetUserAns/{userId}")]
        public async Task<ActionResult<IEnumerable<UserAnswer>>> Getanswers(int userId)
        {
            var Useranswer = await _context.UserAnswers.
                Where(a => a.UserId == userId).ToListAsync();
            int answeredCount = await _context.UserAnswers
                                  .Where(a => a.UserId == userId)
                                  .CountAsync();

            if(Useranswer== null || Useranswer.Count == 0)
            {
                return NotFound("No Answer Found For this user!");
            }
            return Ok(new {answers= Useranswer , totalAnswered= answeredCount } );
        }

        [HttpGet("QuestionOpt")]


     
        public async Task<ActionResult<IEnumerable<object>>> GetQuestionsWithOptions()
        {
            var questionsWithOptions = await (
                from q in _context.QuestionOpts
                join o in _context.Options on q.Id equals o.QuestionId into questionOptions
                select new
                {
                    QuestionId = q.Id,
                    questionText = q.QuestionText,
                    Options = questionOptions.Select(o => new
                    {
                        OptionId = o.Id,
                        optionText = o.OptionText
                    }).ToList()
                }
            ).ToListAsync();

            return Ok(questionsWithOptions);
        }

        [HttpPost("UserAnswers")]
        public async Task<ActionResult> SubmitAns([FromBody] List<UserOption> userOptions)
        {
             if (userOptions== null )
            {
                return BadRequest("No answer Provided");

            }
            int userId = userOptions.First().UserId; // taking userId from first one in list

            var existingUser = _context.UserOptions.Where(a => a.UserId == userId);
            _context.UserOptions.RemoveRange(existingUser);                               //delete previous ans for this user
            await _context.SaveChangesAsync();

            _context.UserOptions.AddRange(userOptions);
            await _context.SaveChangesAsync();

            return Ok( new {message=" Answer Submit SuccessFully!"} );
        }

        [HttpGet("GetAnswithOption/{userId}")]

        public async Task<ActionResult<IEnumerable<UserOption>>> GetAnsWithOption(int userId)
        {
            var userAnswers = await _context.UserOptions.
                Where(a => a.UserId == userId).ToListAsync();

            if (userAnswers== null || userAnswers.Count == 0)
            {
                return NotFound("No answer Found");
            }
            return Ok(userAnswers);
        }

        [HttpGet("GetBinding")]
        public async Task<ActionResult<IEnumerable<Category>>> getAllCategories()
        {
            var Result= await _context.Categories.ToListAsync();
            return Ok(Result);
        }
    



    }



}

