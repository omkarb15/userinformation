using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using UserInformation.Model;
using UserInformation.Repositories;

namespace UserInformation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TreesController : ControllerBase
    {
       private readonly ITreeRepository _treeRepository;

        public TreesController(ITreeRepository treeRepository)
        {
            _treeRepository = treeRepository;
        }

        [HttpGet("GetTreeData")]
        public async  Task<IEnumerable<Tree>> GetAllNodes()
        {
            var nodes=await _treeRepository.GetAllNodesAsync();
            return nodes;

        }

        [HttpPost("AddNewNode")]
        public async Task<ActionResult<Tree>> AddNode([FromBody] Tree node)
        {
            if (node == null)
            {
                return BadRequest("Node Data is Invalid");
            }
            await _treeRepository.AddNodeAsync(node);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> updateNode(int id, [FromBody] Tree node)
        {
            if (node == null)
            {
                return BadRequest("Invalid Node Data");
            }

            try
            {
                await _treeRepository.UpdateNodeAsync(id, node);
                return Ok(new {message="node updated succsessfully"});
            }
            catch(KeyNotFoundException)
            {
                return NotFound(new {message="node not Found"});
            }
        }

        [HttpDelete("{id}/DeleteNode")]
        public async Task<ActionResult> deletNode(int id)
        {
            var node = await _treeRepository.GetNodeByIdAsync(id);
            if (node == null)
            {
                return NotFound();
            }

            await _treeRepository.DeleteNodeAsync(id);
            return NoContent();
        }
        [HttpGet("{treeViewId}/GetTreeDragDrop")]

        public async Task<ActionResult<IEnumerable<TreeDragDrop>>> GetTreeByTreeViewId(int treeViewId)
        {
            var nodes = await _treeRepository.GetAllTree(treeViewId);
            if (nodes == null)
            {
                return NotFound();
            }
            return Ok(nodes);
        }

        [HttpPut("DragAndDrop")]
        public async Task<ActionResult> updateDragDropNode([FromBody] TreeDragDrop node)
        {
            
            
                await _treeRepository.UpdateDragdropAsync(node);
                return Ok(new { message = "Node Updated Successfully" });
            
        }
        [HttpGet("GetCheckBoxTree")]

        public async Task<IEnumerable<CheckBoxTree>> GetCheckBoxTree()
        {
            return await _treeRepository.GetAllCheckBoxTree();
        }

        [HttpPut("ChangeIschecked")]
        public async Task<ActionResult> UpdateIsChecked([FromBody]CheckBoxTree node)
        {
            await _treeRepository.updateCheckBox(node);
            return Ok(new { message = "node Updated SuccessFully" });
        }

        [HttpGet("GetCustomerForRowReorder")]

        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            return await _treeRepository.GetAllcustomerAsync();
        }

        [HttpPost("Update-order")]

        public async Task <IActionResult> updateOrder([FromBody] List<Customer> customers)
        {
            await _treeRepository.UpdateCustomerOrderAsync(customers);
            return Ok();

        }

        [HttpGet("InStockProduct")]
        public async Task<IEnumerable<Product>> GetInStockProducts()
        {
            return await _treeRepository.GetInStockProductsAsync();
        }

        [HttpGet("DiscontinuedProduct")]
        public async Task<IEnumerable<Product>> GetDiscontinuedProducts()
        {
            return await _treeRepository.GetDiscontinuedProductsAsync();
        }

        //[HttpPut("UpdateForProduct/{id}")]
        //public async Task<IActionResult> UpdateStatus(int id , [FromBody] Product dto)
        //{
        //     await _treeRepository.UpdateProductStatusAsync(id, dto.Discontinued);
        //    return NoContent();
        //}
        [HttpPut("UpdateForProduct/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] Product dto)
        {
            var updatedProduct = await _treeRepository.UpdateProductStatusAsync(id, dto.Discontinued);

            if (updatedProduct == null)
            {
                return NotFound();
            }

            return Ok(updatedProduct);
        }


    }





}
