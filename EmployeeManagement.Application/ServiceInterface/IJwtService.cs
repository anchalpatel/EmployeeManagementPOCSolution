﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Core.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Application.ServiceInterface
{
    public interface IJwtService
    {
        Task<string> GenerateToken(TokenDTO tokenDto);

    }
}
