namespace ChatApp.Persistence.Helpers
{
    public class UserParams
    {
		private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
		private int pageSize = 8;

		public int PageSize
        {
			get { return pageSize; }
			set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
		}

        public string? CurrentUserName { get; set; }
        public string? Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 80;
        public string? OrderBy { get; set; } = "lastActive";

    }
}
