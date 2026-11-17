namespace Api.Models.Base;

public class BaseEditableEntity : BaseEntity
{
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}