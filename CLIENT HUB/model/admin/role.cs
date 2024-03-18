namespace BPOI_HUB.model.admin
{
	public class Role
	{
		public int Id { get; set; }
		public string? Name { get; set; }

		public string[]? AllowedClients { get; set; }

		public int[]? AllowedMenus { get; set; }

		public int[]? AllowedApps { get; set; }

		public bool? IsAdmin {  get; set; }

	}
}
