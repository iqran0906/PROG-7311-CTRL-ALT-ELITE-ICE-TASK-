namespace MediLink.Dtos
{
    public class DrugInfoDto
    {
        public string ProductNdc { get; set; } = string.Empty; //Product Nmuber 
        public string GenericName { get; set; } = string.Empty;
        public string LabelerName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public List<ActiveIngredientDto> ActiveIngredients { get; set; } = new();
    }
}