namespace WebAppOnDocker.Core.Model
{
    public class CategoryType
    {
        public static CategoryType Azure = new CategoryType {Id = 1, Type = "Azure"};
        public static CategoryType AmazonWebServices = new CategoryType {Id = 2, Type = "AWS"};
        public static CategoryType GoogleCloudPlatform = new CategoryType {Id = 3, Type = "GCP"};
        public static CategoryType Heroku = new CategoryType {Id = 4, Type = "Heroku"};

        public int Id { get; set; }
        public string Type { get; set; }
    }
}