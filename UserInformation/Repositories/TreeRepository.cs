using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using UserInformation.Model;

namespace UserInformation.Repositories
{
    public class TreeRepository : ITreeRepository
    {
        private readonly UserContext _context;
        private readonly DbSet<Tree> _nodes;
        private readonly DbSet<TreeDragDrop> _treeDragDrop;
        private readonly DbSet<CheckBoxTree> _checkBoxTree;
        private readonly DbSet<Customer> _customer;
        private readonly DbSet<Product> _product;

        public TreeRepository(UserContext context)
        {
            _context = context;
            _nodes = _context.Set<Tree>();
            _treeDragDrop = _context.Set<TreeDragDrop>();
            _checkBoxTree = _context.Set<CheckBoxTree>();
            _customer = _context.Set<Customer>();
            _product = _context.Set<Product>();

        }

        public async Task<Tree?> GetNodeByIdAsync(int id)
        {
            return await _nodes.FindAsync(id);
        }

        public async Task<IEnumerable<Tree>> GetAllNodesAsync()
        {
            return await _nodes.ToListAsync();
        }
        public async Task AddNodeAsync(Tree node)
        {
            await _nodes.AddAsync(node);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateNodeAsync(int id, Tree node)
        {
            var existingNode = await _nodes.FindAsync(id);
            if (existingNode == null)
            {
                throw new KeyNotFoundException("Node Not Found");
            }

            existingNode.Name = node.Name ?? existingNode.Name;  
            existingNode.ParentId = node.ParentId;                     

            _nodes.Update(existingNode);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNodeAsync(int id)
        {
            var node = await _nodes.FindAsync(id);
            if (node == null)
            {
                throw new KeyNotFoundException("Node Not Found");
            }
            _nodes.Remove(node);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TreeDragDrop>> GetAllTree(int treeViewId)
        {
            return await _treeDragDrop.Where(t => t.TreeViewId == treeViewId).ToListAsync();

        }

        public async Task UpdateDragdropAsync(TreeDragDrop node)
        {
            var existingNode = await _treeDragDrop.FindAsync(node.Id);

            if (existingNode == null)
            {
                throw new KeyNotFoundException("Node Not found");
            }
                
            existingNode.ParentId = node.ParentId;
            existingNode.TreeViewId = node.TreeViewId;

            _context.TreeDragDrops.Update(existingNode);

            var childnodes = await _treeDragDrop.Where(n => n.ParentId == existingNode.Id).ToListAsync();

            foreach(var child in childnodes)
            {
                child.TreeViewId = node.TreeViewId;
               
            }
            _treeDragDrop.UpdateRange(childnodes);
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<CheckBoxTree>> GetAllCheckBoxTree()
        {
            return await _checkBoxTree.ToListAsync();

        }

        public async Task updateCheckBox(CheckBoxTree node)
        {
            var existingNode = await _checkBoxTree.FindAsync(node.Id);
            if (existingNode == null)
            {
                throw new KeyNotFoundException("node not found");
            }   
            existingNode.IsChecked = node.IsChecked;
            await UpdateChildNode(existingNode.Id, node.IsChecked);

            _checkBoxTree.Update(existingNode);

          await _context.SaveChangesAsync();
        }
        public async Task UpdateChildNode(int parentId, bool isChecked)
        {
            var Childnodes = await _checkBoxTree.Where(n => n.ParentId == parentId).ToListAsync();
             
            foreach(var child in Childnodes)
            {
                child.IsChecked = isChecked;
                await UpdateChildNode(child.Id, isChecked);
                _checkBoxTree.Update(child);
            }


        }

        public async Task<IEnumerable<Customer>> GetAllcustomerAsync()
        {
            return await _customer.OrderBy(c => c.DisplayOrder).ToListAsync();
        }

        public async Task UpdateCustomerOrderAsync(List<Customer> customers)
        {
            foreach(var customer in customers)
            {
                var existing = await _customer.FindAsync(customer.Id);
                if (existing != null)
                {
                    existing.DisplayOrder = customer.DisplayOrder;
                }
                
            }
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Product>> GetInStockProductsAsync()
        {
            return await _product.Where(p => !p.Discontinued).ToListAsync();

        }

        public async Task<IEnumerable<Product>> GetDiscontinuedProductsAsync()
        {
            return await _product.Where(p => p.Discontinued).ToListAsync();
        }

        //public async Task UpdateProductStatusAsync(int id, bool discontinued)
        //{
        //    var product = await _context.Products.FindAsync(id);
        //    if (product != null)
        //    {
        //        product.Discontinued = discontinued;
        //    }
        //    await _context.SaveChangesAsync();
        //}

        public async Task<Product?> UpdateProductStatusAsync(int id, bool discontinued)
        {
            var product = await _product.FindAsync(id);
            if (product != null)
            {
                product.Discontinued = discontinued;
                await _context.SaveChangesAsync();
            }
            return product;
        }



    }
}