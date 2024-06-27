namespace ChatApp.Core.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate => DateTime.Now;
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
