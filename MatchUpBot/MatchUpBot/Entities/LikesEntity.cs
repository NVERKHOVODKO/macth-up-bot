﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage;
using Telegram.Bot.Types;

namespace Entities;

public class LikesEntity
{
    // Внешний ключ для пользователя, который поставил лайк
    [Required] public int LikedByUserId { get; set; }
    // Внешний ключ для пользователя, которому поставили лайк
    [Required] public int LikedUserId { get; set; }
    [Required] public DateTime Data { get; set; }
    [Required] public bool IsActive { get; set; }
}