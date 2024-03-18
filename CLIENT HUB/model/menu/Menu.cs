
namespace BPOI_HUB.model.menu
{
    public class MenuItem
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public int Parent { get; set; }
        public int Index { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
    }
}
