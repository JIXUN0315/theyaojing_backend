using Backend.Dto;

namespace Backend.ViewModel;

public class FormListViewModel
{
    public FormListViewModel(FormListItemDto formListItem)
    {
        this.Id = formListItem.Id;  
        this.FullName = formListItem.AnswersDto.FullName;
        this.Email = formListItem.AnswersDto.Email;
        this.CreatedAt  = formListItem.CreatedAt;
    }
    
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}