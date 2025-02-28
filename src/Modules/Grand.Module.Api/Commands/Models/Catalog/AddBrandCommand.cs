﻿using Grand.Module.Api.DTOs.Catalog;
using MediatR;

namespace Grand.Module.Api.Commands.Models.Catalog;

public class AddBrandCommand : IRequest<BrandDto>
{
    public BrandDto Model { get; set; }
}