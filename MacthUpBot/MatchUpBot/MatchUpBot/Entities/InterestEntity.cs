namespace Entities;

public class InterestEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<UserInterestsEntity> UserInterests { get; set; }
}