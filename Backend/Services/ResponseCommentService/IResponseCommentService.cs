using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ResponseCommentService
{
    public interface IResponseCommentService
    {
        Task CreateResponse(ResponseComment comment);
        Task AddResponseAsync(ResponseComment comment);
    }
}
