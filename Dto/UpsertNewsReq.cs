namespace Backend.Dto {
    public class UpsertNewsReq {
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string? ImgUrl { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}
