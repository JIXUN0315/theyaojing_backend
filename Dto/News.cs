namespace Backend.Dto {
    public class News {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }  // SunEditor HTML
        public string ImgUrl { get; set; }
        public DateTime Date { get; set; }   // yyyy-mm-dd
        public bool IsPublished { get; set; }
    }
}
