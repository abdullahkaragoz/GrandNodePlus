﻿using Grand.Infrastructure.Models;

namespace Grand.Web.Api.Models.Orders;

public class AddOrderNoteModel : BaseModel
{
    public string OrderId { get; set; }
    public string Note { get; set; }
}