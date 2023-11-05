using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Entities;

public class InterestWeightEntity
{
     [Key] public Guid Id { get; set; }
     [Required] public long UserId { get; set; }
     public UserEntity User { get; set; }

     [DefaultValue(50)] public byte SportWeight { get; set; } = 50 ;
     [DefaultValue(50)] public byte ArtWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte NatureWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte MusicWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte TravelWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte PhotoWeight { get; set; } = 50 ;
     [DefaultValue(50)] public byte CookingWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte MovieWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte LiteratureWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte ScienceWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte TechnologiesWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte HistoryWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte PsychologyWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte ReligionWeight { get; set; }= 50 ;
     [DefaultValue(50)] public byte FashionWeight { get; set; }= 50 ;
     
}