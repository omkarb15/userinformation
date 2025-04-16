using UserInformation.Model;

namespace UserInformation.Repositories
{
    public interface ITreeRepository
    {
        Task<IEnumerable<Tree>> GetAllNodesAsync();
        Task AddNodeAsync(Tree node);
        //Task DeleteNodeAsync(int id);
        Task UpdateNodeAsync(int id, Tree node);
        Task DeleteNodeAsync(int id);

        Task<Tree> GetNodeByIdAsync(int id);

        Task<IEnumerable<TreeDragDrop>> GetAllTree(int treeViewId);
        Task UpdateDragdropAsync(TreeDragDrop node);

        Task<IEnumerable<CheckBoxTree>> GetAllCheckBoxTree();
        Task updateCheckBox(CheckBoxTree node);
        Task UpdateChildNode(int parentId, bool isChecked);

        Task<IEnumerable<Customer>> GetAllcustomerAsync();
        Task UpdateCustomerOrderAsync(List<Customer> customers);

   
        Task<IEnumerable<Product>> GetInStockProductsAsync();
        Task<IEnumerable<Product>> GetDiscontinuedProductsAsync();
        Task<Product?> UpdateProductStatusAsync(int id, bool discontinued);







    }
}
